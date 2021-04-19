using System;
using UnityEngine;

public class TouchGesture {
    public Vector2 StartScreenPoint;
    public Vector2 CurrentScreenPoint;
    public bool IsTouching;
}

public class GesturePlatformTouch : GesturePlatform {

    private readonly GestureState state;
    private readonly Camera camera;
    private readonly TouchGesture[] touches = {
        new TouchGesture(), new TouchGesture()
    };

    private readonly GesturePlatformTouchConfig config;

    public GesturePlatformTouch(GestureState state, Camera camera, GesturePlatformTouchConfig config) {
        this.state = state;
        this.camera = camera;
        this.config = config;
    }
    
    private Ray TouchToCameraRay(Touch touch) {
        return camera.ScreenPointToRay(touch.position)
        ;
    }


    private void UpdateSingleTouch() {
        if (Input.touchCount != 1) {
            state.action = GestureAction.Idle;
            return;
        }

        var touch = Input.GetTouch(0);
        switch (touch.phase) {
            case TouchPhase.Began:
                state.action = GestureAction.Idle;
                break;
            case TouchPhase.Moved:
                if (state.action == GestureAction.Idle) {
                    state.dragState.start = TouchToCameraRay(touch);
                    state.dragState.startScreenPoint = touch.position;
                }
                state.action = GestureAction.Dragging;
                state.dragState.current = TouchToCameraRay(touch);
                state.dragState.currentScreenPoint = touch.position;
                break;
            case TouchPhase.Stationary:
                break;
            case TouchPhase.Ended:
                if (state.action == GestureAction.Idle) {
                    state.action = GestureAction.Tap;
                    state.tapState.tap = TouchToCameraRay(touch);
                    state.tapState.tapScreenPoint = touch.position;
                }
                else {
                    state.action = GestureAction.Idle;    
                }
                break;
            case TouchPhase.Canceled:
                state.action = GestureAction.Idle;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private static void PopulateDrag(Touch touch, TouchGesture gesture) {
        if (!gesture.IsTouching) {
            gesture.StartScreenPoint = touch.position;
        }
        gesture.CurrentScreenPoint = touch.position;
        gesture.IsTouching = true;
    }

    private bool UpdateTwist(Vector2 startDirection, Vector2 currentDirection) {
        var wasTwisting = state.action == GestureAction.Twisting;
        if (!wasTwisting && 1.0f - Mathf.Abs(Vector3.Dot(startDirection.normalized, currentDirection.normalized)) < config.twistTolerance) {
            return false;
        }
        
        if (!wasTwisting) {
            state.twistState.start = startDirection.normalized;
        }
        state.twistState.current = currentDirection.normalized;
        state.action = GestureAction.Twisting;
        return true;
    }

    private bool UpdateZoom(Vector2 startDirection, Vector2 currentDirection) {
        var wasZooming = state.action == GestureAction.Zooming;
        var startDistance = (startDirection).magnitude;
        var currentDistance = (currentDirection).magnitude;
        var delta = (currentDistance - startDistance) * (config.invertZoom ? -1.0f : 1.0f);
        if (!wasZooming && Mathf.Abs(delta) < config.zoomTolerance) {
            return false;
        }

        delta *= config.zoomScale;

        if (!wasZooming) {
            state.zoomState.start = state.zoomState.current;
        }

        var oldCurrent = state.zoomState.current;
        var current = state.zoomState.start + delta;
        
        state.action = GestureAction.Zooming;
        state.zoomState.current = current;
        state.zoomState.delta = current - oldCurrent;
        return true;
    }

    private void UpdateMultiTouch() {
        if (Input.touchCount < 2) {
            touches[1].IsTouching = false;
            touches[0].IsTouching = false;
            return;
        }

        PopulateDrag(Input.GetTouch(1), touches[1]);
        PopulateDrag(Input.GetTouch(0), touches[0]);
        var startDirection = touches[0].StartScreenPoint - touches[1].StartScreenPoint;
        var currentDirection = touches[0].CurrentScreenPoint - touches[1].CurrentScreenPoint;
        if (UpdateTwist(startDirection, currentDirection)) {
            return;
        }

        if (UpdateZoom(startDirection, currentDirection)) {
            return;
        }
        state.action = GestureAction.Idle;

    }

    public void Update() {
        if (Input.touchCount == 0) {
            state.action = GestureAction.Idle;
            return;
        }
        UpdateSingleTouch();
        UpdateMultiTouch();
    }

    public void OnDestroy() {}
}
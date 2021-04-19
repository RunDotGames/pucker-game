using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum MouseGestureState {
    Up, Down, Dragging, Zooming, Twisting, Tap,
}

public class GesturePlatformMouse: GesturePlatform {
    
    private static readonly Vector2 HalfPort = Vector2.one * 0.5f;
        
    private static readonly Dictionary<MouseGestureState, GestureAction> StateMapping = new Dictionary<MouseGestureState, GestureAction>() {
        {MouseGestureState.Up, GestureAction.Idle},
        {MouseGestureState.Down, GestureAction.Idle},
        {MouseGestureState.Dragging, GestureAction.Dragging},
        {MouseGestureState.Zooming, GestureAction.Zooming},
        {MouseGestureState.Twisting, GestureAction.Twisting},
        {MouseGestureState.Tap, GestureAction.Tap}
    };
    
    private static readonly HashSet<MouseGestureState> CanStartDrag = new HashSet<MouseGestureState>() {
        MouseGestureState.Up,
        MouseGestureState.Zooming
    };
    
    private readonly GesturePlatformMouseConfig config;
    private readonly GestureState state;
    private readonly Camera camera;
    private readonly EventSystem eventSystem;
    private readonly KeyActionBindingsSo keys;

    private MouseGestureState currentState = MouseGestureState.Up;
    private Vector3 downStartPosition;
    private float zoomLastScrollTime;
    
    public GesturePlatformMouse(GesturePlatformMouseConfig config, KeyActionBindingsSo keys, GestureState state, Camera camera, EventSystem eventSystem) {
        this.config = config;
        this.state = state;
        this.camera = camera;
        this.eventSystem = eventSystem;
        this.keys = keys;
        keys.OnDown += OnActionDown;
        keys.OnUp += OnActionUp;
    }
    
    public void OnDestroy() {
        keys.OnDown -= OnActionDown;
        keys.OnUp -= OnActionUp;
        
    }

    private void UpdateState(MouseGestureState aState) {
        currentState = aState;
        state.action = StateMapping[aState];
    }

    private Ray MousePositionToCameraRay() {
        var pos = Input.mousePosition;
        return camera.ScreenPointToRay(pos);
    }

    private void OnActionUp(KeyAction action) {
        if (action != KeyAction.Modify && action != KeyAction.Interact) {
            return;
        }
        
        if (action == KeyAction.Interact && currentState == MouseGestureState.Down) {
            state.tapState.tap = MousePositionToCameraRay();
            state.tapState.tapScreenPoint = Input.mousePosition;
            UpdateState(MouseGestureState.Tap);
            return;
        }
        
        UpdateState(MouseGestureState.Up);
    }

    private void OnActionDown(KeyAction action) {
        if (eventSystem.IsPointerOverGameObject()) {
            return;
        }
        
        switch (action) {
            case KeyAction.Modify:
                state.twistState.start = ((Vector2)camera.ScreenToViewportPoint(Input.mousePosition) - HalfPort).normalized;
                state.twistState.current = state.twistState.start;
                UpdateState(MouseGestureState.Twisting);
                break;
            case KeyAction.Interact:
                if (CanStartDrag.Contains(currentState)) {
                    downStartPosition = Input.mousePosition;
                    UpdateState(MouseGestureState.Down);
                }
                break;
        }
    }

    public void Update() {
        var scrollDelta = Input.mouseScrollDelta.y;
        var isScrolling = Mathf.Abs(scrollDelta) > Mathf.Epsilon;
        if (currentState != MouseGestureState.Zooming && isScrolling) {
            UpdateState(MouseGestureState.Zooming);
            state.zoomState.start = state.zoomState.current;
        }

        switch (currentState){
            case MouseGestureState.Tap:
                currentState = MouseGestureState.Up;
                state.action = GestureAction.Tap;
                break;
            case MouseGestureState.Up:
                state.action = GestureAction.Idle;
                break;
            case MouseGestureState.Down:
                var currentVector = (Input.mousePosition - downStartPosition);
                var currentDistance = currentVector.magnitude;
                if (currentDistance > config.minDragDistance) {
                    UpdateState(MouseGestureState.Dragging);
                    state.dragState.start = MousePositionToCameraRay();
                    state.dragState.startScreenPoint = Input.mousePosition;
                    state.dragState.direction = currentVector;
                }
                break;
            case MouseGestureState.Dragging:
                state.dragState.direction =  Input.mousePosition - downStartPosition;
                state.dragState.current = MousePositionToCameraRay();
                state.dragState.currentScreenPoint = Input.mousePosition;
                break;
            case MouseGestureState.Zooming:
                state.zoomState.delta = scrollDelta * (config.invertZoom ? -1.0f : 1.0f);
                state.zoomState.current += state.zoomState.delta;
                if (isScrolling) {
                    zoomLastScrollTime = Time.time;
                    break;
                }
                if (Time.time - zoomLastScrollTime > config.zoomDelaySeconds) {
                    UpdateState(MouseGestureState.Up);
                }
                break;
            case MouseGestureState.Twisting:
                state.twistState.current = ((Vector2)camera.ScreenToViewportPoint(Input.mousePosition) - HalfPort).normalized;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public interface GesturePlatform {
    void Update();
    void OnDestroy();
}

public class GestureEventProducerBeh : MonoBehaviour {
    
    private static readonly HashSet<RuntimePlatform> MousePlatforms = new HashSet<RuntimePlatform> {
        RuntimePlatform.WindowsEditor, RuntimePlatform.WindowsPlayer, RuntimePlatform.WebGLPlayer,
    };

    public GestureEventsSo gestures;
    public KeyActionBindingsSo keys;
    public EventSystem eventSystem;
    
    private GesturePlatform platform;
    private GestureState state;

    private GestureAction currentAction;
    
    public void Start() {
        currentAction = GestureAction.Idle;
        if (Camera.main == null) {
            throw new Exception("no camera in scene, cannot gesture");
        }
        
        state = new GestureState() {
            zoomState = new GestureZoomState(),
            dragState = new GestureDragState(),
            twistState = new GestureTwistState(),
            tapState = new GestureTapState(),
        };

        if (MousePlatforms.Contains(Application.platform)) {
            platform = new GesturePlatformMouse(gestures.config.mouse, keys, state, Camera.main, eventSystem);
            state.platformName = "Mouse";
            return;
        }
        
        platform = new GesturePlatformTouch(state, Camera.main, gestures.config.touch);
        state.platformName = "Touch";
        
    }

    public void OnDestroy() {
        platform?.OnDestroy();
    }

    private void InvokeStops(GestureAction lastAction) {
        switch (lastAction) {
            case GestureAction.Tap:
                break;
            case GestureAction.Idle:
                break;
            case GestureAction.Dragging:
                gestures.OnDragStop?.Invoke(state.dragState);
                break;
            case GestureAction.Zooming:
                gestures.OnZoomStop?.Invoke(state.zoomState);
                break;
            case GestureAction.Twisting:
                gestures.OnTwistStop?.Invoke(state.twistState);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void InvokeContinues() {
        switch (currentAction) {
            case GestureAction.Tap:
                break;
            case GestureAction.Idle:
                break;
            case GestureAction.Dragging:
                gestures.OnDrag?.Invoke(state.dragState);
                break;
            case GestureAction.Zooming:
                gestures.OnZoom?.Invoke(state.zoomState);
                break;
            case GestureAction.Twisting:
                gestures.OnTwist?.Invoke(state.twistState);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    public void Update() {
        if (state == null) {
            return;
        }
        platform?.Update();
        var lastAction = currentAction;
        currentAction = state.action;
        if (lastAction == currentAction) {
            InvokeContinues();
            return;
        }
        
        switch (currentAction) {
            case GestureAction.Tap:
                gestures.OnTap?.Invoke(state.tapState);
                break;
            case GestureAction.Idle:
                InvokeStops(lastAction);
                break;
            case GestureAction.Dragging:
                gestures.OnDragStart?.Invoke(state.dragState);
                break;
            case GestureAction.Zooming:
                gestures.OnZoomStart?.Invoke(state.zoomState);
                break;
            case GestureAction.Twisting:
                gestures.OnTwistStart?.Invoke(state.twistState);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

    }
    
    public GestureState State => state;
}
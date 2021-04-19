
using System;
using UnityEngine;


[Serializable]
public class GesturePlatformMouseConfig {
    public float minDragDistance = 10.0f;
    public float zoomDelaySeconds = 0.35f;
    public bool invertZoom = true;
}

[Serializable]
public class GesturePlatformTouchConfig {
    public float zoomTolerance = 100.0f;
    public float twistTolerance = 0.01f;
    public float zoomScale = 0.01f;
    public bool invertZoom = true;
}

[Serializable]
public class GestureEventsConfig {
    public GesturePlatformMouseConfig mouse;
    public GesturePlatformTouchConfig touch;
}

[CreateAssetMenu(menuName = "RDG/Input/Gesture Platform")]
public class GestureEventsSo: ScriptableObject {
    public GestureEventsConfig config;

    public Action<GestureTwistState> OnTwistStart;
    public Action<GestureTwistState> OnTwist;
    public Action<GestureTwistState> OnTwistStop;

    public Action<GestureZoomState> OnZoomStart;
    public Action<GestureZoomState> OnZoom;
    public Action<GestureZoomState> OnZoomStop;

    public Action<GestureDragState> OnDragStart;
    public Action<GestureDragState> OnDrag;
    public Action<GestureDragState> OnDragStop;

    public Action<GestureTapState> OnTap;

}

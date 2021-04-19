using UnityEngine;

public class GestureDragState {
    public Vector3 startScreenPoint;
    public Vector3 currentScreenPoint;
    public Ray start;
    public Ray current;
    public Vector3 direction;
}

public class GestureZoomState {
    public float start;
    public float current;
    public float delta;
}

public class GestureTapState {
    public Vector3 tapScreenPoint;
    public Ray tap;
}

public class GestureTwistState {
    public Vector2 start;
    public Vector2 current;
}

public enum GestureAction {
    Idle, Dragging, Zooming, Twisting, Tap
}

public class GestureState {
    public string platformName;
    public GestureAction action;
    public GestureDragState dragState;
    public GestureZoomState zoomState;
    public GestureTwistState twistState;
    public GestureTapState tapState;
}
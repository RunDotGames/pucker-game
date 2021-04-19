using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]


public class GestureDemoBeh : MonoBehaviour {
    public KeyActionBindingsSo keys;
    public GestureEventsSo gesture;
    public GestureEventProducerBeh producer;
    
    public Text debugText;
    public GestureEventsConfig config;
    private bool isIdleOut;
    public float forwardDraw;
    public GameObject startIndicator;
    public GameObject endIndicator;
    public float twistOffset = 1.0f;


    public void Update() {
        if (producer.State == null) {
            return;
        }

        debugText.text = producer.State.platformName;
        if (producer.State.action != GestureAction.Idle) {
            isIdleOut = true;
        }

        if (!isIdleOut) {
            return;
        }

        debugText.text = producer.State.action.ToString();

        if (producer.State.action == GestureAction.Dragging) {
            var start =producer.State.dragState.start.GetPoint(forwardDraw);
            var end =producer.State.dragState.current.GetPoint(forwardDraw);
            startIndicator.transform.position = start;
            endIndicator.transform.position = end;
        }

        var state = producer.State;

        if (state.action == GestureAction.Twisting) {
            if (Camera.main == null) {
                return;
            }

            var twist = state.twistState.current;
            var t = Camera.main.transform;
            var forward = t.forward;
            startIndicator.transform.position = t.position + forward * forwardDraw;
            var offset = t.rotation * (twist) * twistOffset;
            endIndicator.transform.position = startIndicator.transform.position + offset;
        }
    }
}

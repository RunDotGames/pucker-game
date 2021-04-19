using System;
using UnityEngine;

namespace RDG.PuckGame.Scripts {
  
  [RequireComponent(typeof(PuckBeh))]
  public class PuckInputBeh : MonoBehaviour {

    public GestureEventsSo gestures;
    public GamePlayControllerSo gameController;
    public PuckUIControllerSo uiController;
    
    private Puck puck;
    private PuckUIDragIndicator dragIndicator;
    
    private void OnEnable() {
      puck = GetComponent<PuckBeh>().Puck;
      gestures.OnDragStart += HandleFlickStart;
      gestures.OnDragStop += HandleFlickStop;
      gestures.OnDrag += HandleDrag;
    }
    
    private void OnDisable() {
      gestures.OnDragStart -= HandleFlickStart;
      gestures.OnDragStop -= HandleFlickStop;
      gestures.OnDrag -= HandleDrag;
    }
    
    private void HandleDrag(GestureDragState state) {
      dragIndicator?.Update(state.current.origin);
    }
    private void HandleFlickStop(GestureDragState obj) {
      if (dragIndicator == null) {
        return;
      }
      
      var result = dragIndicator.Stop();
      dragIndicator = null;
      gameController.Flick(puck, result.Direction, result.Percent);
    }
    private void HandleFlickStart(GestureDragState state) {
      if (!puck.IsFlickable) {
        return;
      }
      
      if(dragIndicator != null || !puck.InputCollider.bounds.IntersectRay(state.start)) {
        return;
      }
      
      dragIndicator = uiController.BeginFlickIndication(puck.Body.position);
      dragIndicator?.Update(state.current.origin);
    }

    
  }
}

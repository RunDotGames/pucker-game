using System;
using UnityEngine;

namespace RDG.PuckGame.Scripts {

  
  
  
  public class GameAreaBeh : MonoBehaviour {
    
    
    [SerializeField] private GameLevelSo level;
    [SerializeField] private Vector2 size;

    public Vector2 Size => size;
    
    public void OnEnable() {
      level.SetArea(new Rect(transform.position, size));
    }

  }
}

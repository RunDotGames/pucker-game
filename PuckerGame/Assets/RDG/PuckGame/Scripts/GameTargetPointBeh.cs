using System;
using UnityEngine;

namespace RDG.PuckGame.Scripts {
  public class GameTargetPointBeh : MonoBehaviour {
    [SerializeField] private GameLevelSo level;
    [SerializeField] private float radius;

    public float Radius => radius;
    private void OnEnable() {
      level.SetTarget(transform.position, radius);
    }
  }
}

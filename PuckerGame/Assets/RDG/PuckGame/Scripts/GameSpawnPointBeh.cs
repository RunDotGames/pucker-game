using System;
using UnityEngine;

namespace RDG.PuckGame.Scripts {
  public class GameSpawnPointBeh : MonoBehaviour {

    [SerializeField] private GameLevelSo level;
    [SerializeField] private float size;

    public float Size => size;
    
    private void OnEnable() {
      level.SetSpawnPoint(transform.position, size);
    }

  }
}

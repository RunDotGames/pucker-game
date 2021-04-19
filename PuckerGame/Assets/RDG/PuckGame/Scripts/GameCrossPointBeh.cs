using UnityEngine;

namespace RDG.PuckGame.Scripts {
  public class GameCrossPointBeh : MonoBehaviour{
    
    [SerializeField] private GameLevelSo level;

    private void OnEnable() {
      level.SetCrossPoint(transform.position);
    }

  }
}

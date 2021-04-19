using System;
using System.Collections.Generic;
using Scripts.RDG.UI;
using UnityEngine;
using UnityEngine.UI;

namespace RDG.PuckGame.Scripts {
  public class GameInfoHudBeh : MonoBehaviour, UIPostInitializable {
    [SerializeField] private UiThemeTextBeh puckCountText;
    [SerializeField] private UiThemeTextBeh gameScoreText;
    [SerializeField] private GamePlayControllerSo gameController;

    private void HandlePuckCountChange(int count) {
      puckCountText.SetValue($"x{count}");
    }
    
    private void HandleScoreChange(int score) {
      gameScoreText.SetValue($"{score}");
    }
    
    public IEnumerable<GameObject> PostInitialize(UiSo ui, UiTheme theme) {
      gameController.OnPuckCountChange += HandlePuckCountChange;
      gameController.OnScoreChange += HandleScoreChange;
      HandlePuckCountChange(gameController.State.PuckHandCount);
      HandleScoreChange(gameController.State.Score);
      return UnityUtil.Once(gameObject);
    }
    
    private void OnDestroy() {
      gameController.OnPuckCountChange -= HandlePuckCountChange;
      gameController.OnScoreChange -= HandleScoreChange;
    }
  }
}

using System;
using System.Collections.Generic;
using Scripts.RDG.UI;
using UnityEngine;

namespace RDG.PuckGame.Scripts {
  public class GameEndScreen : MonoBehaviour, UIPostInitializable {
    [SerializeField] private GamePlayControllerSo gameController;
    [SerializeField] private UiThemeTextBeh text;
    [SerializeField] private UiThemeButtonBeh resetButton;
    
    public IEnumerable<GameObject> PostInitialize(UiSo ui, UiTheme theme) {
      gameController.OnGameEnd += HandleGameEnd;
      resetButton.OnClick += HandleReset;
      gameObject.SetActive(false);
      return UnityUtil.Once(gameObject);
    }

    private void OnDestroy() {
      gameController.OnGameEnd -= HandleGameEnd;
      resetButton.OnClick -= HandleReset;
    }

    private void HandleGameEnd() {
      gameObject.SetActive(true);
      text.SetValue($"{gameController.State.Score}");
    }

    private void HandleReset() {
      gameObject.SetActive(false);
      gameController.ResetGame();
    }
  }
}

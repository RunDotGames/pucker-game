using System;
using System.Collections.Generic;
using UnityEngine;

namespace RDG.PuckGame.Scripts {
  public class GameStartBeh : MonoBehaviour {

    [SerializeField] private GamePlayControllerSo gameController;
    public void Start() {
      StartCoroutine(StartCheckRoutine());
    }
    private IEnumerator<YieldInstruction> StartCheckRoutine() {
      if (!gameController.Level.State.IsReady()) {
        yield return UnityUtil.EndOfFrame;
      }
      
      gameController.BeginGame();
    }

    public void OnDestroy() {
      gameController.DestroyGame();
    }
  }
}

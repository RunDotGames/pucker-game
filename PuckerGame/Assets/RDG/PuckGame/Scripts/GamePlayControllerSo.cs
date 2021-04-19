using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RDG.PuckGame.Scripts {

  public interface GamePlayState {
    int PuckHandCount { get; }
    IEnumerable<Puck> Pucks { get; }
    int Score { get; }
    
  }

  public class GamePlayStateMutable : GamePlayState {
    public int PuckHandCount { get; set; }
    public IEnumerable<Puck> Pucks => PuckList;
    public int Score { get; set; }
    public List<Puck> PuckList = new List<Puck>();
    public readonly Dictionary<Guid, Vector2> puckSettledPositions = new Dictionary<Guid, Vector2>();

    public void Reset() {
      foreach (var puck in PuckList) {
        Object.Destroy(puck.Mono.gameObject);
      }
      PuckList.Clear();
      PuckHandCount = 0;
      Score = 0;
    }
  }

  [Serializable]
  public class GameSettings {
    public int puckCount;
  }
  
  [CreateAssetMenu(menuName = "RDG/PuckGame/PuckController")]
  public class GamePlayControllerSo: ScriptableObject {

    [SerializeField] private GameLevelSo level;
    [SerializeField] private float flickForce = 10;
    [SerializeField] private float floorDrag = 4;
    [SerializeField] private float flickFollowThreshold = .2f;
    [SerializeField] private GameSettings settings;
    [SerializeField] private string scoreLayerName;

    private readonly GamePlayStateMutable state = new GamePlayStateMutable();
    public GameLevelSo Level => level;
    public GamePlayState State => state;

    public Action<int> OnPuckCountChange;
    public Action<int> OnScoreChange;
    public Action OnGameEnd;
    
    public void BeginGame() {
      level.StandUp();
      level.State.Floor.Effector.drag = floorDrag;
      ResetGame();
    }
    private void SetPuckCount(int count) {
      if (count == state.PuckHandCount) {
        return;
      }
      
      state.PuckHandCount = count;
      OnPuckCountChange?.Invoke(count);
    }
    
    public void DestroyGame() {
      level.TearDown();
      state.Reset();
      state.PuckHandCount = 0;
    }

    public void ResetGame() {
      state.Reset();
      state.PuckHandCount = settings.puckCount;
      SpawnPuck();
    }

    private bool SpawnPuck() {
      if (state.PuckHandCount == 0) {
        return false;
      }
      var puck = level.InstantiatePuck(level.State.SpawnPoint);
      state.PuckList.Add(puck);
      puck.ScoreCollider.gameObject.layer = LayerMask.NameToLayer(scoreLayerName);
      puck.IsFlickable = true;
      SetPuckCount(state.PuckHandCount - 1);
      return true;
    }
    
    public Task<bool> Flick(Puck puck, Vector2 scaledDirection, float percent) {
      if (!puck.IsFlickable) {
        return Task.FromResult(false);
      }
      var taskSource = new TaskCompletionSource<bool>(TaskCreationOptions.AttachedToParent);
      puck.Mono.StartCoroutine(RunFlickRoutine(taskSource, puck, scaledDirection, percent));
      return taskSource.Task;
    }
    private IEnumerator<YieldInstruction> RunFlickRoutine(TaskCompletionSource<bool> source, Puck puck, Vector2 scaledDirection, float percent) {
      puck.IsFlickable = false;
      puck.Body.AddForce(scaledDirection * (flickForce * percent), ForceMode2D.Impulse);
      while (!IsAllSettled()) {
        CalcScore();
        yield return UnityUtil.EndOfFrame;
      }
      
      if ( (puck.Body.position.y - puck.Size) < level.State.CrossPoint.y) {
        ResetToSettledPositions();
        CalcScore();
        puck.IsFlickable = true;
        puck.Body.transform.position = level.State.SpawnPoint;
        puck.Body.velocity = Vector2.zero;
        source.SetResult(false);  
      }
      else {
        RecordSettledPositions();
        CalcScore();
        source.SetResult(true);
        
        if (!SpawnPuck()) {
          OnGameEnd.Invoke();
        }
      }
    }

    private bool IsAllSettled() {
      foreach (var puck in state.PuckList) {
        if (puck.Body.velocity.magnitude > flickFollowThreshold) {
          return false;
        }
      }
      return true;
    }

    private void RecordSettledPositions() {
      foreach (var puck in state.PuckList) {
        state.puckSettledPositions[puck.ID] = puck.Body.position;
        puck.Body.velocity = Vector2.zero;
      }
    }
    
    private void ResetToSettledPositions() {
      foreach (var puck in state.PuckList) {
        if (!state.puckSettledPositions.TryGetValue(puck.ID, out var position)) {
          continue;
        }
        puck.Body.position = position;
        puck.Body.velocity = Vector2.zero;
      }
    }

    private readonly Collider2D[] resultsCache = new Collider2D[100];
    private void CalcScore() {
      var size = Physics2D.OverlapCircleNonAlloc(level.State.Target, level.State.TargetRadius, resultsCache, LayerMask.GetMask(scoreLayerName));
      if (state.Score == size) {
        return;
      }
      
      state.Score = size;
      OnScoreChange?.Invoke(state.Score);
    }

    
  }
}

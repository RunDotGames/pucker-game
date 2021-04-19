using System;
using Unity.Mathematics;
using UnityEngine;

namespace RDG.PuckGame.Scripts {

  public interface LevelState {
    Vector2 SpawnPoint { get; }
    Vector2 CrossPoint { get; }
    Vector2 Target { get; }
    
    float TargetRadius { get; }
    
    Rect Area { get; }
    LevelFloor Floor { get; }
    bool IsReady();
  }
  public class LevelStateMutable : LevelState{

    public LevelStateMutable() {
      Reset();
    }

    public void Reset() {
      Area = Rect.zero;
      SpawnPoint = Vector2.negativeInfinity;
      CrossPoint = Vector2.negativeInfinity;
      Target = Vector2.negativeInfinity;
      TargetRadius = 0.0f;
    }
    public Vector2 SpawnPoint { get; set; }
    public float SpawnSize { get; set; }
    public Vector2 CrossPoint { get; set; }
    public LevelFloor Floor { get; set; }
    public Vector2 Target { get; set; }
    public float TargetRadius { get; set; }
    public Rect Area { get; set; }
    
    public bool IsReady() {
      return SpawnPoint != Vector2.negativeInfinity &&
             CrossPoint != Vector2.negativeInfinity &&
             Target != Vector2.negativeInfinity &&
             Area != Rect.zero &&
             SpawnSize > 0.0f &&
             TargetRadius > 0.0f;
    }
  }
  [CreateAssetMenu(menuName = "RDG/PuckGame/Level")]
  public class GameLevelSo :ScriptableObject{
    
    [SerializeField] private GameObject puckPrefab;
    [SerializeField] private GameObject targetPrefab;
    [SerializeField] private GameObject wallPrefab;
    [SerializeField] private GameObject floorPrefab;
    [SerializeField] private GameObject crossBackgroundPrefab;

    private readonly LevelStateMutable state = new LevelStateMutable();
    private readonly LevelWall[] walls = new LevelWall[4];
    public LevelState State => state;

    public Puck InstantiatePuck(Vector2 position) {
      var puck = Instantiate(puckPrefab, new Vector3(position.x, position.y, 0.0f), quaternion.identity).GetComponent<PuckBeh>().Puck;
      puck.Resize(state.SpawnSize);
      return puck;
    }

    public void StandUp() {
      if (!state.IsReady()) {
        throw new Exception("Attempted to init before ready");
      }
      var area = state.Area;
      var halfSize = area.size * 0.5f;
      state.Floor = Instantiate(floorPrefab).GetComponent<LevelFloorBeh>().LevelFloor;
      state.Floor.Collider.size = area.size;
      state.Floor = state.Floor;
      state.Floor.Sprite.size = area.size;
      walls[0] = InstanceWall(area.width, new Vector2(area.x, area.y + halfSize.y), Vector3.right);
      walls[1] = InstanceWall(area.width, new Vector2(area.x, area.y - halfSize.y),Vector3.right);
      walls[2] = InstanceWall(area.height, new Vector2(area.x + halfSize.x, area.y), Vector3.up);
      walls[3] = InstanceWall(area.height, new Vector2(area.x - halfSize.x, area.y), Vector3.up);

      var crossBkg = Instantiate(crossBackgroundPrefab);
      var min = area.position - (area.size * 0.5f);
      crossBkg.transform.position = new Vector3(area.position.x, (min.y + state.CrossPoint.y) / 2.0f, 0.0f);
      var crossBkgSprite = crossBkg.GetComponent<SpriteRenderer>();
      crossBkgSprite.size = new Vector2(area.width, state.CrossPoint.y - min.y);

      var target = Instantiate(targetPrefab);
      target.transform.position = state.Target;
      var targetSprite = target.GetComponent<SpriteRenderer>();
      targetSprite.size = Vector2.one * (state.TargetRadius * 2.0f);
      
      if (Camera.main == null) {
        throw new Exception("no camera in scene!");
      }
      var cameraTransform = Camera.main.transform;
      cameraTransform.position = (Vector3)area.position + (Vector3.forward * cameraTransform.position.z);
      Camera.main.orthographicSize = Mathf.Max(area.size.x, area.size.y) * 0.5f;
    }

    
    private LevelWall InstanceWall(float length,Vector2 position, Vector3 up) {
      var wall = Instantiate(wallPrefab).GetComponent<LevelWallBeh>().LevelWall;
      wall.Root.position = position;
      wall.Root.up = up;
      wall.Collider.size = new Vector2(wall.Collider.size.x, length);
      wall.Sprite.size = new Vector2(wall.Sprite.size.x, length);
      return wall;
    }

    public void TearDown() {
      state.Reset();
      if (state.Floor != null) {
        Destroy(state.Floor.Root.gameObject);
        state.Floor = null;  
      }
      
      for (var i = 0; i < walls.Length; i++) {
        if (walls[i] == null){
          continue;
        }
        Destroy(walls[i].Root);
        walls[i] = null;
      }
    }

    public void SetSpawnPoint(Vector2 aSpawnPoint, float size) {
      state.SpawnPoint = aSpawnPoint;
      state.SpawnSize = size;
    }

    public void SetCrossPoint(Vector2 aCrossPoint) {
      state.CrossPoint = aCrossPoint;
    }
    
    public void SetArea(Rect area) {
      state.Area = area;
    }

    public void SetTarget(Vector2 aTarget, float radius) {
      state.Target = aTarget;
      state.TargetRadius = radius;
    }

    
    
  }
}

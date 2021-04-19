using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RDG.PuckGame.Scripts {
  
  public class LevelFloor {
    public AreaEffector2D Effector;
    public BoxCollider2D Collider;
    public SpriteRenderer Sprite;
    public Transform Root;
  }
  
  [RequireComponent(typeof(AreaEffector2D))]
  [RequireComponent(typeof(BoxCollider2D))]
  [RequireComponent(typeof(SpriteRenderer))]
  public class LevelFloorBeh : MonoBehaviour {
    
    private readonly Lazy<LevelFloor> levelFloor;
    
    public LevelFloor LevelFloor => levelFloor.Value;

    public LevelFloorBeh() {
      levelFloor = new Lazy<LevelFloor>(() => new LevelFloor{
        Effector = GetComponent<AreaEffector2D>(),
        Collider = GetComponent<BoxCollider2D>(),
        Sprite = GetComponent<SpriteRenderer>(),
        Root = transform,
      });
    }
    
    
  }
}

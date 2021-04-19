using System;
using UnityEngine;

namespace RDG.PuckGame.Scripts {

  public class LevelWall {
    public BoxCollider2D Collider;
    public SpriteRenderer Sprite;
    public Transform Root;
  }
  
  [RequireComponent(typeof(Collider2D))]
  [RequireComponent(typeof(SpriteRenderer))]
  public class LevelWallBeh : MonoBehaviour {
    
    private Lazy<LevelWall> levelWall;

    public LevelWall LevelWall => levelWall.Value;

    public LevelWallBeh() {
      levelWall = new Lazy<LevelWall>(() => new LevelWall{
          Collider = GetComponent<BoxCollider2D>(),
          Root = transform,
          Sprite = GetComponent<SpriteRenderer>(),
        }
      );
    }
  }
}

using System;
using RDG.PuckGame.Scripts;
using UnityEngine;

public class Puck {
  public Guid ID;
  public Rigidbody2D Body;
  public CircleCollider2D InputCollider;
  public CircleCollider2D ScoreCollider;
  public CircleCollider2D PhysicsCollider;
  
  public SpriteRenderer Sprite;
  public bool IsFlickable;
  public MonoBehaviour Mono;
  public float Size;

  public void Resize(float size) {
    Size = size;
    InputCollider.radius = size;
    ScoreCollider.radius = size;
    PhysicsCollider.radius = size;
    Sprite.size = Vector2.one * size;
  }
}

[RequireComponent(typeof(Rigidbody2D))]
public class PuckBeh : MonoBehaviour {

  [SerializeField] private CircleCollider2D inputCollider;
  [SerializeField] private CircleCollider2D scoreCollider;
  [SerializeField] private CircleCollider2D physicsCollider;
  [SerializeField] private SpriteRenderer Sprite;
  
  public Puck Puck => puck.Value;
  
  private readonly Lazy<Puck> puck;

  public PuckBeh() {
    puck = new Lazy<Puck>(() =>
      new Puck(){
        Body = GetComponent<Rigidbody2D>(),
        InputCollider = inputCollider,
        ScoreCollider = scoreCollider,
        PhysicsCollider = physicsCollider,
        Sprite = Sprite,
        ID = Guid.NewGuid(),
        Mono =  this
      });
  }

}

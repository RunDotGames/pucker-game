

using System;
using RDG.PuckGame.Scripts;
using UnityEngine;


[Serializable]
public class FlickIndicationConfig {
  public float max;
  public float min;
  public float dirOffset;
  public float holdOffset;
}

public class FlickResult {
  public Vector2 Direction;
  public float Percent;
}

public class PuckUIDragIndicator {
  public SpriteRenderer DirIndicator;
  public SpriteRenderer HoldIndicator;
  public Transform Root;
  
  private FlickIndicationConfig config;
  private float stretch;
  private Vector2 direction;

  public void Init(FlickIndicationConfig aConfig) {
    config = aConfig;
  }
  
  public void Update(Vector2 stretchTo) {
    if (config == null) {
      return;
    }
    
    var from = (Vector2)Root.position;
    stretch = (from - stretchTo).magnitude;
    stretch = Mathf.Clamp(stretch, config.min, config.max);
    direction = stretchTo - from;
    var inverseDirection = direction * -1.0f;
    HoldIndicator.size = (HoldIndicator.size.x * Vector2.right) + (Vector2.up * stretch);
    var holdTransform = HoldIndicator.transform;
    var dirTransform = DirIndicator.transform;
    holdTransform.up = inverseDirection;
    holdTransform.localPosition = direction.normalized * config.holdOffset;
    dirTransform.localPosition = inverseDirection.normalized * config.dirOffset;
    dirTransform.up = inverseDirection;
  }

  public FlickResult Stop() {
    UnityEngine.Object.Destroy(Root.gameObject);
    var delta = config.max - config.min;
    var percent = (stretch - config.min) / delta;
    return new FlickResult(){
      Direction = direction.normalized * -1.0f,
      Percent = percent
    };
  }
  
}
public class PuckUIDragIndicatorBeh : MonoBehaviour {
  [SerializeField] private SpriteRenderer dirIndicator;
  [SerializeField] private SpriteRenderer holdIndicator;


  public PuckUIDragIndicator Indicator => indicator.Value;

  private readonly Lazy<PuckUIDragIndicator> indicator;

  public PuckUIDragIndicatorBeh() {
    indicator = new Lazy<PuckUIDragIndicator>(() => new PuckUIDragIndicator{
      DirIndicator = dirIndicator,
      HoldIndicator = holdIndicator,
      Root =  transform,
    });
  }
}
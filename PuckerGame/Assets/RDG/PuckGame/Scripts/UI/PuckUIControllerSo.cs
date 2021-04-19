using System;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace RDG.PuckGame.Scripts {
  
  [CreateAssetMenu(menuName = "RDG/PuckGame/UIController")]
  public class PuckUIControllerSo : ScriptableObject {

    [SerializeField] private GameObject flickIndicatorPrefab;
    [SerializeField] private FlickIndicationConfig flickIndicationConfig;
    
    public PuckUIDragIndicator BeginFlickIndication(Vector2 from) {
      var indicator = Instantiate(flickIndicatorPrefab).GetComponent<PuckUIDragIndicatorBeh>().Indicator;
      indicator.Root.position = from;
      indicator.Init(flickIndicationConfig);
      return indicator;
    }

    
  }
}

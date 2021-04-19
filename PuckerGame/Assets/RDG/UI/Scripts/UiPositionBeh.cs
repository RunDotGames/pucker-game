using System.Collections.Generic;
using UnityEngine;

namespace Scripts.RDG.UI {
    public class UiPositionBeh: MonoBehaviour {

        public Vector2 offset;
        
        public void RePosition() {
            GetComponent<RectTransform>().anchoredPosition = offset;
        }
    }
}

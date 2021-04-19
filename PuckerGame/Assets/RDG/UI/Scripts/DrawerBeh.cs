using System.Collections.Generic;
using System.Linq;
using RDG;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.RDG.UI {
    public class DrawerBeh : MonoBehaviour, UIInitializable {
        public LayoutElement toggleLayout;
        public UiThemeButtonBeh toggle;
        public bool isCollapsed;
        private RectTransform drawerRect;
        private UiSo ui;
        private bool isTweening;

        private static readonly Quaternion Upward = Quaternion.Euler(new Vector3(0, 0 , -90));
        private static readonly Quaternion Downward = Quaternion.Euler(new Vector3(0, 0 , 90));

        private float targetHeight;
        private float toggleHeight;
        
        public IEnumerable<GameObject> Initialize(UiSo aUi, UiTheme theme) {
            ui = aUi;
            drawerRect = GetComponent<RectTransform>();
            toggleHeight = toggleLayout.preferredHeight;
            targetHeight = drawerRect.rect.height - toggleHeight;
            UpdateHeight(1.0f);
            return UnityUtil.Once(gameObject);
        }

        public void Start() {
            toggle.OnClick += HandleToggle;
        }

        public void OnDestroy() {
            toggle.OnClick -= HandleToggle;
        }

        private void HandleToggle() {
            if (isTweening) {
                return;
            }
            isCollapsed = !isCollapsed;
            isTweening = true;
            StartCoroutine(RunToggle());
        }


        private void UpdateHeight(float percent) {
            var deltaHeight = percent * targetHeight;
            if (isCollapsed) {
                deltaHeight = targetHeight - deltaHeight;
            }
            drawerRect.sizeDelta = new Vector2(drawerRect.sizeDelta.x, deltaHeight + toggleHeight);
            // container.offsetMin = Vector2.up * (deltaHeight + toggleRect.rect.height);
            toggle.transform.rotation = Quaternion.Lerp(isCollapsed ? Upward : Downward, isCollapsed ? Downward : Upward, percent);
        }

        private IEnumerator<YieldInstruction> RunToggle() {
            var deltaTime = 0.0f;
            while (true) {
                deltaTime += Time.deltaTime;
                var percent = ui.drawerToggleCurve.Evaluate(deltaTime);
                UpdateHeight(percent);
                if (deltaTime > ui.drawerToggleCurve.keys.Last().time) {
                    isTweening = false;
                    break;
                }
                
                yield return UnityUtil.EndOfFrame;
            }
        }
        public GameObject InitRoot => gameObject;
    }
}

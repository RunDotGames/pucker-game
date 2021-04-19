using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RDG;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.RDG.UI {
    
    [RequireComponent(typeof(UiThemeShapeBeh))]
    [RequireComponent(typeof(UiMotionBeh))]
    public class UiModalBeh : MonoBehaviour, UIInitializable, UIPostInitializable, UIModal {

        public Action OnOpen;
        public Action<bool> OnClose;
        
        public Vector2 size;
        public Vector2 position;
        public bool isVisible;
        private UiSo ui;
        
        [HideInInspector, SerializeField] private UiMotionBeh openMotion;
        [HideInInspector, SerializeField] private UiThemeButtonBeh closeButton;
        [HideInInspector, SerializeField] private UiThemeShapeBeh windowShape;
        [HideInInspector, SerializeField] private VerticalLayoutGroup layoutGroup;
        [HideInInspector, SerializeField] private LayoutElement closeButtonRowLayout;
        [HideInInspector, SerializeField] private LayoutElement contentRowLayout;
        
        public IEnumerable<GameObject> Initialize(UiSo aUi, UiTheme theme) {
            ui = aUi;
            windowShape = UnityUtil.GetRequiredComp<UiThemeShapeBeh>(this);
            windowShape.hideFlags = ui.GetHideFlags();
            openMotion = UnityUtil.GetRequiredComp<UiMotionBeh>(this);
            openMotion.hideFlags = ui.GetHideFlags();
            openMotion.offset = Vector2.right * (size.x + position.x);
            var rectTransform = GetComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.one;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.pivot = Vector2.one;
            rectTransform.sizeDelta = size;
                
            
            if (UiThemeUtil.AddChild(ref layoutGroup, "Layout", transform, ui)) {
                layoutGroup.padding = new RectOffset(12, 12, 12, 12);
                layoutGroup.spacing = 6;    
            }
            layoutGroup.childForceExpandHeight = false;
            layoutGroup.childForceExpandWidth = true;
            layoutGroup.childControlHeight = true;
            layoutGroup.childControlWidth = true;
            layoutGroup.gameObject.hideFlags = HideFlags.None;
            
            UiThemeUtil.AddChild(ref closeButtonRowLayout, "Modal Row", layoutGroup.transform, ui);
            closeButtonRowLayout.preferredWidth = 25;
            closeButtonRowLayout.preferredHeight = 25;
            closeButtonRowLayout.minHeight = 25;
            closeButtonRowLayout.minWidth = 25;
            closeButtonRowLayout.flexibleWidth = 1.0f;
            closeButtonRowLayout.flexibleHeight = 0;
            closeButtonRowLayout.gameObject.hideFlags = HideFlags.None;
            
            UiThemeUtil.AddChild(ref contentRowLayout, "Content Row", layoutGroup.transform, ui);
            contentRowLayout.flexibleWidth = 1.0f;
            contentRowLayout.flexibleHeight = 1.0f;
            contentRowLayout.gameObject.hideFlags = HideFlags.None;
            
            closeButtonRowLayout.transform.SetSiblingIndex(0);
            contentRowLayout.transform.SetSiblingIndex(1);
            
            UiThemeUtil.AddChild(ref closeButton, "Close Button", closeButtonRowLayout.transform, ui);
            closeButton.shapeType = UiThemeShapeType.Circle;
            closeButton.colorType = UIThemeColorType.Secondary;
            closeButton.text = "x";
            closeButton.Initialize(ui, theme);
            closeButton.gameObject.hideFlags = ui.GetHideFlags();
            var closeTransform = closeButton.GetComponent<RectTransform>();
            closeTransform.anchorMax = Vector2.one;
            closeTransform.anchorMin = Vector2.right;
            closeTransform.pivot = Vector2.right + Vector2.up * .5f;
            closeTransform.sizeDelta = Vector2.right * 25;



            return UnityUtil.Once(gameObject);
        }
        public IEnumerable<GameObject> PostInitialize(UiSo ui, UiTheme theme) {
            var rectTransform = GetComponent<RectTransform>();
            rectTransform.anchoredPosition = position;
            openMotion.EvalEndPos();
            gameObject.SetActive(isVisible);

            closeButton.OnClick = HandleCloseClick;
            
            return UnityUtil.Once(gameObject);
        }
        private void HandleCloseClick() {
            CloseModal(false);
        }

        public bool OpenModalAsSingle() {
            return ui.ShowModal(this);
        }
        
        public void OpenModal() {
            gameObject.SetActive(true);
            openMotion.PlayOut();
            isVisible = true;
            OnOpen?.Invoke();
        }
        public Task CloseModal(bool isGraceful) {
            if (!isVisible) {
                return Task.CompletedTask;
            }
            
            isVisible = false;
            OnClose?.Invoke(isGraceful);
            return openMotion.PlayIn().ContinueWith(result => {
                gameObject.SetActive(false);
            }, TaskContinuationOptions.ExecuteSynchronously);
        }
        
        public void AddCloseHandler(Action<bool> onClose) {
            OnClose += onClose;
        }
        public void RemoveCloseHandler(Action<bool> onClose) {
            OnClose -= onClose;
        }
        public void AddOpenHandler(Action onOpen) {
            OnOpen += onOpen;
        }
        public void RemoveOpenHandler(Action onOpen) {
            OnOpen -= onOpen;
        }
    }
}

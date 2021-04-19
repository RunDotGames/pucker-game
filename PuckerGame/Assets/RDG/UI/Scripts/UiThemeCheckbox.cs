using System.Collections.Generic;
using RDG;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Scripts.RDG.UI {
    public class UiThemeCheckbox : MonoBehaviour, UIInitializable, IPointerClickHandler {

        public bool isChecked;
        public string label;
        public bool isClickDisabled;
        public UIThemeColorType labelColor = UIThemeColorType.OnSurface;
        public UIThemeColorType boxColor = UIThemeColorType.Primary;

        public bool IsChecked => isChecked;
        
        [SerializeField, HideInInspector] private UiThemeTextBeh textBeh;
        [SerializeField, HideInInspector] private UiThemeShapeBeh boxShapeBeh;
        [SerializeField, HideInInspector] private UiThemeShapeRipple ripple;
        [SerializeField, HideInInspector] private UiThemeShapeBeh checkShapeBeh;

        private static void PositionBox(UiSo ui, RectTransform rect) {
            var halfUp = Vector2.up * 0.5f;
            rect.anchorMax = Vector2.right + halfUp;
            rect.anchorMin = Vector2.right + halfUp;
            rect.pivot = Vector2.right + halfUp;
            rect.sizeDelta = Vector2.one * ui.checkboxSize;
        }
        public IEnumerable<GameObject> Initialize(UiSo ui, UiTheme theme) {
            UiThemeUtil.AddChild(ref textBeh, "Label", transform, ui);
            if (UiThemeUtil.AddChild(ref boxShapeBeh, "Box", transform, ui)) {
                boxShapeBeh.gameObject.AddComponent<UiThemeShapeRipple>();
            }
            UiThemeUtil.AddChild(ref checkShapeBeh, "Check", boxShapeBeh.transform, ui);
            ripple = boxShapeBeh.gameObject.GetComponent<UiThemeShapeRipple>();
            textBeh.Initialize(ui, theme);
            boxShapeBeh.Initialize(ui, theme);
            checkShapeBeh.Initialize(ui, theme);
            ripple.Initialize(ui, theme);
            
            textBeh.SetAlignment(TextAnchor.MiddleRight);
            textBeh.SetColor(labelColor);
            textBeh.SetValue(label);
            textBeh.SetFontType(UIThemeFontType.Body);
            PositionBox(ui, boxShapeBeh.GetComponent<RectTransform>());
            boxShapeBeh.SetHasShadow(true);
            boxShapeBeh.SetColorType(boxColor);
            boxShapeBeh.SetShapeType(UiThemeShapeType.RoundedSquare);
            boxShapeBeh.SetIsOutline(false);
            checkShapeBeh.SetHasShadow(false);
            var checkRect = checkShapeBeh.GetComponent<RectTransform>();
            checkRect.offsetMax = Vector2.one * -2.0f;
            checkRect.offsetMin = Vector2.one * 2.0f;
            checkShapeBeh.SetShapeType(UiThemeShapeType.Check);
            checkShapeBeh.SetIsOutline(false);
            var textRect = textBeh.GetComponent<RectTransform>();
            textRect.offsetMax = Vector2.right * (-ui.checkboxSize - 5);
            boxShapeBeh.SetColorType(boxColor);
            boxShapeBeh.SetShapeType(UiThemeShapeType.RoundedSquare);
            
            SetChecked(isChecked);
            SetClickDisabled(isClickDisabled);
            
            return UnityUtil.Once(gameObject);
        }

        public void SetChecked(bool isNowChecked) {
            isChecked = isNowChecked;
            if (checkShapeBeh == null) {
                return;
            }
            checkShapeBeh.ShapeImage.gameObject.SetActive(isNowChecked);
        }

        public void SetClickDisabled(bool isNowClickDisabled) {
            isClickDisabled = isNowClickDisabled;
            ripple.SetDisabled(isClickDisabled);
            boxShapeBeh.SetHasShadow(!isClickDisabled);
            boxShapeBeh.SetIsOutline(isClickDisabled);
            checkShapeBeh.SetColorType(isNowClickDisabled ? boxColor : UiThemeUtil.ToOnColor[boxColor]);
        }
        public void OnPointerClick(PointerEventData eventData) {
            if (isClickDisabled) {
                return;
            }
            
            SetChecked(!isChecked);
        }
    }
}

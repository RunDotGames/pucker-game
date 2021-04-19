using System;
using System.Collections.Generic;
using RDG;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Scripts.RDG.UI {
    
    [RequireComponent(typeof(UiThemeShapeBeh))]
    [RequireComponent(typeof(UiThemeShapeRipple))]
    public class UiThemeButtonBeh : MonoBehaviour, UIInitializable, IPointerClickHandler {

        public string text;
        public bool isClickDisabled;
        public UIThemeColorType colorType;
        public UiThemeShapeType shapeType;
        
        [SerializeField, HideInInspector]
        private UiThemeShapeBeh shape;

        [SerializeField, HideInInspector]
        private UiThemeTextBeh textBeh;

        [SerializeField, HideInInspector]
        private UiThemeShapeRipple shapeRipple;

        public Action OnClick;
        
        public IEnumerable<GameObject> Initialize(UiSo ui, UiTheme theme) {
            shape = UnityUtil.GetRequiredComp<UiThemeShapeBeh>(this);
            shape.hideFlags = ui.isVerboseDebug ? HideFlags.None :  HideFlags.HideInInspector;
            UiThemeUtil.AddChild(ref textBeh, "Text", transform, ui);
            shape.Initialize(ui, theme);
            textBeh.Initialize(ui, theme);
            textBeh.SetFontType(UIThemeFontType.Button);
            textBeh.SetAlignment(TextAnchor.MiddleCenter);
            textBeh.SetValue(text);
            shape.SetShapeType(shapeType);
            shapeRipple = UnityUtil.GetRequiredComp<UiThemeShapeRipple>(this);
            shapeRipple.Initialize(ui, theme);
            shapeRipple.SetDisabled(isClickDisabled);
            shapeRipple.hideFlags = ui.isVerboseDebug ? HideFlags.None :  HideFlags.HideInInspector;
            shape.hideFlags = ui.isVerboseDebug ? HideFlags.None :  HideFlags.HideInInspector;
            
            SetClickDisabled(isClickDisabled);
            return UnityUtil.Once(gameObject);
        }
     
        public void OnPointerClick(PointerEventData eventData) {
            if (isClickDisabled) {
                return;
            }
            OnClick?.Invoke();
        }

        public void SetClickDisabled(bool disabled) {
            isClickDisabled = disabled;
            shape.SetIsOutline(disabled);
            var color = isClickDisabled ?
                colorType :
                UiThemeUtil.ToOnColor[colorType];
            textBeh.SetColor(color);
            shape.SetHasShadow(!disabled);
            shape.SetColorType(colorType);
            shapeRipple.SetDisabled(disabled);
        }
    }
}

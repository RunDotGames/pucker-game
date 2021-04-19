using System;
using System.Collections.Generic;
using System.Threading;
using RDG;
using UnityEngine;
using UnityEngine.UI;


namespace Scripts.RDG.UI {
    
    [RequireComponent(typeof(Text))]
    public class UiThemeTextBeh : MonoBehaviour, UIInitializable {

        public UIThemeColorType colorType = UIThemeColorType.OnSurface;
        public UIThemeFontType fontType = UIThemeFontType.Body;
        public TextAnchor alignment;
        public string value = "";
        private UiTheme theme;

        [SerializeField, HideInInspector] private Text text;

        public IEnumerable<GameObject> Initialize(UiSo ui, UiTheme aTheme) {
            theme = aTheme;
            text = UnityUtil.GetRequiredComp<Text>(this);
            text.hideFlags = ui.isVerboseDebug ? HideFlags.None : HideFlags.HideInInspector;
            SetColor(colorType);
            SetFontType(fontType);
            SetAlignment(alignment);
            SetValue(value);
            text.horizontalOverflow = HorizontalWrapMode.Overflow;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            return UnityUtil.Once(gameObject);
        }
        public void SetColor(UIThemeColorType color) {
            colorType = color;
            text.color = theme.GetColor(color).color;
        }

        public void SetAlignment(TextAnchor aAlignment) {
            alignment = aAlignment;
            text.alignment = aAlignment;
            text.alignByGeometry = true;
            text.resizeTextForBestFit = false;
        }

        public void SetFontType(UIThemeFontType aFontType) {
            fontType = aFontType;
            var themeFont = theme.GetFont(fontType);
            text.font = themeFont.font;
            text.lineSpacing = themeFont.spacing;
            text.fontSize = themeFont.size;
            text.text = themeFont.isCaps ? value.ToUpper() : value;
        }

        public void SetValue(string textValue) {
            value = textValue;
            var themeFont = theme.GetFont(fontType);
            text.text = themeFont.isCaps ? textValue.ToUpper() : textValue;
        }
        
        public GameObject InitRoot => gameObject;
    }
}

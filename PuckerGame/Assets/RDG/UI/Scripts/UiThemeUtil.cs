using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.RDG.UI {
    public static class UiThemeUtil {
        public static bool AddChild<T>(ref T child, string name, Transform parent, UiSo ui) where T : MonoBehaviour{
            var isNew = false;
            if (child == null) {
                isNew = true;
                var obj = new GameObject(name, typeof(RectTransform), typeof(T));
                obj.transform.SetParent(parent);
                child = obj.GetComponent<T>();
            }
            var rectTransform = child.gameObject.GetComponent<RectTransform>();
            rectTransform.anchorMax = Vector2.one;
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.localScale = Vector3.one;
            rectTransform.localPosition = Vector3.zero;
            rectTransform.sizeDelta = Vector2.zero;
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.gameObject.hideFlags = ui.isVerboseDebug ? HideFlags.None : HideFlags.HideInHierarchy;
            return isNew;
        }
        
        public static readonly IReadOnlyDictionary<UIThemeColorType, UIThemeColorType> ToLightColor =
        new Dictionary<UIThemeColorType, UIThemeColorType>{
            { UIThemeColorType.Surface, UIThemeColorType.LightSurface },
            { UIThemeColorType.OnSurface, UIThemeColorType.LightSurface },
            { UIThemeColorType.LightSurface, UIThemeColorType.LightSurface },
            { UIThemeColorType.DarkSurface, UIThemeColorType.LightSurface },
            
            { UIThemeColorType.Primary, UIThemeColorType.LightPrimary },
            { UIThemeColorType.OnPrimary, UIThemeColorType.LightPrimary },
            { UIThemeColorType.LightPrimary, UIThemeColorType.LightSurface },
            { UIThemeColorType.DarkPrimary, UIThemeColorType.LightPrimary },
            
            { UIThemeColorType.Secondary, UIThemeColorType.LightSecondary },
            { UIThemeColorType.OnSecondary, UIThemeColorType.LightSecondary },
            { UIThemeColorType.LightSecondary, UIThemeColorType.LightSecondary },
            { UIThemeColorType.DarkSecondary, UIThemeColorType.LightSecondary },
        };
    
    public static readonly IReadOnlyDictionary<UIThemeColorType, UIThemeColorType> ToDarkColor =
        new Dictionary<UIThemeColorType, UIThemeColorType>{
            { UIThemeColorType.Surface, UIThemeColorType.DarkSurface },
            { UIThemeColorType.OnSurface, UIThemeColorType.DarkSurface },
            { UIThemeColorType.LightSurface, UIThemeColorType.DarkSurface },
            { UIThemeColorType.DarkSurface, UIThemeColorType.DarkSurface },
            
            { UIThemeColorType.Primary, UIThemeColorType.DarkPrimary },
            { UIThemeColorType.OnPrimary, UIThemeColorType.DarkPrimary },
            { UIThemeColorType.LightPrimary, UIThemeColorType.DarkPrimary },
            { UIThemeColorType.DarkPrimary, UIThemeColorType.DarkPrimary },
            
            { UIThemeColorType.Secondary, UIThemeColorType.DarkSecondary },
            { UIThemeColorType.OnSecondary, UIThemeColorType.DarkSecondary },
            { UIThemeColorType.LightSecondary, UIThemeColorType.DarkSecondary },
            { UIThemeColorType.DarkSecondary, UIThemeColorType.DarkSecondary },
        };
    
    public static readonly IReadOnlyDictionary<UIThemeColorType, UIThemeColorType> ToNormalColor =
        new Dictionary<UIThemeColorType, UIThemeColorType>{
            { UIThemeColorType.Surface, UIThemeColorType.Surface },
            { UIThemeColorType.OnSurface, UIThemeColorType.Surface },
            { UIThemeColorType.LightSurface, UIThemeColorType.Surface },
            { UIThemeColorType.DarkSurface, UIThemeColorType.Surface },
            
            { UIThemeColorType.Primary, UIThemeColorType.Primary },
            { UIThemeColorType.OnPrimary, UIThemeColorType.Primary },
            { UIThemeColorType.LightPrimary, UIThemeColorType.Primary },
            { UIThemeColorType.DarkPrimary, UIThemeColorType.Primary },
            
            { UIThemeColorType.Secondary, UIThemeColorType.Secondary },
            { UIThemeColorType.OnSecondary, UIThemeColorType.Secondary },
            { UIThemeColorType.LightSecondary, UIThemeColorType.Secondary },
            { UIThemeColorType.DarkSecondary, UIThemeColorType.Secondary },
        };
    
    public static readonly IReadOnlyDictionary<UIThemeColorType, UIThemeColorType> ToOnColor =
        new Dictionary<UIThemeColorType, UIThemeColorType>{
            { UIThemeColorType.Surface, UIThemeColorType.OnSurface },
            { UIThemeColorType.OnSurface, UIThemeColorType.OnSurface },
            { UIThemeColorType.LightSurface, UIThemeColorType.OnSurface },
            { UIThemeColorType.DarkSurface, UIThemeColorType.OnSurface },
            
            { UIThemeColorType.Primary, UIThemeColorType.OnPrimary },
            { UIThemeColorType.OnPrimary, UIThemeColorType.OnPrimary },
            { UIThemeColorType.LightPrimary, UIThemeColorType.OnPrimary },
            { UIThemeColorType.DarkPrimary, UIThemeColorType.OnPrimary },
            
            { UIThemeColorType.Secondary, UIThemeColorType.OnSecondary },
            { UIThemeColorType.OnSecondary, UIThemeColorType.OnSecondary },
            { UIThemeColorType.LightSecondary, UIThemeColorType.OnSecondary },
            { UIThemeColorType.DarkSecondary, UIThemeColorType.OnSecondary },
        };
    }
    
}

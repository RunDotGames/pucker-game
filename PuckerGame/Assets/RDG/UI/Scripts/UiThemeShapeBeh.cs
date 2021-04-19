using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Scripts.RDG.UI {

    public class UiThemeShapeBeh : MonoBehaviour, UIInitializable {
        
        public UiThemeShapeType shapeType = UiThemeShapeType.Square;
        public UIThemeColorType colorType = UIThemeColorType.Surface;
        public bool hasShadow;
        public bool isOutline;
        
        [HideInInspector, SerializeField]
        private Image shapeImage;
        [HideInInspector, SerializeField]
        private Image shadowImage;

        public Image ShapeImage => shapeImage;
        private UiTheme theme;
        private UiSo ui;

        public IEnumerable<GameObject> Initialize(UiSo aUi, UiTheme aTheme) {
            ui = aUi;
            theme = aTheme;
            var initObjs = new List<GameObject>{
                gameObject
            };
            SetHasShadow(hasShadow);
            if (UiThemeUtil.AddChild(ref shapeImage, "Shape", transform, ui)) {
                shapeImage.gameObject.AddComponent<Mask>();
            }
            SetShapeType(shapeType);
            SetColorType(colorType);
            initObjs.Add(shapeImage.gameObject);
            var mask = shapeImage.gameObject.GetComponent<Mask>();
            mask.showMaskGraphic = true;
            shapeImage.raycastTarget = true;
            return initObjs;
        }

        public void SetIsOutline(bool isNowOutline) {
            isOutline = isNowOutline;
            var shape = theme.GetShape(shapeType);
            shapeImage.sprite = isOutline ? shape.outline : shape.sprite;
            
        }
        
        public void SetHasShadow(bool nowHasShadow) {
            hasShadow = nowHasShadow;
            var shape = theme.GetShape(shapeType);
            if (!hasShadow && shadowImage != null) {
                DestroyImmediate(shadowImage.gameObject);
                shadowImage = null;
            }

            if (hasShadow) {
                UiThemeUtil.AddChild(ref shadowImage, "Shadow", transform, ui);
                var shadowTransform = shadowImage.gameObject.GetComponent<RectTransform>();
                shadowTransform.sizeDelta = Vector2.one * (shape.shadowSize * 2.0f);
                shadowTransform.anchoredPosition = Vector2.zero;
                shadowImage.sprite = shape.shadow;
                shadowImage.type = shape.imageType;
                shadowImage.preserveAspect = shape.preserveAspect;
                shadowImage.raycastTarget = false;
                shadowTransform.SetSiblingIndex(0);
            }
        }
        public void SetColorType(UIThemeColorType aColorType) {
            colorType = aColorType;
            shapeImage.color = theme.GetColor(colorType).color;
        }
        public void SetShapeType(UiThemeShapeType aShapeType) {
            shapeType = aShapeType;
            SetHasShadow(hasShadow); //update shadow shape
            var shape = theme.GetShape(shapeType);
            shapeImage.sprite = isOutline ? shape.outline : shape.sprite;
            shapeImage.preserveAspect = shape.preserveAspect;
            shapeImage.type = shape.imageType;
            shapeImage.transform.SetSiblingIndex(hasShadow ? 1 : 0);
            
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using RDG;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Scripts.RDG.UI {
    
    [RequireComponent(typeof(UiThemeShapeBeh))]
    public class UiThemeShapeRipple : MonoBehaviour, UIInitializable, IPointerDownHandler, IPointerUpHandler {

        
        public bool isClickDisabled;
        
        private UiThemeShapeBeh shape;
        private Image ripple;
        private UiTheme uiTheme;
        private UiSo uiSo;
        private bool fadeOut;
        private Coroutine rippleRoutine;

        public IEnumerable<GameObject> Initialize(UiSo ui, UiTheme theme) {
            uiTheme = theme;
            uiSo = ui;
            shape = UnityUtil.GetRequiredComp<UiThemeShapeBeh>(this);
            SetDisabled(isClickDisabled);
            return UnityUtil.Once(gameObject);
        }
        
        public void OnEnable() {
            if (ripple == null) {
                return;
            }
            ripple.gameObject.SetActive(false);
        }

        private IEnumerator<YieldInstruction> OnRipple(float targetSize) {
            var deltaTime = 0.0f;
            while (true) {
                deltaTime += Time.deltaTime;
                var percent = uiSo.buttonRippleSizeCurve.Evaluate(deltaTime);
                ripple.rectTransform.sizeDelta = Vector2.one * (targetSize * percent);
                if (deltaTime > uiSo.buttonRippleSizeCurve.keys.Last().time) {
                    break;
                }
                
                yield return UnityUtil.EndOfFrame;
            }
            while (!fadeOut) {
                yield return UnityUtil.EndOfFrame;
            }
            deltaTime = 0.0f;
            while (true) {
                deltaTime += Time.deltaTime;
                var percent = uiSo.buttonRippleFadeCurve.Evaluate(deltaTime);
                
                var color = ripple.color;
                color.a = percent;
                ripple.color = color;
                if (deltaTime > uiSo.buttonRippleFadeCurve.keys.Last().time) {
                    break;
                }
                yield return UnityUtil.EndOfFrame;
            }
            ripple.gameObject.SetActive(false);
        }
        
        public void OnPointerDown(PointerEventData eventData) {
            if (isClickDisabled) {
                return;
            }
            fadeOut = false;
            var shapeImage = shape.ShapeImage;
            UiThemeUtil.AddChild(ref ripple, "Ripple", shapeImage.transform, uiSo);
            ripple.transform.SetSiblingIndex(0);
            ripple.gameObject.SetActive(true);
            var rippleRect = ripple.rectTransform;
            var shapeRect = shapeImage.rectTransform;
            ripple.sprite = uiTheme.GetShape(UiThemeShapeType.Circle).sprite;
            ripple.preserveAspect = true;
            ripple.type = Image.Type.Simple;
            ripple.color = uiTheme.GetColor(UiThemeUtil.ToLightColor[shape.colorType]).color;
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                shapeImage.rectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out var localPoint
            )) {
                return;
            }
            rippleRect.anchorMax = Vector2.one  * .5f;
            rippleRect.anchorMin = Vector2.one  * .5f;
            rippleRect.anchoredPosition = localPoint;
            var shapeSize = shapeRect.rect.size;
            var shapeMid = shapeSize * 0.5f;
            var size = Mathf.Max(
                shapeMid.x + Mathf.Abs(localPoint.x),
                shapeMid.y + Mathf.Abs(localPoint.y)
            ) * 2.0f;
            
            if (rippleRoutine != null) {
                StopCoroutine(rippleRoutine);
            }
            rippleRoutine = StartCoroutine(OnRipple(size));
        }
        public void OnPointerUp(PointerEventData eventData) {
            fadeOut = true;
        }

        public void SetDisabled(bool disabled) {
            isClickDisabled = disabled;
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RDG;
using UnityEngine;

namespace Scripts.RDG.UI {

    public class UiMotionBeh : MonoBehaviour, UIInitializable {

        public Vector2 offset;
        
        private UiSo uiSo;
        private Vector2 endPos;
        private RectTransform rectTransform;
        private Coroutine motionRoutine;
        private TaskCompletionSource<bool> taskSource;

        public IEnumerable<GameObject> Initialize(UiSo ui, UiTheme theme) {
            uiSo = ui;
            var obj = gameObject;
            rectTransform = obj.GetComponent<RectTransform>();
            EvalEndPos();
            return UnityUtil.Once(obj);
        }

        public void EvalEndPos() {
            endPos = rectTransform.anchoredPosition;
        }

        private void HaltMotion() {
            if (motionRoutine != null) {
                taskSource.SetCanceled();
                StopCoroutine(motionRoutine);
            }
        }

        public Task<bool> PlayIn() {
            HaltMotion();
            taskSource = new TaskCompletionSource<bool>();
            motionRoutine = StartCoroutine(RunMotion(true));
            return taskSource.Task;
        }

        public Task<bool> PlayOut() {
            HaltMotion();
            taskSource = new TaskCompletionSource<bool>();
            motionRoutine = StartCoroutine(RunMotion(false));
            return taskSource.Task;
        }
        private IEnumerator<YieldInstruction> RunMotion(bool isForward) {
            var deltaTime = 0.0f;
            while (true) {
                deltaTime += Time.deltaTime;
                var percent = uiSo.motionCurve.Evaluate(deltaTime);
                if (!isForward) {
                    percent = 1.0f - percent;
                }
                rectTransform.anchoredPosition = endPos + offset * percent;
                if (deltaTime > uiSo.motionCurve.keys.Last().time) {
                    motionRoutine = null;
                    taskSource.SetResult(true);
                    break;
                }
                yield return UnityUtil.EndOfFrame;
            }
        }
        
        public GameObject InitRoot => gameObject;
    }
}

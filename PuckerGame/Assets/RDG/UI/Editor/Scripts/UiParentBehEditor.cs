
    using System.Collections.Generic;
    using Scripts.RDG.UI;
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(UiParentBeh))]
    public class UiParentBehEditor : Editor {

        public override void OnInspectorGUI() {
            DrawDefaultInspector();
            if (!GUILayout.Button("Apply",  GUILayout.ExpandWidth(false))) {
                return;
            }
            var parent = (UiParentBeh)target;
            parent.Apply(HandleDirty, false);
            EditorUtility.SetDirty(parent.gameObject);
        }

        private void HandleDirty(IEnumerable<GameObject> objs) {
            foreach (var obj in objs) {
                EditorUtility.SetDirty(obj);    
            }
            
        }
    }


using System;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.RDG.UI {
    public class UiParentBeh : MonoBehaviour {
        public UiSo ui;
        public UiThemeSo themeSo;

        public void Start() {
            ui.ResetModals();
            Apply(objs => { }, true);
        }
        public void Apply(Action<IEnumerable<GameObject>> handleDirty, bool runPostInits) {
            var inits = GetComponentsInChildren<UIInitializable>();
            var theme = themeSo.NewTheme();
            foreach (var init in inits) {
                handleDirty(init.Initialize(ui, theme));
            }
            if (!runPostInits) {
                return;
            }
            var postInits = GetComponentsInChildren<UIPostInitializable>();
            foreach (var postInit in postInits) {
                handleDirty(postInit.PostInitialize(ui, theme));
            }
        }

    }
}

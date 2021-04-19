using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
public interface UIInitializable {
    IEnumerable<GameObject> Initialize(UiSo ui, UiTheme theme);
}

public interface UIPostInitializable {
    IEnumerable<GameObject> PostInitialize(UiSo ui, UiTheme theme);
}

public interface UIModal {
    void OpenModal();
    Task CloseModal(bool isGraceful);

    void AddCloseHandler(Action<bool> onClose);
    void RemoveCloseHandler(Action<bool> onClose);
    void AddOpenHandler(Action onOpen);
    void RemoveOpenHandler(Action onOpen);
}

[CreateAssetMenu(menuName = "RDG/UI/UI Global")]
public class UiSo : ScriptableObject {
    public bool isVerboseDebug;
    public AnimationCurve drawerToggleCurve;
    public AnimationCurve buttonRippleSizeCurve;
    public AnimationCurve buttonRippleFadeCurve;
    public AnimationCurve motionCurve;
    public float checkboxSize = 20;

    private UIModal visibleModal;
    private bool isModalInTransition;

    
    public HideFlags GetHideFlags() {
        return isVerboseDebug ? HideFlags.None :  HideFlags.HideInInspector;
    }

    public void ResetModals() {
        visibleModal = null;
        isModalInTransition = false;
    }

    public bool ShowModal(UIModal modal) {
        if (isModalInTransition) {
            return false;
        }
        
        if (visibleModal == null) {
            OpenAsVisibleModal(modal);
            return  true;
        }
        isModalInTransition = true;
        visibleModal.RemoveCloseHandler(HandleVisibleClose);
        visibleModal.CloseModal(false).ContinueWith((task) => {
            OpenAsVisibleModal(modal);
            isModalInTransition = false;
        }, TaskContinuationOptions.ExecuteSynchronously);
        return true;
    }

    private void OpenAsVisibleModal(UIModal modal) {
        visibleModal = modal;
        visibleModal.AddCloseHandler(HandleVisibleClose);
        modal.OpenModal();
    }

    private void HandleVisibleClose(bool isGraceful) {
        visibleModal.RemoveCloseHandler(HandleVisibleClose);
        visibleModal = null;
    }
}


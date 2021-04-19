using System;
using System.Collections.Generic;
using UnityEngine;

public enum KeyAction {
    MoveLeft=0, MoveRight=1, MoveForward=2, MoveBack=3, Interact=4, Modify=5,
}

[Serializable]
public class KeyActionBinding {
    public KeyAction action;
    public KeyCode key;
  
    // Index < 0 indicates binding is keyCode instead
    public int mouseButton = -1;
}

[CreateAssetMenu(menuName = "RDG/Input/Key Bindings")]
public class KeyActionBindingsSo: ScriptableObject {
    public List<KeyActionBinding> bindings;

    public Action<KeyAction> OnDown;
    public Action<KeyAction> OnUp;
    
}
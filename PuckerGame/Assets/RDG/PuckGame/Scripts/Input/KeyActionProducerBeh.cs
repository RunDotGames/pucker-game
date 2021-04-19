using UnityEngine;
using System;
using System.Collections.Generic;



public class KeyActionProducerBeh : MonoBehaviour {

  public KeyActionBindingsSo keyBindings;
  private Dictionary<KeyAction, KeyActionBinding> actionMap;

  public void Start(){
    actionMap = new Dictionary<KeyAction, KeyActionBinding>();
    foreach (var binding in keyBindings.bindings) {
        actionMap[binding.action] = binding;
    }
  }

  public void Update() {
    if (actionMap == null) {
      return;
    }
    
    foreach (var binding in actionMap.Values) {
      if (IsActionDown(binding.action)) {
        keyBindings.OnDown?.Invoke(binding.action);
      }

      if (IsActionUp(binding.action)) {
        keyBindings.OnUp?.Invoke(binding.action);
      }
    }
  }

  private bool IsActionDown(KeyAction action) {
    if (!actionMap.TryGetValue(action, out var binding)) {
      return false;
    }

    if (binding.key != KeyCode.None) {
      return Input.GetKeyDown(binding.key);
    }

    return binding.mouseButton > -1 && Input.GetMouseButtonDown(binding.mouseButton);
  }
  
  private bool IsActionUp(KeyAction action) {
    if (!actionMap.TryGetValue(action, out var binding)) {
      return false;
    }

    if (binding.key != KeyCode.None) {
      return Input.GetKeyUp(binding.key);
    }

    return binding.mouseButton > -1 && Input.GetMouseButtonUp(binding.mouseButton);
  }
}


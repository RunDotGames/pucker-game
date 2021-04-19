using System.Collections.Generic;
using System;
using System.Diagnostics;

public class InputPressEvent {
  public readonly KeyAction keyAction;
  public bool isPressed;
  public long at;

  public InputPressEvent(KeyAction keyAction) {
    this.keyAction = keyAction;
    at = long.MinValue;
  }
}

public class InputPressEventComparator : IComparer<InputPressEvent> {
    public int Compare(InputPressEvent x, InputPressEvent y) {
      var dif = (y?.at ?? 0) - (x?.at ?? 0);
      return dif == 0 ? 0 : Math.Sign(dif);
      
    }
}
public class InputStack {
  
  private static readonly InputPressEventComparator COMPARER = new InputPressEventComparator();
  private readonly List<InputPressEvent> events = new List<InputPressEvent>();
  private readonly Dictionary<KeyAction, InputPressEvent> eventMap = new Dictionary<KeyAction, InputPressEvent>();

  public InputStack(KeyActionBindingsSo bindings) {
    bindings.OnDown += HandleActionDown;
    bindings.OnUp += HandleActionUp;
    foreach (var value in Enum.GetValues(typeof(KeyAction))) {
      var asAction = (KeyAction)value;
      var keyEvent = new InputPressEvent(asAction);
      events.Add(keyEvent);
      eventMap[asAction] = keyEvent;
    }
  }

  private void HandleActionUp(KeyAction action) {
    eventMap[action].isPressed = false;
    events.Sort(COMPARER);
  }

  private void HandleActionDown(KeyAction action) {
    eventMap[action].isPressed = true;
    eventMap[action].at = Stopwatch.GetTimestamp();
    events.Sort(COMPARER);
  }

  public KeyAction GetMostRecentPress(List<KeyAction> keys, out bool isFound){
    isFound = false;
    foreach (var keyEvent in events) {
      if(keys.Contains(keyEvent.keyAction) && keyEvent.isPressed){
        isFound = true;
        return keyEvent.keyAction;
      }
    }
    return default;
  }

}
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RDG {
  public static class UnityUtil {
    public static readonly YieldInstruction EndOfFrame = new WaitForEndOfFrame();
    
    public static IEnumerable<T> Once<T>(T value) {
      yield return value;
    }
    
    public static TRequired GetRequiredComp<TRequired>(MonoBehaviour source) {
      var found = source.GetComponent<TRequired>();
      if (found == null) {
        throw new Exception($"{nameof(source)} requires same obj {nameof(TRequired)}");
      }
      return found;
    }
  }
}

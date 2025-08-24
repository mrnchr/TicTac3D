using System;
using System.Diagnostics;
using UnityEngine.Scripting;

namespace CollectiveMind.TicTac3D.Runtime.Utils
{
  [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
  [Conditional("UNITY_EDITOR")]
  public class ResourcePathAttribute : PreserveAttribute
  {
    public Type ResourceType;
  }
}
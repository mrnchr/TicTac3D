using UnityEngine;

namespace CollectiveMind.TicTac3D.Runtime.Shared.Utils
{
  public static class UnityExtensions
  {
    public static T ObjOrNull<T>(this T obj) where T : Object
    {
      return obj ? obj : null;
    }
  }
}
using UnityEngine;

namespace CollectiveMind.TicTac3D.Runtime.Utils
{
  public static class UnityExtensions
  {
    public static T OrNull<T>(this T obj) where T : Object
    {
      return obj ? obj : null;
    }
  }
}
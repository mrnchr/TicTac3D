using System.Collections.Generic;
using System.Linq;

namespace CollectiveMind.TicTac3D.Runtime.Server.Utils
{
  public static class Combinatorics
  {
    public static IEnumerable<IEnumerable<T>> GetCombinations<T>(IEnumerable<T> elements, int k)
    {
      return GetCombinationsRecursive(elements.ToList(), k, 0, new List<T>());
    }

    private static IEnumerable<IEnumerable<T>> GetCombinationsRecursive<T>(List<T> elements, int k, int start,
      List<T> current)
    {
      if (current.Count == k)
      {
        yield return new List<T>(current);
        yield break;
      }

      for (int i = start; i < elements.Count; i++)
      {
        current.Add(elements[i]);
        foreach (IEnumerable<T> combination in GetCombinationsRecursive(elements, k, i + 1, current))
        {
          yield return combination;
        }

        current.RemoveAt(current.Count - 1);
      }
    }
  }
}
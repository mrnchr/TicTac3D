using System;
using UnityEngine;

namespace CollectiveMind.TicTac3D.Runtime.Client.Input
{
  [Serializable]
  public class InputProvider
  {
    public bool Rotate;
    public Vector2 Delta;

    public void Reset()
    {
      Rotate = false;
      Delta = Vector2.zero;
    }
  }
}
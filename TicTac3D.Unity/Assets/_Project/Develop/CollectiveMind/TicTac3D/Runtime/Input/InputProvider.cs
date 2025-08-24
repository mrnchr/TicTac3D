using System;
using UnityEngine;

namespace CollectiveMind.TicTac3D.Runtime.Input
{
  [Serializable]
  public class InputProvider
  {
    public bool Click;
    public bool Rotate;
    public Vector2 Delta;
    public Vector2 MousePosition;

    public void Reset()
    {
      Click = false;
      Rotate = false;
      Delta = Vector2.zero;
      MousePosition = Vector2.zero;
    }
  }
}
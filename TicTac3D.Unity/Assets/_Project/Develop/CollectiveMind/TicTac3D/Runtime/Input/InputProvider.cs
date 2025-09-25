using System;
using UnityEngine;

namespace CollectiveMind.TicTac3D.Runtime.Input
{
  [Serializable]
  public class InputProvider
  {
    public bool Touch;
    public bool Click;
    public Vector2 PointerPosition;
    public bool Rotate;
    public Vector2 RotateValue;

    public void Reset()
    {
      Touch = false;
      Click = false;
      PointerPosition = Vector2.zero;
      Rotate = false;
      RotateValue = Vector2.zero;
    }
  }
}
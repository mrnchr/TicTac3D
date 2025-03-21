using System;
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay.Shape;
using R3;
using UnityEngine;

namespace CollectiveMind.TicTac3D.Runtime.Shared.Gameplay.Cell
{
  [Serializable]
  public class CellModel
  {
    public Vector3 Index;
    public Vector3 Position;

    public SerializableReactiveProperty<bool> IsHovered = new SerializableReactiveProperty<bool>();
    public SerializableReactiveProperty<ShapeType> Shape = new SerializableReactiveProperty<ShapeType>();

    public bool HasShape()
    {
      return Shape.Value != ShapeType.None;
    }
  }
}
using System;
using R3;
using UnityEngine;

namespace CollectiveMind.TicTac3D.Runtime.Gameplay
{
  [Serializable]
  public class CellModel
  {
    public Vector3 Index;
    public Vector3 Position;

    public SerializableReactiveProperty<bool> IsHovered = new SerializableReactiveProperty<bool>();
    public SerializableReactiveProperty<ShapeType> Shape = new SerializableReactiveProperty<ShapeType>();
    public ShapeType FadingContext;
    public SerializableReactiveProperty<int> LifeTime = new SerializableReactiveProperty<int>();
    
    public bool HasShape()
    {
      return Shape.Value != ShapeType.None;
    }
  }
}
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay.Cell;
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay.Shape;
using UnityEngine;

namespace CollectiveMind.TicTac3D.Runtime.Client.Gameplay.Shape
{
  public interface IShapeFactory
  {
    ShapeVisual Create(ShapeType id, Vector3 position, Transform parent, CellModel model);
  }
}
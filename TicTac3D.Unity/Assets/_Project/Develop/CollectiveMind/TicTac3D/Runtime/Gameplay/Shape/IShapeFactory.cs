using UnityEngine;

namespace CollectiveMind.TicTac3D.Runtime.Gameplay
{
  public interface IShapeFactory
  {
    ShapeVisual Create(ShapeType id, Vector3 position, Transform parent, CellModel model);
  }
}
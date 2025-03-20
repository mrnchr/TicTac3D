using UnityEngine;

namespace CollectiveMind.TicTac3D.Runtime.Shared.Gameplay.Cell
{
  public interface ICellModelFactory
  {
    CellModel Create(Vector3 index);
  }
}
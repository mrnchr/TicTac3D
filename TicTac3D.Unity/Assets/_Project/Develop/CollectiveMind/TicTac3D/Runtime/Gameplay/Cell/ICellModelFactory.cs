using UnityEngine;

namespace CollectiveMind.TicTac3D.Runtime.Gameplay
{
  public interface ICellModelFactory
  {
    CellModel Create(Vector3 index);
  }
}
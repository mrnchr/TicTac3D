using System.Collections.Generic;

namespace CollectiveMind.TicTac3D.Runtime.Shared.Gameplay.Cell
{
  public interface ICellCreator
  {
    void CreateCells(List<CellModel> cells);
  }
}
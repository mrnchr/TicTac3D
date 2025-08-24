using System.Collections.Generic;

namespace CollectiveMind.TicTac3D.Runtime.Gameplay
{
  public interface ICellCreator
  {
    void CreateCells(List<CellModel> cells);
  }
}
using System.Collections.Generic;
using UnityEngine;

namespace CollectiveMind.TicTac3D.Runtime.Gameplay
{
  public class CellCreator : ICellCreator
  {
    private readonly ICellModelFactory _cellModelFactory;

    public CellCreator(ICellModelFactory cellModelFactory)
    {
      _cellModelFactory = cellModelFactory;
    }

    public void CreateCells(List<CellModel> cells)
    {
      for (int i = 0; i < 3; i++)
      {
        for (int j = 0; j < 3; j++)
        {
          for (int k = 0; k < 3; k++)
          {
            Vector3 index = new Vector3(i, j, k) - Vector3.one;
            CellModel cell = _cellModelFactory.Create(index);
            cells.Add(cell);
          }
        }
      }
    }
  }
}
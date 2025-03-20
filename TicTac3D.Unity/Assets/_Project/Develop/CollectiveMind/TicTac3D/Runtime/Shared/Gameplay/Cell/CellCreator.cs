using System.Collections.Generic;
using UnityEngine;

namespace CollectiveMind.TicTac3D.Runtime.Shared.Gameplay.Cell
{
  public class CellCreator : ICellCreator
  {
    private readonly List<CellModel> _cells;
    private readonly ICellModelFactory _cellModelFactory;

    public CellCreator(List<CellModel> cells, ICellModelFactory cellModelFactory)
    {
      _cells = cells;
      _cellModelFactory = cellModelFactory;
    }

    public void CreateModels()
    {
      for (int i = 0; i < 3; i++)
      {
        for (int j = 0; j < 3; j++)
        {
          for (int k = 0; k < 3; k++)
          {
            Vector3 index = new Vector3(i, j, k) - Vector3.one;
            CellModel cell = _cellModelFactory.Create(index);
            _cells.Add(cell);
          }
        }
      }
    }
  }
}
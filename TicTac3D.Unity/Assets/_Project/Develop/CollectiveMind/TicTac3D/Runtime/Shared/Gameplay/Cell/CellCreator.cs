using System.Collections.Generic;
using CollectiveMind.TicTac3D.Runtime.Shared.Network;
using UnityEngine;

namespace CollectiveMind.TicTac3D.Runtime.Shared.Gameplay.Cell
{
  public class CellCreator : ICellCreator
  {
    private readonly List<CellModel> _cells;
    private readonly CellModelFactory _cellModelFactory;
    private readonly INetworkBus _networkBus;

    public CellCreator(List<CellModel> cells, CellModelFactory cellModelFactory)
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
            Vector3Int position = new Vector3Int(i, j, k) - Vector3Int.one;
            CellModel cell = _cellModelFactory.Create(position);
            cell.Value = i * 9 + j * 3 + k;
            _cells.Add(cell);
          }
        }
      }
    }
  }
}
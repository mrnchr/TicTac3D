using System;
using System.Collections.Generic;
using CollectiveMind.TicTac3D.Runtime.Client.Gameplay.Cell;
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay.Cell;
using Unity.Netcode;
using Object = UnityEngine.Object;

namespace CollectiveMind.TicTac3D.Runtime.Client.Gameplay
{
  public class FieldCleaner : IDisposable
  {
    private readonly NetworkManager _networkManager;
    private readonly List<CellModel> _cells;
    private readonly List<CellVisual> _cellVisuals;

    public FieldCleaner(NetworkManager networkManager, List<CellModel> cells, List<CellVisual> cellVisuals)
    {
      _networkManager = networkManager;
      _cells = cells;
      _cellVisuals = cellVisuals;

      _networkManager.OnClientStopped += CleanField;
    }

    private void CleanField(bool obj)
    {
      foreach (CellVisual cell in _cellVisuals)
        Object.Destroy(cell.gameObject);

      _cellVisuals.Clear();
      _cells.Clear();
    }

    public void Dispose()
    {
      _networkManager.OnClientStopped -= CleanField;
    }
  }
}
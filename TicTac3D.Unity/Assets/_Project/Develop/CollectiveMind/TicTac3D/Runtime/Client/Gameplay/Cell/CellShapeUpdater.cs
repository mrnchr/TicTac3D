using System;
using System.Collections.Generic;
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay;
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay.Cell;
using CollectiveMind.TicTac3D.Runtime.Shared.Network;

namespace CollectiveMind.TicTac3D.Runtime.Client.Gameplay.Cell
{
  public class CellShapeUpdater : IDisposable
  {
    private readonly INetworkBus _networkBus;
    private readonly List<CellModel> _cells;

    public CellShapeUpdater(INetworkBus networkBus, List<CellModel> cells)
    {
      _networkBus = networkBus;
      _cells = cells;
      
      _networkBus.SubscribeOnRpcWithParameter<UpdateLifeTimeResponse>(UpdateLifeTime);
      _networkBus.SubscribeOnRpcWithParameter<UpdateShapeResponse>(UpdateCell);
    }

    private void UpdateCell(UpdateShapeResponse response)
    {
      CellModel cell = _cells.Find(x => x.Index == response.CellIndex);
      cell.Shape.Value = response.Shape;
    }

    private void UpdateLifeTime(UpdateLifeTimeResponse response)
    {
      CellModel cell = _cells.Find(x => x.Index == response.CellIndex);
      cell.LifeTime.Value = response.LifeTime;
    }

    public void Dispose()
    {
      _networkBus.UnsubscribeFromRpc<UpdateShapeResponse>();
    }
  }
}
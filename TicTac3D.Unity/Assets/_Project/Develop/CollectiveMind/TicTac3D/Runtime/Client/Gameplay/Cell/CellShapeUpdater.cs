using System;
using System.Collections.Generic;
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
      
      _networkBus.SubscribeOnRpcWithParameter<UpdatedShapeResponse>(UpdateCell);
    }

    private void UpdateCell(UpdatedShapeResponse response)
    {
      CellModel cell = _cells.Find(x => x.Index == response.CellIndex);
      cell.Shape.Value = response.Shape;
    }

    public void Dispose()
    {
      _networkBus.UnsubscribeFromRpc<UpdatedShapeResponse>();
    }
  }
}
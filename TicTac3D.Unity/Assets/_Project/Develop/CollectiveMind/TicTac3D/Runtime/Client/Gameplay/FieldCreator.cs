using System;
using System.Collections.Generic;
using CollectiveMind.TicTac3D.Runtime.Client.Gameplay.Cell;
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay;
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay.Cell;
using CollectiveMind.TicTac3D.Runtime.Shared.Network;

namespace CollectiveMind.TicTac3D.Runtime.Client.Gameplay
{
  public class FieldCreator : IDisposable
  {
    private readonly INetworkBus _networkBus;
    private readonly ICellCreator _cellCreator;
    private readonly ICellVisualFactory _cellVisualFactory;
    private readonly List<CellModel> _cells;

    public FieldCreator(INetworkBus networkBus, ICellCreator cellCreator, ICellVisualFactory cellVisualFactory,
      List<CellModel> cells)
    {
      _networkBus = networkBus;
      _cellCreator = cellCreator;
      _cellVisualFactory = cellVisualFactory;
      _cells = cells;

      if (NetworkRole.IsClient)
        _networkBus.SubscribeOnRpc<GameStartedEvent>(CreateField);
    }

    private void CreateField()
    {
      _cellCreator.CreateModels();

      foreach (CellModel cell in _cells)
        _cellVisualFactory.Create(cell);
    }

    public void Dispose()
    {
      _networkBus.UnsubscribeFromRpc<GameStartedEvent>();
    }
  }
}
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
    private readonly GameInfo _gameInfo;

    public FieldCreator(INetworkBus networkBus,
      ICellCreator cellCreator,
      ICellVisualFactory cellVisualFactory,
      List<CellModel> cells,
      GameInfo gameInfo)
    {
      _networkBus = networkBus;
      _cellCreator = cellCreator;
      _cellVisualFactory = cellVisualFactory;
      _cells = cells;
      _gameInfo = gameInfo;

      if (NetworkRole.IsClient)
        _networkBus.SubscribeOnRpcWithParameter<StartedGameResponse>(CreateField);
      
      _networkBus.SubscribeOnRpcWithParameter<DefinedShapeResponse>(DefineShape);
    }

    private void CreateField(StartedGameResponse response)
    {
      _cellCreator.CreateModels();

      foreach (CellModel cell in _cells)
        _cellVisualFactory.Create(cell);
    }

    private void DefineShape(DefinedShapeResponse response)
    {
      _gameInfo.Shape = response.Shape;
    }

    public void Dispose()
    {
      _networkBus.UnsubscribeFromRpc<StartedGameResponse>();
      _networkBus.UnsubscribeFromRpc<DefinedShapeResponse>();
    }
  }
}
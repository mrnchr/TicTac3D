using System;
using System.Collections.Generic;
using CollectiveMind.TicTac3D.Runtime.Client.Gameplay.Cell;
using CollectiveMind.TicTac3D.Runtime.Client.GameStateComponents;
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
    private readonly List<CellVisual> _cellVisuals;
    private readonly IGameStateMachine _gameStateMachine;

    public FieldCreator(INetworkBus networkBus,
      ICellCreator cellCreator,
      ICellVisualFactory cellVisualFactory,
      List<CellModel> cells,
      GameInfo gameInfo,
      List<CellVisual> cellVisuals,
      IGameStateMachine gameStateMachine)
    {
      _networkBus = networkBus;
      _cellCreator = cellCreator;
      _cellVisualFactory = cellVisualFactory;
      _cells = cells;
      _gameInfo = gameInfo;
      _cellVisuals = cellVisuals;
      _gameStateMachine = gameStateMachine;

      _networkBus.SubscribeOnRpcWithParameter<StartGameResponse>(CreateField);
      _networkBus.SubscribeOnRpcWithParameter<DefinedShapeResponse>(DefineShape);
    }

    private async void CreateField(StartGameResponse response)
    {
      _gameInfo.Rules.Data = response.GameRules;
      _gameInfo.BackgroundIndex.Value = response.BackgroundIndex;
      await _gameStateMachine.SwitchState<GameplayGameState>();

      _cellCreator.CreateCells(_cells);

      foreach (CellModel cell in _cells)
        _cellVisuals.Add(_cellVisualFactory.Create(cell));
    }

    private void DefineShape(DefinedShapeResponse response)
    {
      _gameInfo.Shape = response.Shape;
    }

    public void Dispose()
    {
      _networkBus.UnsubscribeFromRpc<StartGameResponse>();
      _networkBus.UnsubscribeFromRpc<DefinedShapeResponse>();
    }
  }
}
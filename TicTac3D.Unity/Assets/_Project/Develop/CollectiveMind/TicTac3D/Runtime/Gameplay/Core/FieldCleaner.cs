using System;
using System.Collections.Generic;
using CollectiveMind.TicTac3D.Runtime.GameStateComponents;
using CollectiveMind.TicTac3D.Runtime.Network;
using Cysharp.Threading.Tasks;
using Object = UnityEngine.Object;

namespace CollectiveMind.TicTac3D.Runtime.Gameplay
{
  public class FieldCleaner : IFieldCleaner, IDisposable
  {
    private readonly List<CellModel> _cells;
    private readonly List<CellVisual> _cellVisuals;
    private readonly INetworkBus _networkBus;
    private readonly IGameStateMachine _gameStateMachine;
    private readonly GameInfo _gameInfo;

    public FieldCleaner(List<CellModel> cells,
      List<CellVisual> cellVisuals,
      INetworkBus networkBus,
      IGameStateMachine gameStateMachine,
      GameInfo gameInfo)
    {
      _cells = cells;
      _cellVisuals = cellVisuals;
      _networkBus = networkBus;
      _gameStateMachine = gameStateMachine;
      _gameInfo = gameInfo;

      _networkBus.SubscribeOnRpcWithParameter<FinishGameResponse>(FinishGame);
    }

    private void FinishGame(FinishGameResponse response)
    {
      _gameInfo.Winner = response.Winner;
      _gameInfo.Result = _gameInfo.Winner == ShapeType.XO
        ? GameResultType.Draw
        : _gameInfo.Winner == _gameInfo.Shape
          ? GameResultType.Win
          : GameResultType.Lose;
      
      _gameStateMachine.SwitchState<EndGameState>().Forget();
    }

    public void CleanField()
    {
      foreach (CellVisual cell in _cellVisuals)
        Object.Destroy(cell.gameObject);

      _cellVisuals.Clear();
      _cells.Clear();
    }

    public void Dispose()
    {
      _networkBus.UnsubscribeFromRpc<FinishGameResponse>();
    }
  }
}
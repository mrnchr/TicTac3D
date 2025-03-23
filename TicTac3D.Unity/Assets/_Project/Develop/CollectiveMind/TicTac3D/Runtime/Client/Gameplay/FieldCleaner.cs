using System;
using System.Collections.Generic;
using CollectiveMind.TicTac3D.Runtime.Client.Gameplay.Cell;
using CollectiveMind.TicTac3D.Runtime.Client.GameStateComponents;
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay;
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay.Cell;
using CollectiveMind.TicTac3D.Runtime.Shared.Network;
using Cysharp.Threading.Tasks;
using Object = UnityEngine.Object;

namespace CollectiveMind.TicTac3D.Runtime.Client.Gameplay
{
  public class FieldCleaner : IDisposable
  {
    private readonly List<CellModel> _cells;
    private readonly List<CellVisual> _cellVisuals;
    private readonly INetworkBus _networkBus;
    private readonly IGameStateMachine _gameStateMachine;

    public FieldCleaner(List<CellModel> cells,
      List<CellVisual> cellVisuals,
      INetworkBus networkBus,
      IGameStateMachine gameStateMachine)
    {
      _cells = cells;
      _cellVisuals = cellVisuals;
      _networkBus = networkBus;
      _gameStateMachine = gameStateMachine;

      _networkBus.SubscribeOnRpcWithParameter<FinishGameResponse>(FinishGame);
    }

    private void FinishGame(FinishGameResponse response)
    {
      CleanField();
      _gameStateMachine.SwitchState<MenuGameState>().Forget();
    }

    private void CleanField()
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
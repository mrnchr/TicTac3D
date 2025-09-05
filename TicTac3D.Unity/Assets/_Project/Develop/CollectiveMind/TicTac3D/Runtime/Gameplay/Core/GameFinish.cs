using System;
using CollectiveMind.TicTac3D.Runtime.GameStateComponents;
using CollectiveMind.TicTac3D.Runtime.Network;
using Cysharp.Threading.Tasks;

namespace CollectiveMind.TicTac3D.Runtime.Gameplay
{
  public class GameFinish : IDisposable
  {
    private readonly INetworkBus _networkBus;
    private readonly IGameStateMachine _gameStateMachine;
    private readonly GameInfo _gameInfo;

    public GameFinish(INetworkBus networkBus,
      IGameStateMachine gameStateMachine,
      GameInfo gameInfo)
    {
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

    public void Dispose()
    {
      _networkBus.UnsubscribeFromRpc<FinishGameResponse>();
    }
  }
}
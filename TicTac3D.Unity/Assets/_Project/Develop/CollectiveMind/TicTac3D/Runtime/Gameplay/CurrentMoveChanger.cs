using System;
using CollectiveMind.TicTac3D.Runtime.Network;

namespace CollectiveMind.TicTac3D.Runtime.Gameplay
{
  public class CurrentMoveChanger : IDisposable
  {
    private readonly INetworkBus _networkBus;
    private readonly GameInfo _gameInfo;

    public CurrentMoveChanger(INetworkBus networkBus, GameInfo gameInfo)
    {
      _networkBus = networkBus;
      _gameInfo = gameInfo;

      _networkBus.SubscribeOnRpcWithParameter<ChangedMoveResponse>(ChangeCurrentMove);
    }

    private void ChangeCurrentMove(ChangedMoveResponse response)
    {
      _gameInfo.CurrentMove.Value = response.CurrentMove;
    }

    public void Dispose()
    {
      _networkBus.UnsubscribeFromRpc<ChangedMoveResponse>();
    }
  }
}
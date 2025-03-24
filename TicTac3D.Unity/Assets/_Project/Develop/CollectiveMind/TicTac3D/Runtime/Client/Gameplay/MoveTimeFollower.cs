using System;
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay;
using CollectiveMind.TicTac3D.Runtime.Shared.Network;

namespace CollectiveMind.TicTac3D.Runtime.Client.Gameplay
{
  public class MoveTimeFollower : IDisposable
  {
    private readonly INetworkBus _networkBus;
    private readonly GameInfo _gameInfo;

    public MoveTimeFollower(INetworkBus networkBus, GameInfo gameInfo)
    {
      _networkBus = networkBus;
      _gameInfo = gameInfo;
      _networkBus.SubscribeOnRpcWithParameter<UpdateMoveTimeResponse>(UpdateMoveTime);
    }

    private void UpdateMoveTime(UpdateMoveTimeResponse response)
    {
      _gameInfo.MoveTime.Value = response.Time;
    }

    public void Dispose()
    {
      _networkBus.UnsubscribeFromRpc<UpdateMoveTimeResponse>();
    }
  }
}
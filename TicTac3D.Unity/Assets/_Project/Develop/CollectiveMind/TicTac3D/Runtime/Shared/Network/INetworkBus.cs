using System;

namespace CollectiveMind.TicTac3D.Runtime.Shared.Network
{
  public interface INetworkBus
  {
    void SubscribeOnRpc<T>(Action handler);
    void SubscribeOnRpcWithParameter<T>(Action<T> handler);
    void UnsubscribeFromRpc<T>();
    void HandleRpc<T>(T rpcData);
  }
}
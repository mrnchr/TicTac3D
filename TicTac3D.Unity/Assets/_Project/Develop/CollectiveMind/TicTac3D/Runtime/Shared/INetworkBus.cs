using System;

namespace CollectiveMind.TicTac3D.Runtime.Shared
{
  public interface INetworkBus
  {
    void AddRpc<T>(Action handler);
    void AddRpcWithParameter<T>(Action<T> handler);
    void RemoveRpc<T>();
    void HandleRpc<T>(T rpcData);
  }
}
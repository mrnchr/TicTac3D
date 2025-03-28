﻿using System;
using Unity.Netcode;

namespace CollectiveMind.TicTac3D.Runtime.Shared.Network
{
  public interface INetworkBus
  {
    void SubscribeOnRpc<T>(Action handler);
    void SubscribeOnRpcWithParameter<T>(Action<T> handler);
    void SubscribeOnRpcWithParameter<T>(Action<RpcParams> handler);
    void SubscribeOnRpcWithParameters<T>(Action<T, RpcParams> handler);
    void UnsubscribeFromRpc<T>();
    void HandleRpc<T>(T rpcData, RpcParams rpcParams);
    void OnVariableChanged<TVariable>(TVariable previousValue, TVariable currentValue) where TVariable : struct;
  }
}
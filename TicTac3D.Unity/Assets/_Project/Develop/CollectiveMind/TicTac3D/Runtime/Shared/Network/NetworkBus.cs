using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace CollectiveMind.TicTac3D.Runtime.Shared.Network
{
  public class NetworkBus : INetworkBus
  {
    private Dictionary<Type, Delegate> _rpcs = new Dictionary<Type, Delegate>();

    public void SubscribeOnRpc<T>(Action handler)
    {
      Type type = typeof(T);
      _rpcs[type] = handler;
    }

    public void SubscribeOnRpcWithParameter<T>(Action<RpcParams> handler)
    {
      Type type = typeof(T);
      _rpcs[type] = handler;
    }

    public void SubscribeOnRpcWithParameter<T>(Action<T> handler)
    {
      Type type = typeof(T);
      _rpcs[type] = handler;
    }

    public void SubscribeOnRpcWithParameters<T>(Action<T, RpcParams> handler)
    {
      Type type = typeof(T);
      _rpcs[type] = handler;
    }

    public void UnsubscribeFromRpc<T>()
    {
      Type type = typeof(T);
      _rpcs.Remove(type);
    }

    public void HandleRpc<T>(T rpcData, RpcParams rpcParams)
    {
      if (_rpcs.TryGetValue(typeof(T), out Delegate handler))
      {
        Debug.Log($"Calling RPC handler for {typeof(T).Name}");
        switch (handler.Method.GetParameters().Length)
        {
          case > 1:
            handler.DynamicInvoke(rpcData, rpcParams);
            break;
          case > 0 when handler.Method.GetParameters()[0].ParameterType == typeof(RpcParams):
            handler.DynamicInvoke(rpcParams);
            break;
          case > 0:
            handler.DynamicInvoke(rpcData);
            break;
          default:
            handler.DynamicInvoke();
            break;
        }
      }
    }
  }
}
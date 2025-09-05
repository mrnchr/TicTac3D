using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.Netcode;
using UnityEngine;

namespace CollectiveMind.TicTac3D.Runtime.Network
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
      Debug.Log($"HandleRpc<{typeof(T).Name}>");
      if (_rpcs.TryGetValue(typeof(T), out Delegate handler))
      {
        ParameterInfo[] parameters = handler.Method.GetParameters();
        switch (parameters.Length)
        {
          case > 1:
            handler.DynamicInvoke(rpcData, rpcParams);
            break;
          case > 0 when parameters[0].ParameterType == typeof(RpcParams):
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

    public void OnVariableChanged<TVariable>(TVariable previousValue, TVariable currentValue) where TVariable : struct
    {
      Debug.Log($"OnValueChanged<{typeof(TVariable).Name}>");
      if (_rpcs.TryGetValue(typeof(TVariable), out Delegate handler))
      {
        switch (handler.Method.GetParameters().Length)
        {
          case > 1:
            handler.DynamicInvoke(previousValue, currentValue);
            break;
          case > 0:
            handler.DynamicInvoke(currentValue);
            break;
          default:
            handler.DynamicInvoke();
            break;
        }
      }
    }
  }
}
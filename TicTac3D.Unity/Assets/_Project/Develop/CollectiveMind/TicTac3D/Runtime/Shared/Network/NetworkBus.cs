using System;
using System.Collections.Generic;

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

    public void SubscribeOnRpcWithParameter<T>(Action<T> handler)
    {
      Type type = typeof(T);
      _rpcs[type] = handler;
    }

    public void UnsubscribeFromRpc<T>()
    {
      Type type = typeof(T);
      _rpcs.Remove(type);
    }

    public void HandleRpc<T>(T rpcData)
    {
      if (_rpcs.TryGetValue(typeof(T), out Delegate handler))
      {
        if(handler.Method.GetParameters().Length > 0)
          handler.DynamicInvoke(rpcData);
        else 
          handler.DynamicInvoke();
      }
    }
  }
}
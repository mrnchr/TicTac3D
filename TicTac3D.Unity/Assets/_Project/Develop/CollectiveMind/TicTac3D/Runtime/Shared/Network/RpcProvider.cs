using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Netcode;
using UnityEngine;

namespace CollectiveMind.TicTac3D.Runtime.Shared.Network
{
  public class RpcProvider : IRpcProvider
  {
    private readonly Dictionary<RpcHandlerKey, Delegate> _rpcMap = new Dictionary<RpcHandlerKey, Delegate>();
    private readonly NetworkManager _networkManager;
    private NetworkBridge _bridge;

    public RpcProvider(NetworkManager networkManager)
    {
      _networkManager = networkManager;
    }

    public void SetBridge(NetworkBridge bridge)
    {
      _rpcMap.Clear();
      _bridge = bridge;
      if (!_bridge)
        return;

      Type type = _bridge.GetType();
      foreach (MethodInfo method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance)
        .Where(x => x.Name.EndsWith("Rpc")))
      {
        Type parameterType = method.GetParameters()[0].ParameterType;
        _rpcMap.Add(new RpcHandlerKey(parameterType, method.Name.EndsWith("ServerRpc")),
          Delegate.CreateDelegate(typeof(Action<,>).MakeGenericType(parameterType, typeof(RpcParams)), _bridge,
            method));
      }
    }

    public void SendRequest<TRequest>() where TRequest : new()
    {
      SendRequest<TRequest>(GetDefaultRpcParams());
    }

    public void SendRequest<TRequest>(TRequest request) where TRequest : new()
    {
      SendRequest(request, GetDefaultRpcParams());
    }

    public void SendRequest<TRequest>(RpcParams rpcParams) where TRequest : new()
    {
      SendRequest(new TRequest(), rpcParams);
    }

    public void SendRequest<TRequest>(TRequest request, RpcParams rpcParams)
    {
      if (_bridge && _rpcMap.TryGetValue(new RpcHandlerKey(typeof(TRequest), !_networkManager.IsServer),
        out Delegate action))
      {
        Debug.Log($"SendRequest<{typeof(TRequest).Name}>");
        action.DynamicInvoke(request, rpcParams);
      }
    }

    private RpcParams GetDefaultRpcParams()
    {
      RpcTarget target = _networkManager.RpcTarget;
      return _networkManager.IsServer ? target.NotServer : target.Server;
    }

    private struct RpcHandlerKey
    {
      public Type Type;
      public bool IsServer;

      public RpcHandlerKey(Type type, bool isServer)
      {
        Type = type;
        IsServer = isServer;
      }
    }
  }
}
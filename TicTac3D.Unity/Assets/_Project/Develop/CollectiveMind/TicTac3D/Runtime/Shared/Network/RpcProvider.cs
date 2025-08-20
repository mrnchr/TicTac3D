using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Unity.Netcode;
using UnityEngine;

namespace CollectiveMind.TicTac3D.Runtime.Shared.Network
{
  public class RpcProvider : IRpcProvider
  {
    private readonly Dictionary<RpcHandlerKey, Delegate> _rpcMap = new Dictionary<RpcHandlerKey, Delegate>();
    private readonly Dictionary<Type, VariableChanger> _variableMap = new Dictionary<Type, VariableChanger>();
    private readonly NetworkManager _networkManager;
    private readonly Type _networkBridgeType = typeof(NetworkBridge);
    private readonly Type _networkVariableType = typeof(NetworkVariable<>);

    private readonly MethodInfo _changeVariableMethod =
      typeof(RpcProvider).GetMethod("ChangeVariableExplicit", BindingFlags.NonPublic | BindingFlags.Instance);

    private readonly MethodInfo _onValueChangedMethod =
      typeof(NetworkBridge).GetMethod("OnVariableChanged", BindingFlags.Public | BindingFlags.Instance);

    private NetworkBridge _networkBridge;
    
    public bool IsReady => _networkBridge != null;

    public RpcProvider(NetworkManager networkManager)
    {
      _networkManager = networkManager;
    }

    public void SetBridge(NetworkBridge bridge)
    {
      _rpcMap.Clear();
      _variableMap.Clear();
      _networkBridge = bridge;
      if (_networkBridge)
      {
        MapRpcs();
        MapVariables();
      }
    }

    private void MapRpcs()
    {
      foreach (MethodInfo method in _networkBridgeType
        .GetMethods(BindingFlags.Public | BindingFlags.Instance)
        .Where(x => x.Name.EndsWith("Rpc")))
      {
        Type parameterType = method.GetParameters()[0].ParameterType;
        _rpcMap.Add(new RpcHandlerKey(parameterType, true),
          Delegate.CreateDelegate(typeof(Action<,>).MakeGenericType(parameterType, typeof(RpcParams)), _networkBridge,
            method));
      }
    }

    private void MapVariables()
    {
      foreach (FieldInfo field in _networkBridgeType
        .GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
        .Where(x => x.Name.EndsWith("Variable"))
        .Where(x => x.FieldType.GetGenericTypeDefinition() == _networkVariableType))
      {
        Type variableType = field.FieldType.GetGenericArguments()[0];
        var networkVariable = (NetworkVariableBase)field.GetValue(_networkBridge);
        _variableMap.Add(variableType, new VariableChanger
        {
          NetworkVariable = networkVariable,
          Delegate = Delegate.CreateDelegate(typeof(Action<,>).MakeGenericType(field.FieldType, variableType), this,
            _changeVariableMethod.MakeGenericMethod(variableType))
        });

        FieldInfo onValueChangedField =
          field.FieldType.GetField("OnValueChanged", BindingFlags.Public | BindingFlags.Instance)!;
        var onValueChanged = (Delegate)onValueChangedField.GetValue(networkVariable);
        var onValueChangedAction =
          Delegate.CreateDelegate(onValueChangedField.FieldType, _networkBridge,
            _onValueChangedMethod.MakeGenericMethod(variableType));
        onValueChangedField.SetValue(networkVariable, Delegate.Combine(onValueChanged, onValueChangedAction));
      }
    }

    public void SendRequest<TRequest>() where TRequest : struct
    {
      SendRequest<TRequest>(GetDefaultRpcParams());
    }

    public void SendRequest<TRequest>(TRequest request) where TRequest : struct
    {
      SendRequest(request, GetDefaultRpcParams());
    }

    public void SendRequest<TRequest>(RpcParams rpcParams) where TRequest : struct
    {
      SendRequest(new TRequest(), rpcParams);
    }

    public void SendRequest<TRequest>(TRequest request, RpcParams rpcParams) where TRequest : struct
    {
      if (_networkBridge && _rpcMap.TryGetValue(new RpcHandlerKey(typeof(TRequest), _networkManager.IsClient),
        out Delegate action))
      {
        Debug.Log($"SendRequest<{typeof(TRequest).Name}>");
        action.DynamicInvoke(request, rpcParams);
      }
    }

    public void ChangeVariable<TVariable>(TVariable value) where TVariable : struct
    {
      if (_networkBridge && _variableMap.TryGetValue(typeof(TVariable), out VariableChanger changer))
      {
        Debug.Log($"ChangeVariable<{typeof(TVariable).Name}>");
        changer.Delegate.DynamicInvoke(changer.NetworkVariable, value);
      }
    }

    [UsedImplicitly]
    private void ChangeVariableExplicit<TVariable>(NetworkVariable<TVariable> networkVariable, TVariable nextValue)
      where TVariable : struct
    {
      networkVariable.Value = nextValue;
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

    private struct VariableChanger
    {
      public NetworkVariableBase NetworkVariable;
      public Delegate Delegate;
    }
  }
}
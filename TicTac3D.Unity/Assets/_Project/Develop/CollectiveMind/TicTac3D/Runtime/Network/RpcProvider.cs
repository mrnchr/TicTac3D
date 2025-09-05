using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Unity.Netcode;
using UnityEngine;

namespace CollectiveMind.TicTac3D.Runtime.Network
{
  public class RpcProvider : IRpcProvider
  {
    private readonly Dictionary<Type, RpcHandlerValue> _rpcMap = new Dictionary<Type, RpcHandlerValue>();
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
      if (_networkBridge == bridge)
        return;
      
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
        _rpcMap.Add(parameterType,
          new RpcHandlerValue(
            Delegate.CreateDelegate(typeof(Action<,>).MakeGenericType(parameterType, typeof(RpcParams)), _networkBridge,
              method), method.Name.EndsWith("ServerRpc")));
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
      SendRequest<TRequest>(new RpcParams());
    }

    public void SendRequest<TRequest>(TRequest request) where TRequest : struct
    {
      SendRequest(request, new RpcParams());
    }

    public void SendRequest<TRequest>(RpcParams rpcParams) where TRequest : struct
    {
      SendRequest(new TRequest(), rpcParams);
    }

    public void SendRequest<TRequest>(TRequest request, RpcParams rpcParams) where TRequest : struct
    {
      if (_networkBridge && _rpcMap.TryGetValue(typeof(TRequest), out RpcHandlerValue handler))
      {
        if (rpcParams.Send.Target == null)
          rpcParams = GetDefaultRpcParams(handler.ToServer);
        
        Debug.Log($"SendRequest<{typeof(TRequest).Name}>({rpcParams.Send.Target.GetType().Name})");
        handler.Delegate.DynamicInvoke(request, rpcParams);
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

    private RpcParams GetDefaultRpcParams(bool forServer)
    {
      RpcTarget target = _networkManager.RpcTarget;
      return forServer ? target.Server : target.ClientsAndHost;
    }

    private struct RpcHandlerValue
    {
      public Delegate Delegate;
      public bool ToServer;

      public RpcHandlerValue(Delegate @delegate, bool toServer)
      {
        Delegate = @delegate;
        ToServer = toServer;
      }
    }

    private struct VariableChanger
    {
      public NetworkVariableBase NetworkVariable;
      public Delegate Delegate;
    }
  }
}
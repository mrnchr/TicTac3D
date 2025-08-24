using CollectiveMind.TicTac3D.Runtime.Gameplay;
using JetBrains.Annotations;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace CollectiveMind.TicTac3D.Runtime.Network
{
  public class NetworkBridge : NetworkBehaviour
  {
    private INetworkBus _networkBus;
    private IRpcProvider _rpcProvider;

    [SerializeField]
    [UsedImplicitly]
    private NetworkVariable<MoveTimeVariable> _moveTimeVariable = new NetworkVariable<MoveTimeVariable>();

    [Inject]
    public void Construct(INetworkBus networkBus, IRpcProvider rpcProvider)
    {
      _rpcProvider = rpcProvider;
      _networkBus = networkBus;
    }

    [UsedImplicitly]
    public void OnVariableChanged<TVariable>(TVariable previousValue, TVariable currentValue) where TVariable : struct
    {
      _networkBus.OnVariableChanged(previousValue, currentValue);
    }

    public override void OnNetworkSpawn()
    {
      _rpcProvider.SetBridge(this);
    }

    public override void OnNetworkDespawn()
    {
      _rpcProvider.SetBridge(null);
    }

    [Rpc(SendTo.SpecifiedInParams)]
    [UsedImplicitly]
    public void SendRequestClientRpc(StartGameResponse response, RpcParams rpcParams)
    {
      _networkBus.HandleRpc(response, rpcParams);
    }

    [Rpc(SendTo.SpecifiedInParams)]
    [UsedImplicitly]
    public void SendResponseClientRpc(DefinedShapeResponse response, RpcParams rpcParams)
    {
      _networkBus.HandleRpc(response, rpcParams);
    }

    [Rpc(SendTo.SpecifiedInParams)]
    [UsedImplicitly]
    public void SendResponseClientRpc(ChangedMoveResponse response, RpcParams rpcParams)
    {
      _networkBus.HandleRpc(response, rpcParams);
    }

    [Rpc(SendTo.SpecifiedInParams)]
    [UsedImplicitly]
    public void SendRequestServerRpc(SetShapeRequest request, RpcParams rpcParams)
    {
      _networkBus.HandleRpc(request, rpcParams);
    }

    [Rpc(SendTo.SpecifiedInParams)]
    [UsedImplicitly]
    public void SendResponseClientRpc(UpdateShapeResponse response, RpcParams rpcParams)
    {
      _networkBus.HandleRpc(response, rpcParams);
    }

    [Rpc(SendTo.SpecifiedInParams)]
    [UsedImplicitly]
    public void SendRequestServerRpc(StartGameRequest request, RpcParams rpcParams)
    {
      _networkBus.HandleRpc(request, rpcParams);
    }

    [Rpc(SendTo.SpecifiedInParams)]
    [UsedImplicitly]
    public void SendResponseClientRpc(FinishGameResponse response, RpcParams rpcParams)
    {
      _networkBus.HandleRpc(response, rpcParams);
    }

    [Rpc(SendTo.SpecifiedInParams)]
    [UsedImplicitly]
    public void SendRequestServerRpc(StopSearchGameRequest request, RpcParams rpcParams)
    {
      _networkBus.HandleRpc(request, rpcParams);
    }

    [Rpc(SendTo.SpecifiedInParams)]
    [UsedImplicitly]
    public void SendRequestServerRpc(LeaveGameRequest request, RpcParams rpcParams)
    {
      _networkBus.HandleRpc(request, rpcParams);
    }
    
    [Rpc(SendTo.SpecifiedInParams)]
    [UsedImplicitly]
    public void SendResponseClientRpc(UpdateMoveTimeResponse response, RpcParams rpcParams)
    {
      _networkBus.HandleRpc(response, rpcParams);
    }

    [Rpc(SendTo.SpecifiedInParams)]
    [UsedImplicitly]
    public void SendResponseClientRpc(UpdateLifeTimeResponse response, RpcParams rpcParams)
    {
      _networkBus.HandleRpc(response, rpcParams);
    }
  }
}
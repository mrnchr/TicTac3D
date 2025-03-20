using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay;
using JetBrains.Annotations;
using Unity.Netcode;
using Zenject;

namespace CollectiveMind.TicTac3D.Runtime.Shared.Network
{
  public class NetworkBridge : NetworkBehaviour
  {
    private INetworkBus _networkBus;
    private IRpcProvider _rpcProvider;

    [Inject]
    public void Construct(INetworkBus networkBus, IRpcProvider rpcProvider)
    {
      _rpcProvider = rpcProvider;
      _networkBus = networkBus;
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
    public void SendRequestClientRpc(int request, RpcParams rpcParams)
    {
      _networkBus.HandleRpc(request);
    }
    
    [Rpc(SendTo.SpecifiedInParams)]
    [UsedImplicitly]
    public void SendResponseServerRpc(int response, RpcParams rpcParams)
    {
      _networkBus.HandleRpc(response);
    }
    
    [Rpc(SendTo.SpecifiedInParams)]
    [UsedImplicitly]
    public void SendRequestClientRpc(GameStartedEvent response, RpcParams rpcParams)
    {
      _networkBus.HandleRpc(response);
    }
  }
}
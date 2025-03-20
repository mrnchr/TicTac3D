using System.Linq;
using CollectiveMind.TicTac3D.Runtime.Shared.AssetManagement;
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay;
using CollectiveMind.TicTac3D.Runtime.Shared.Network;
using R3;
using Unity.Netcode;
using Zenject;

namespace CollectiveMind.TicTac3D.Runtime.Server
{
  public class GameStarter : ITickable
  {
    private readonly NetworkManager _networkManager;
    private readonly IRpcProvider _rpcProvider;
    private readonly IPrefabLoader _prefabLoader;
    private bool _isGameStarted;

    private readonly ReactiveProperty<bool> _isPlayerSpawned = new ReactiveProperty<bool>();

    public GameStarter(NetworkManager networkManager, IRpcProvider rpcProvider, IPrefabLoader prefabLoader)
    {
      _networkManager = networkManager;
      _rpcProvider = rpcProvider;
      _prefabLoader = prefabLoader;

      _isPlayerSpawned.Subscribe(OnClientConnected);
      _networkManager.OnServerStarted += OnServerStarted;
    }

    private void OnServerStarted()
    {
      var player = _prefabLoader.LoadPrefab<NetworkObject>(EntityType.Player);
      _networkManager.SpawnManager.InstantiateAndSpawn(player);
      _prefabLoader.UnloadPrefab(EntityType.Player);
    }

    public void Tick()
    {
      _isPlayerSpawned.Value = _networkManager.IsServer
        && _networkManager.ConnectedClients.Count == 2
        && _networkManager.SpawnManager.PlayerObjects.All(x => x.IsSpawned);
    }

    private void OnClientConnected(bool isPlayerSpawned)
    {
      if (isPlayerSpawned)
      {
        _isGameStarted = true;
        _rpcProvider.SendRequest<GameStartedEvent>(_networkManager.RpcTarget.Everyone);
      }
    }
  }
}
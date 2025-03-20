using System;
using System.Linq;
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay;
using CollectiveMind.TicTac3D.Runtime.Shared.Network;
using R3;
using Unity.Netcode;
using Zenject;

namespace CollectiveMind.TicTac3D.Runtime.Server
{
  public class GameStarter : IDisposable, ITickable
  {
    private readonly NetworkManager _networkManager;
    private readonly IRpcProvider _rpcProvider;
    private bool _isGameStarted;

    private readonly ReactiveProperty<bool> _isPlayerSpawned = new ReactiveProperty<bool>();

    public GameStarter(NetworkManager networkManager, IRpcProvider rpcProvider)
    {
      _networkManager = networkManager;
      _rpcProvider = rpcProvider;
      
      _isPlayerSpawned.Subscribe(OnClientConnected);
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
        _rpcProvider.SendRequest<GameStartedEvent>();
      }
    }

    public void Dispose()
    {
    }
  }
}
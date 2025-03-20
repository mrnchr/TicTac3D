using System;
using System.Collections.Generic;
using System.Linq;
using CollectiveMind.TicTac3D.Runtime.Shared.AssetManagement;
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay;
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay.Cell;
using CollectiveMind.TicTac3D.Runtime.Shared.Network;
using R3;
using Unity.Netcode;
using Zenject;
using Random = UnityEngine.Random;

namespace CollectiveMind.TicTac3D.Runtime.Server
{
  public class GameStarter : ITickable, IDisposable
  {
    private readonly NetworkManager _networkManager;
    private readonly IRpcProvider _rpcProvider;
    private readonly IPrefabLoader _prefabLoader;
    private readonly SessionInfo _sessionInfo;
    private bool _isGameStarted;

    private readonly ReactiveProperty<bool> _isPlayerSpawned = new ReactiveProperty<bool>();

    public GameStarter(NetworkManager networkManager,
      IRpcProvider rpcProvider,
      IPrefabLoader prefabLoader,
      SessionInfo sessionInfo)
    {
      _networkManager = networkManager;
      _rpcProvider = rpcProvider;
      _prefabLoader = prefabLoader;
      _sessionInfo = sessionInfo;

      _networkManager.OnServerStarted += OnServerStarted;
      _isPlayerSpawned.Subscribe(StartGame);
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

    private void StartGame(bool isPlayerSpawned)
    {
      if (isPlayerSpawned)
      {
        _isGameStarted = true;
        _sessionInfo.ClientInfos.Clear();
        List<ShapeType> shapes = new List<ShapeType> { ShapeType.X, ShapeType.O };
        foreach (KeyValuePair<ulong, NetworkClient> client in _networkManager.ConnectedClients)
        {
          int index = Random.Range(0, shapes.Count);
          _sessionInfo.ClientInfos.Add(new SessionInfoAboutClient
          {
            Client = client.Value,
            Shape = shapes[index]
          });
          
          shapes.RemoveAt(index);
        }

        _rpcProvider.SendRequest<StartedGameResponse>(_networkManager.RpcTarget.Everyone);
      }
    }


    public void Dispose()
    {
      _networkManager.OnServerStarted -= OnServerStarted;
    }
  }
}
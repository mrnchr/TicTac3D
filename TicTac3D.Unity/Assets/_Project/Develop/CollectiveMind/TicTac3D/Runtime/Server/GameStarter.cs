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
        _sessionInfo.ClientInfos.Clear();
        _rpcProvider.SendRequest<StartedGameResponse>(_networkManager.RpcTarget.Everyone);
        var shapes = new List<ShapeType> { ShapeType.X, ShapeType.O };
        foreach (KeyValuePair<ulong, NetworkClient> client in _networkManager.ConnectedClients)
        {
          int index = Random.Range(0, shapes.Count);
          ShapeType shape = shapes[index];
          _sessionInfo.ClientInfos.Add(new SessionInfoAboutClient
          {
            Client = client.Value,
            Shape = shapes[index]
          });

          _rpcProvider.SendRequest(new DefinedShapeResponse { Shape = shape },
            _networkManager.RpcTarget.Single(client.Key, RpcTargetUse.Persistent));

          shapes.RemoveAt(index);
        }

        _sessionInfo.CurrentMove = ShapeType.X;
        _rpcProvider.SendRequest(new ChangedMoveResponse { CurrentMove = ShapeType.X });
      }
    }

    public void Dispose()
    {
      _networkManager.OnServerStarted -= OnServerStarted;
    }
  }
}
using System;
using System.Collections.Generic;
using System.Linq;
using CollectiveMind.TicTac3D.Runtime.Server.Session;
using CollectiveMind.TicTac3D.Runtime.Shared.AssetManagement;
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay;
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay.Cell;
using CollectiveMind.TicTac3D.Runtime.Shared.Network;
using Cysharp.Threading.Tasks;
using Unity.Netcode;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace CollectiveMind.TicTac3D.Runtime.Server
{
  public class GameStarter : IGameStarter, IDisposable
  {
    private readonly NetworkManager _networkManager;
    private readonly IRpcProvider _rpcProvider;
    private readonly IPrefabLoader _prefabLoader;
    private readonly ICellCreator _cellCreator;

    public GameStarter(NetworkManager networkManager,
      IRpcProvider rpcProvider,
      IPrefabLoader prefabLoader,
      ICellCreator cellCreator)
    {
      _networkManager = networkManager;
      _rpcProvider = rpcProvider;
      _prefabLoader = prefabLoader;
      _cellCreator = cellCreator;
      
      _networkManager.OnServerStarted += OnServerStarted;
    }
    
    private void OnServerStarted()
    {
      var bridgePrefab = _prefabLoader.LoadPrefab<NetworkObject>(EntityType.NetworkBridge);
      Object.DontDestroyOnLoad(_networkManager.SpawnManager.InstantiateAndSpawn(bridgePrefab));
      _prefabLoader.UnloadPrefab(EntityType.NetworkBridge);
    }

    public async void StartGame(GameSession session)
    {
      await UniTask.WaitUntil(() =>
        session.Players.All(x => Object.FindAnyObjectByType<NetworkBridge>().IsSpawned));

      _rpcProvider.SendRequest<StartedGameResponse>(session.Target);
      _cellCreator.CreateCells(session.Cells);
      var shapes = new List<ShapeType> { ShapeType.X, ShapeType.O };
      foreach (PlayerInfo player in session.Players)
      {
        int index = Random.Range(0, shapes.Count);
        ShapeType shape = shapes[index];
        player.Shape = shape;

        _rpcProvider.SendRequest(new DefinedShapeResponse { Shape = shape },
          _networkManager.RpcTarget.Single(player.PlayerId, RpcTargetUse.Persistent));

        shapes.RemoveAt(index);
      }

      session.CurrentMove = ShapeType.X;
      _rpcProvider.SendRequest(new ChangedMoveResponse { CurrentMove = ShapeType.X }, session.Target);
    }

    public void Dispose()
    {
      _networkManager.OnServerStarted -= OnServerStarted;
    }
  }
}
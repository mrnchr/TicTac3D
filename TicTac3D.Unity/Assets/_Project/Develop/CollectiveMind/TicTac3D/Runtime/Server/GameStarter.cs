using System;
using System.Collections.Generic;
using System.Linq;
using CollectiveMind.TicTac3D.Runtime.Server.Session;
using CollectiveMind.TicTac3D.Runtime.Shared.AssetManagement;
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay;
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay.Cell;
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay.Rules;
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay.Shape;
using CollectiveMind.TicTac3D.Runtime.Shared.Network;
using Cysharp.Threading.Tasks;
using Unity.Netcode;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace CollectiveMind.TicTac3D.Runtime.Server
{
  public class GameStarter : IGameStarter, IDisposable
  {
    private readonly List<ShapeType> _defaultShapes = new List<ShapeType> { ShapeType.X, ShapeType.O };
    private readonly List<ShapeType> _shapes = new List<ShapeType>();
    private readonly NetworkManager _networkManager;
    private readonly IRpcProvider _rpcProvider;
    private readonly IPrefabLoader _prefabLoader;
    private readonly IConfigLoader _configLoader;
    private readonly ICellCreator _cellCreator;
    private readonly GameConfig _config;

    public GameStarter(NetworkManager networkManager,
      IRpcProvider rpcProvider,
      IPrefabLoader prefabLoader,
      IConfigLoader configLoader,
      ICellCreator cellCreator)
    {
      _networkManager = networkManager;
      _rpcProvider = rpcProvider;
      _prefabLoader = prefabLoader;
      _configLoader = configLoader;
      _cellCreator = cellCreator;
      _config = configLoader.LoadConfig<GameConfig>();

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
      await UniTask.WaitUntil(() => Object.FindAnyObjectByType<NetworkBridge>().IsSpawned);

      _rpcProvider.SendRequest<StartedGameResponse>(session.Target);
      _cellCreator.CreateCells(session.Cells);

      session.Rules.Data = RandomizeRules(session.JoinPlayerRules());
      DefineShapes(session);

      session.CurrentMove = ShapeType.X;
      _rpcProvider.SendRequest(new ChangedMoveResponse { CurrentMove = ShapeType.X }, session.Target);
    }

    private void DefineShapes(GameSession session)
    {
      _shapes.Clear();
      _shapes.AddRange(_defaultShapes);
      _shapes.RemoveAll(x => session.Players.Any(y => y.GameRules.Data.DesiredShape == x));
      foreach (PlayerInfo player in session.Players)
      {
        ShapeType shape = player.GameRules.Data.DesiredShape;
        if (shape is < ShapeType.X or > ShapeType.O)
        {
          int index = Random.Range(0, _shapes.Count);
          shape = _shapes[index];
          _shapes.RemoveAt(index);
        }

        player.Shape = shape;

        _rpcProvider.SendRequest(new DefinedShapeResponse { Shape = shape },
          _networkManager.RpcTarget.Single(player.PlayerId, RpcTargetUse.Persistent));
      }
    }

    private GameRulesData RandomizeRules(GameRulesData data)
    {
      data.BotMoveCount = RandomizeRule(GameRuleType.BotMoveCount, data.BotMoveCount, data.BotMoveCount < 0);
      data.MoveTime = RandomizeRule(GameRuleType.MoveTime, data.MoveTime, data.MoveTime < 0);
      data.ShapeFading = RandomizeRule(GameRuleType.ShapeFading, data.ShapeFading, data.ShapeFading == ShapeFadingType.None);
      data.FadingMoveCount = RandomizeRule(GameRuleType.FadingMoveCount, data.FadingMoveCount, data.FadingMoveCount < 0);
      return data;
    }

    private TRule RandomizeRule<TRule>(GameRuleType type, TRule value, bool randomize)
    {
      if (randomize)
      {
        List<TRule> rules = _config.GetAvailableRule<TRule>(type);
        return rules[Random.Range(0, rules.Count)];
      }

      return value;
    }

    public void Dispose()
    {
      _configLoader.UnloadConfig<GameConfig>();
      _networkManager.OnServerStarted -= OnServerStarted;
    }
  }
}
using System;
using System.Collections.Generic;
using System.Linq;
using CollectiveMind.TicTac3D.Runtime.Shared.AssetManagement;
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay;
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay.Cell;
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay.Shape;
using CollectiveMind.TicTac3D.Runtime.Shared.Network;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CollectiveMind.TicTac3D.Runtime.Server.Session
{
  public class GameRulesProcessor : IGameRulesProcessor, IDisposable
  {
    private readonly IConfigLoader _configLoader;
    private readonly IRpcProvider _rpcProvider;
    private readonly SessionRegistry _sessionRegistry;
    private readonly GameConfig _config;

    public GameRulesProcessor(IConfigLoader configLoader,
      IRpcProvider rpcProvider,
      SessionRegistry sessionRegistry)
    {
      _configLoader = configLoader;
      _rpcProvider = rpcProvider;
      _sessionRegistry = sessionRegistry;
      _config = _configLoader.LoadConfig<GameConfig>();
    }

    public void SetShape(GameSession session, CellModel cell, ShapeType shape)
    {
      if (shape == session.CurrentMove && !cell.HasShape())
      {
        cell.Shape.Value = shape;
        _rpcProvider.SendRequest(new UpdatedShapeResponse { CellIndex = cell.Index, Shape = shape },
          session.Target);

        ShapeType winner = CheckWin(session);
        if (winner != ShapeType.None)
        {
          session.Winner = winner;
          session.Status = SessionState.Completed;
          _sessionRegistry.Sessions.Remove(session);
          _rpcProvider.SendRequest(new FinishGameResponse { Winner = winner }, session.Target);
          return;
        }

        ShapeType next = GetNextMove(session);
        session.LastMove = session.CurrentMove;
        session.CurrentMove = next;
        _rpcProvider.SendRequest(new ChangedMoveResponse { CurrentMove = session.CurrentMove }, session.Target);

        if (session.CurrentMove == ShapeType.XO)
          MoveByBot(session);
      }
    }

    private ShapeType GetNextMove(GameSession session)
    {
      int botMoveCount = _config.DefaultRules.Data.BotMoveCount;
      ShapeType current = session.CurrentMove;
      return botMoveCount switch
      {
        <= 0 => GetNext(current, 2),
        1 => GetNext(current, 3),
        2 => current == ShapeType.XO ? GetNext(session.LastMove, 2) : ShapeType.XO,
        _ => throw new ArgumentOutOfRangeException()
      };
    }

    private void MoveByBot(GameSession session)
    {
      List<CellModel> cells = session.Cells.Where(x => !x.HasShape()).ToList();
      CellModel cell = cells[Random.Range(0, cells.Count)];
      SetShape(session, cell, ShapeType.XO);
    }

    private ShapeType CheckWin(GameSession session)
    {
      Dictionary<ShapeType, List<CellModel>> shapeCells = session.Cells.Where(x => x.HasShape())
        .GroupBy(x => x.Shape.Value)
        .Where(x => x.Key is >= ShapeType.X and <= ShapeType.O)
        .ToDictionary(x => x.Key, x => x.ToList());

      if (shapeCells.All(x => x.Value.Count <= 2))
        return ShapeType.None;

      foreach (KeyValuePair<ShapeType, List<CellModel>> item in shapeCells)
      {
        if (HasWinCombination(item.Value))
          return item.Key;
      }

      if (session.Cells.All(x => x.HasShape()))
        return ShapeType.XO;

      return ShapeType.None;
    }

    private static bool HasWinCombination(List<CellModel> cells)
    {
      return GetCombinations(cells, 3)
        .Select(x => x.ToList())
        .Any(IsWinCombination);
    }

    private static bool IsWinCombination(List<CellModel> combination)
    {
      Vector3 direction = combination[1].Index - combination[0].Index;
      for (var i = 1; i < combination.Count - 1; i++)
      {
        Vector3 dir = combination[i + 1].Index - combination[i].Index;
        if (Vector3.Cross(direction, dir).sqrMagnitude != 0)
          return false;
      }

      return true;
    }

    private static IEnumerable<IEnumerable<T>> GetCombinations<T>(IEnumerable<T> elements, int k)
    {
      return GetCombinationsRecursive(elements.ToList(), k, 0, new List<T>());
    }

    private static IEnumerable<IEnumerable<T>> GetCombinationsRecursive<T>(List<T> elements, int k, int start,
      List<T> current)
    {
      if (current.Count == k)
      {
        yield return new List<T>(current);
        yield break;
      }

      for (int i = start; i < elements.Count; i++)
      {
        current.Add(elements[i]);
        foreach (IEnumerable<T> combination in GetCombinationsRecursive(elements, k, i + 1, current))
        {
          yield return combination;
        }

        current.RemoveAt(current.Count - 1);
      }
    }

    private ShapeType GetNext(ShapeType current, int count)
    {
      return (ShapeType)((int)current % count + 1);
    }

    public void Dispose()
    {
      _configLoader.UnloadConfig<GameConfig>();
    }
  }
}
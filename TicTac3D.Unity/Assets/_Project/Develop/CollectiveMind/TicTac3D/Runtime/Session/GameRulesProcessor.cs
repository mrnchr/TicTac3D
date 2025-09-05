using System;
using System.Collections.Generic;
using System.Linq;
using CollectiveMind.TicTac3D.Runtime.AssetManagement;
using CollectiveMind.TicTac3D.Runtime.Gameplay;
using CollectiveMind.TicTac3D.Runtime.Network;
using UnityEngine;

namespace CollectiveMind.TicTac3D.Runtime.Session
{
  public class GameRulesProcessor : IGameRulesProcessor, IDisposable
  {
    private readonly IRpcProvider _rpcProvider;
    private readonly SessionProvider _sessionProvider;
    private readonly IBotBrain _botBrain;
    private readonly IConfigLoader _configLoader;
    private readonly GameConfig _config;

    public GameRulesProcessor(IRpcProvider rpcProvider,
      SessionProvider sessionProvider,
      IBotBrain botBrain,
      IConfigLoader configLoader)
    {
      _rpcProvider = rpcProvider;
      _sessionProvider = sessionProvider;
      _botBrain = botBrain;
      _configLoader = configLoader;
      _config = configLoader.LoadConfig<GameConfig>();
    }

    public void SetShape(GameSession session, CellModel cell, ShapeType shape)
    {
      if (shape == session.CurrentMove && !cell.HasShape())
      {
        SetShapeImplicit(session, cell, shape);

        ShapeFadingType fading = session.Rules.Data.ShapeFading;
        if (fading > ShapeFadingType.Off && cell.HasShape())
        {
          var lifeTime = 0;
          if(fading.IsBot() && session.CurrentMove == ShapeType.XO)
            lifeTime = session.Rules.Data.BotFadingMoveCount;
          
          if(fading.IsPlayers() && session.CurrentMove.IsPlayer())
            lifeTime = session.Rules.Data.PlayerFadingMoveCount;
          
          SetLifeTime(session, lifeTime, cell, session.LastMove);
        }

        ShapeType winner = CheckWin(session);
        if (winner != ShapeType.None)
        {
          session.Winner = winner;
          session.Status = SessionState.Completed;
          _rpcProvider.SendRequest(new FinishGameResponse { Winner = winner }, session.Target);
          return;
        }

        ChangeMove(session);
      }
    }

    private ShapeType CheckWin(GameSession session)
    {
      Dictionary<ShapeType, List<CellModel>> shapeCells = session.GroupCellsByPlayerShape();
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
      return Combinatorics.GetCombinations(cells, 3)
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

    public void ChangeMove(GameSession session)
    {
      ShapeType next = GetNextMove(session);
      session.LastMove = session.CurrentMove;
      session.CurrentMove = next;
      _rpcProvider.SendRequest(new ChangedMoveResponse { CurrentMove = session.CurrentMove }, session.Target);

      ProcessFading(session);
      
      if (session.CurrentMove != ShapeType.XO)
      {
        if(session.Rules.Data.MoveTime > 0)
          session.MoveTime = session.Rules.Data.MoveTime;
      }
      else
      {
        MoveByBot(session);
      }
    }

    private void ProcessFading(GameSession session)
    {
      if (session.Rules.Data.ShapeFading > ShapeFadingType.Off)
      {
        foreach (CellModel cell in GetFadingCells(session))
        {
          SetLifeTime(session, Mathf.Max(cell.LifeTime.Value - 1, 0), cell, cell.FadingContext);

          if (cell.LifeTime.Value == 0)
            SetShapeImplicit(session, cell, ShapeType.None);
        }
      }
    }

    private IEnumerable<CellModel> GetFadingCells(GameSession session)
    {
      bool isMoveEnd = session.CurrentMove == ShapeType.X && session.LastMove != ShapeType.None;
      ShapeFadingType unifiedFading = session.Rules.Data.ShapeFading & _config.UnifiedFading;
      ShapeFadingType separateFading = session.Rules.Data.ShapeFading & _config.OverridenSeparateFading;
      foreach (CellModel cell in session.Cells)
      {
        if ((isMoveEnd && ((cell.Shape.Value.IsPlayer() && unifiedFading.IsPlayers())
          || (cell.Shape.Value == ShapeType.XO && unifiedFading.IsBot()))) 
          || (cell.Shape.Value == session.CurrentMove && ((separateFading.IsPlayers() && cell.Shape.Value.IsPlayer()) 
            || (separateFading.IsBot() && cell.Shape.Value == ShapeType.XO && cell.FadingContext == session.LastMove))))
        {
          yield return cell;
        }
      }
    }

    private void SetLifeTime(GameSession session, int lifeTime, CellModel cell, ShapeType context = ShapeType.None)
    {
      cell.LifeTime.Value = lifeTime;
      cell.FadingContext = context;
      
      _rpcProvider.SendRequest(new UpdateLifeTimeResponse { CellIndex = cell.Index, LifeTime = lifeTime },
        session.Target);
    }

    private void SetShapeImplicit(GameSession session, CellModel cell, ShapeType shape)
    {
      cell.Shape.Value = shape;
      _rpcProvider.SendRequest(new UpdateShapeResponse { CellIndex = cell.Index, Shape = shape },
        session.Target);
    }

    private ShapeType GetNextMove(GameSession session)
    {
      int botMoveCount = session.Rules.Data.BotMoveCount;
      ShapeType current = session.CurrentMove;
      return botMoveCount switch
      {
        <= 0 => GetNext(current, 2),
        1 => GetNext(current, 3),
        2 => current is ShapeType.None or ShapeType.XO ? GetNext(session.LastMove, 2) : ShapeType.XO,
        _ => throw new ArgumentOutOfRangeException()
      };
    }

    private ShapeType GetNext(ShapeType current, int count)
    {
      return (ShapeType)((int)current % count + 1);
    }

    private void MoveByBot(GameSession session)
    {
      SetShape(session, _botBrain.GetCellToMove(session), ShapeType.XO);
    }

    public void Dispose()
    {
      _configLoader.UnloadConfig<GameConfig>();
    }
  }
}
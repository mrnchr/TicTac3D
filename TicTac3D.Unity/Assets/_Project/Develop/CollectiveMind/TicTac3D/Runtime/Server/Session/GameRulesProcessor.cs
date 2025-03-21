using System;
using System.Collections.Generic;
using System.Linq;
using CollectiveMind.TicTac3D.Runtime.Shared.AssetManagement;
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay;
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay.Cell;
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay.Shape;
using CollectiveMind.TicTac3D.Runtime.Shared.Network;
using Random = UnityEngine.Random;

namespace CollectiveMind.TicTac3D.Runtime.Server.Session
{
  public class GameRulesProcessor : IGameRulesProcessor, IDisposable
  {
    private readonly IConfigLoader _configLoader;
    private readonly IRpcProvider _rpcProvider;
    private readonly GameConfig _config;

    public GameRulesProcessor(IConfigLoader configLoader, IRpcProvider rpcProvider)
    {
      _configLoader = configLoader;
      _rpcProvider = rpcProvider;
      _config = _configLoader.LoadConfig<GameConfig>();
    }

    public void SetShape(GameSession session, CellModel cell, ShapeType shape)
    {
      if (shape == session.CurrentMove && !cell.HasShape())
      {
        cell.Shape.Value = shape;
        _rpcProvider.SendRequest(new UpdatedShapeResponse { CellIndex = cell.Index, Shape = shape },
          session.Target);

        ShapeType next = GetNextMove(session);
        session.LastMove = session.CurrentMove;
        session.CurrentMove = next;
        _rpcProvider.SendRequest(new ChangedMoveResponse { CurrentMove = session.CurrentMove }, session.Target);

        if (session.CurrentMove == ShapeType.XO)
          MoveByBot(session);
      }
    }

    public void MoveByBot(GameSession session)
    {
      List<CellModel> cells = session.Cells.Where(x => !x.HasShape()).ToList();
      CellModel cell = cells[Random.Range(0, cells.Count)];
      SetShape(session, cell, ShapeType.XO);
    }

    public ShapeType GetNextMove(GameSession session)
    {
      int botMoveCount = _config.Rules.BotMoveCount;
      ShapeType current = session.CurrentMove;
      return botMoveCount switch
      {
        <= 0 => GetNext(current, 2),
        1 => GetNext(current, 3),
        2 => current == ShapeType.XO ? GetNext(session.LastMove, 2) : ShapeType.XO,
        _ => throw new ArgumentOutOfRangeException()
      };
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
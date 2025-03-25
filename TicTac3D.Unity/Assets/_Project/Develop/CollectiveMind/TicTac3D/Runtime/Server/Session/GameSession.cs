using System;
using System.Collections.Generic;
using System.Linq;
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay.Cell;
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay.Rules;
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay.Shape;
using Unity.Netcode;

namespace CollectiveMind.TicTac3D.Runtime.Server.Session
{
  [Serializable]
  public class GameSession
  {
    public SessionState Status;

    public BaseRpcTarget Target;
    public List<PlayerInfo> Players = new List<PlayerInfo>();

    public GameRules Rules = new GameRules();
    public List<CellModel> Cells = new List<CellModel>();
    
    public ShapeType LastMove;
    public ShapeType CurrentMove;
    public float MoveTime;

    public ShapeType Winner;

    public PlayerInfo GetPlayerInfo(ulong clientId)
    {
      return Players.Find(x => x.PlayerId == clientId);
    }

    public PlayerInfo GetMovingPlayer()
    {
      return Players.Find(x => x.Shape == CurrentMove);
    }

    public void AddPlayer(ulong playerId, GameRulesData rules)
    {
      var playerInfo = new PlayerInfo
      {
        PlayerId = playerId,
        GameRules = new GameRules { Data = rules }
      };
      Players.Add(playerInfo);
    }

    public Dictionary<ShapeType, List<CellModel>> GroupCellsByPlayerShape()
    {
      return Cells.Where(x => x.HasShape())
        .GroupBy(x => x.Shape.Value)
        .Where(x => x.Key is >= ShapeType.X and <= ShapeType.O)
        .ToDictionary(x => x.Key, x => x.ToList());
    }

    public GameRulesData JoinPlayerRules()
    {
      return Players[0].GameRules.Join(Players[1].GameRules.Data);
    }
  }
}
using System;
using System.Collections.Generic;
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay;
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay.Cell;
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

    public GameRules Rules;
    public List<CellModel> Cells = new List<CellModel>();
    
    public ShapeType LastMove;
    public ShapeType CurrentMove;

    public PlayerInfo GetPlayerInfo(ulong clientId)
    {
      return Players.Find(x => x.PlayerId == clientId);
    }

    public PlayerInfo GetMovingPlayer()
    {
      return Players.Find(x => x.Shape == CurrentMove);
    }

    public void AddPlayer(ulong playerId)
    {
      Players.Add(new PlayerInfo { PlayerId = playerId });
    }
  }
}
using System;
using System.Collections.Generic;
using System.Linq;

namespace CollectiveMind.TicTac3D.Runtime.Server.Session
{
  [Serializable]
  public class SessionRegistry
  {
    public List<GameSession> Sessions = new List<GameSession>();

    public GameSession GetWaitingSession()
    {
      return Sessions.Find(x => x.Status == SessionState.Waiting);
    }

    public GameSession GetSessionByPlayerId(ulong playerId)
    {
      return Sessions.Find(x => x.Players.Any(y => y.PlayerId == playerId));
    }
  }
}
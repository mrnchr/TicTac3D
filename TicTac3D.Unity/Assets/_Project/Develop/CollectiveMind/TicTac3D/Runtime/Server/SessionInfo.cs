using System;
using System.Collections.Generic;
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay.Cell;

namespace CollectiveMind.TicTac3D.Runtime.Server
{
  [Serializable]
  public class SessionInfo
  {
    public List<SessionInfoAboutClient> ClientInfos = new List<SessionInfoAboutClient>();
    public ShapeType CurrentMove;

    public SessionInfoAboutClient GetClientInfo(ulong clientId)
    {
      return ClientInfos.Find(x => x.Client.ClientId == clientId);
    }

    public SessionInfoAboutClient GetMovingPlayer()
    {
      return ClientInfos.Find(x => x.Shape == CurrentMove);
    }
  }
}
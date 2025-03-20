using System;
using System.Collections.Generic;

namespace CollectiveMind.TicTac3D.Runtime.Server
{
  [Serializable]
  public class SessionInfo
  {
    public List<SessionInfoAboutClient> ClientInfos = new List<SessionInfoAboutClient>();

    public SessionInfoAboutClient GetClientInfo(ulong clientId)
    {
      return ClientInfos.Find(x => x.Client.ClientId == clientId);
    }
  }
}
using System;
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay.Cell;
using Unity.Netcode;

namespace CollectiveMind.TicTac3D.Runtime.Server
{
  [Serializable]
  public class SessionInfoAboutClient
  {
    public NetworkClient Client;
    public ShapeType Shape;
  }
}
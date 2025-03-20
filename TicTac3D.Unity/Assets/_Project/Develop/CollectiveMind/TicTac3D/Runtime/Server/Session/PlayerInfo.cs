using System;
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay.Cell;

namespace CollectiveMind.TicTac3D.Runtime.Server.Session
{
  [Serializable]
  public class PlayerInfo
  {
    public ulong PlayerId;
    public ShapeType Shape;
  }
}
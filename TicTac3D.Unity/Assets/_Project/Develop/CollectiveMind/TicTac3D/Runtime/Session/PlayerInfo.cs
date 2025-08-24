using System;
using CollectiveMind.TicTac3D.Runtime.Gameplay;

namespace CollectiveMind.TicTac3D.Runtime.Session
{
  [Serializable]
  public class PlayerInfo
  {
    public ulong PlayerId;
    public ShapeType Shape;
    
    public GameRules GameRules;
  }
}
using System;
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay.Shape;

namespace CollectiveMind.TicTac3D.Runtime.Shared.Gameplay
{
  [Serializable]
  public class GameRules
  {
    public ShapeType DesiredShape; 
    public int BotMoveCount;
    public float MoveTime;
    public ShapeFadingType ShapeFading;
    public int FadingMoveCount;

    public GameRules Clone()
    {
      return MemberwiseClone() as GameRules;
    }
  }
}
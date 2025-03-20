using System;

namespace CollectiveMind.TicTac3D.Runtime.Shared.Gameplay
{
  [Serializable]
  public class GameContext
  {
    public int BotMoveCount;
    public float MoveTime;
    public ShapeFadingType ShapeFading;
    public int FadingMoveCount;
  }
}
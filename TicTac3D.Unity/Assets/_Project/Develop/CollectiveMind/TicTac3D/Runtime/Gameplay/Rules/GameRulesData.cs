using System;
using Unity.Netcode;

namespace CollectiveMind.TicTac3D.Runtime.Gameplay
{
  [Serializable]
  public struct GameRulesData : INetworkSerializeByMemcpy
  {
    public ShapeType DesiredShape; 
    public int BotMoveCount;
    public float MoveTime;
    public ShapeFadingType ShapeFading;
    public int BotFadingMoveCount;
    public int PlayerFadingMoveCount;

    public static GameRulesData CreateRandom()
    {
      return new GameRulesData
      {
        DesiredShape = ShapeType.XO,
        BotMoveCount = -1,
        MoveTime = -1,
        ShapeFading = ShapeFadingType.None,
        BotFadingMoveCount = -1,
        PlayerFadingMoveCount = -1
      };
    }
    
    public static bool Match(GameRulesData left, GameRulesData right)
    {
      return left.Match(right);
    }
    
    public bool Match(GameRulesData rules)
    {
      return MatchDesiredShape(rules.DesiredShape)
        && MatchMoveCount(rules.BotMoveCount)
        && MatchMoveTime(rules.MoveTime)
        && MatchShapeFading(rules.ShapeFading)
        && MatchFadingMoveCount(BotFadingMoveCount, rules.BotFadingMoveCount, ShapeFading, rules.ShapeFading)
        && MatchFadingMoveCount(PlayerFadingMoveCount, rules.PlayerFadingMoveCount, ShapeFading, rules.ShapeFading);
    }

    private bool MatchDesiredShape(ShapeType shape)
    {
      return (DesiredShape, shape) switch
      {
        (ShapeType.XO, not ShapeType.None) => true,
        (not ShapeType.None, ShapeType.XO) => true,
        (ShapeType.X, ShapeType.O) => true,
        (ShapeType.O, ShapeType.X) => true,
        _ => false
      };
    }

    private bool MatchMoveCount(int count)
    {
      return count < 0 || BotMoveCount < 0 || count == BotMoveCount;
    }

    private bool MatchMoveTime(float time)
    {
      return time < 0 || MoveTime < 0 || time == MoveTime;
    }

    private bool MatchShapeFading(ShapeFadingType fading)
    {
      return ShapeFading == ShapeFadingType.None || fading == ShapeFadingType.None || fading == ShapeFading;
    }

    private bool MatchFadingMoveCount(int botLeft, int botRight, ShapeFadingType leftFading, ShapeFadingType rightFading)
    {
      int l = leftFading == ShapeFadingType.None ? -1 : botLeft;
      int r = rightFading == ShapeFadingType.None ? -1 : botRight;
      return l < 0 || r < 0 || l == r;
    }
  }
}
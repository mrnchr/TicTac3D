using System;
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay.Shape;
using Unity.Netcode;

namespace CollectiveMind.TicTac3D.Runtime.Shared.Gameplay.Rules
{
  [Serializable]
  public struct GameRulesData : INetworkSerializeByMemcpy
  {
    public ShapeType DesiredShape; 
    public int BotMoveCount;
    public float MoveTime;
    public ShapeFadingType ShapeFading;
    public int FadingMoveCount;

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
        && MatchFadingMoveCount(FadingMoveCount, rules.FadingMoveCount, ShapeFading, rules.ShapeFading);
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

    private bool MatchFadingMoveCount(int left, int right, ShapeFadingType leftFading, ShapeFadingType rightFading)
    {
      int l = leftFading == ShapeFadingType.None ? -1 : left;
      int r = rightFading == ShapeFadingType.None ? -1 : right;
      return l < 0 || r < 0 || l == r;
    }
  }
}
using System;
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay.Shape;
using TriInspector;

namespace CollectiveMind.TicTac3D.Runtime.Shared.Gameplay
{
  [Serializable]
  public class GameRules
  {
    [InlineProperty]
    [HideLabel]
    public GameRulesData Data;

    public GameRules Clone()
    {
      return MemberwiseClone() as GameRules;
    }

    public bool Match(GameRules rules)
    {
      return Match(rules.Data);
    }
    
    public bool Match(GameRulesData rules)
    {
      return MatchDesiredShape(rules.DesiredShape)
        && MatchMoveCount(rules.BotMoveCount)
        && MatchMoveTime(rules.MoveTime)
        && MatchShapeFading(rules.ShapeFading)
        && MatchFadingMoveCount(rules.FadingMoveCount);
    }

    private bool MatchDesiredShape(ShapeType shape)
    {
      return (Data.DesiredShape, shape) switch
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
      return count < 0 || Data.BotMoveCount < 0 || count == Data.BotMoveCount;
    }

    private bool MatchMoveTime(float time)
    {
      return time < 0 || Data.MoveTime < 0 || time == Data.MoveTime;
    }

    private bool MatchShapeFading(ShapeFadingType fading)
    {
      return Data.ShapeFading == ShapeFadingType.None || fading == ShapeFadingType.None || fading == Data.ShapeFading;
    }

    private bool MatchFadingMoveCount(int count)
    {
      return Data.FadingMoveCount < 0 || count < 0 || count == Data.FadingMoveCount;
    }
  }
}
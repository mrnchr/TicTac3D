using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using TriInspector;
using UnityEngine;

namespace CollectiveMind.TicTac3D.Runtime.Gameplay
{
  [CreateAssetMenu(menuName = CAC.Names.GAME_CONFIG_MENU, fileName = "GameConfig")]
  public class GameConfig : ScriptableObject
  {
    public GameRulesData DefaultRules;

    public ShapeFadingType UnifiedFading;

    [ValidateInput("ValidateSeparateFading")]
    public ShapeFadingType SeparateFading;

    public ShapeFadingType OverridenSeparateFading => SeparateFading & ~UnifiedFading & ~ShapeFadingType.Off;

    public List<ShapeType> AvailableShapes = new List<ShapeType>();
    public List<int> AvailableBotMoveCounts = new List<int>();
    public List<float> AvailableMoveTimes = new List<float>();
    public List<ShapeFadingType> AvailableShapeFadings = new List<ShapeFadingType>();
    public List<int> AvailableBotFadingMoveCounts = new List<int>();
    public List<int> AvailablePlayerFadingMoveCounts = new List<int>();

    public List<TRule> GetAvailableRule<TRule>(GameRuleType type)
    {
      return type switch
      {
        GameRuleType.DesiredShape when AvailableShapes is List<TRule> shapes => shapes,
        GameRuleType.BotMoveCount when AvailableBotMoveCounts is List<TRule> moveCounts => moveCounts,
        GameRuleType.MoveTime when AvailableMoveTimes is List<TRule> moveTimes => moveTimes,
        GameRuleType.ShapeFading when AvailableShapeFadings is List<TRule> shapeFadings => shapeFadings,
        GameRuleType.BotFadingMoveCount when AvailableBotFadingMoveCounts is List<TRule> fadingMoveCounts => fadingMoveCounts,
        GameRuleType.PlayerFadingMoveCount when AvailablePlayerFadingMoveCounts is List<TRule> fadingMoveCounts => fadingMoveCounts,
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
      };
    }

#if UNITY_EDITOR
    [UsedImplicitly]
    private TriValidationResult ValidateSeparateFading()
    {
      if (OverridenSeparateFading != SeparateFading)
        return TriValidationResult.Error(
          $"{UnityEditor.ObjectNames.NicifyVariableName(nameof(UnifiedFading))} overrides some values of "
          + $"{UnityEditor.ObjectNames.NicifyVariableName(nameof(SeparateFading))}. "
          + $"The values of {UnityEditor.ObjectNames.NicifyVariableName(nameof(UnifiedFading))} will be applied.");
      
      return TriValidationResult.Valid;
    }
#endif
  }
}
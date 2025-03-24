using System;
using System.Collections.Generic;
using TriInspector;
using UnityEngine;

namespace CollectiveMind.TicTac3D.Runtime.Shared.Gameplay.Shape
{
  [CreateAssetMenu(menuName = CAC.Names.SHAPE_CONFIG_MENU, fileName = CAC.Names.SHAPE_CONFIG_FILE)]
  public class ShapeConfig : ScriptableObject
  {
    [SerializeField]
    [ListDrawerSettings(ShowElementLabels = true)]
    private List<ShapeTuple> _shapeColors;

    public ShapeTuple GetDataForShape(ShapeType shape)
    {
      return _shapeColors.Find(x => x.Shape == shape);
    }
  }

  [Serializable]
  public struct ShapeTuple
  {
    public ShapeType Shape;
    public Color Color;
    public Sprite TimerSprite;
  }
}
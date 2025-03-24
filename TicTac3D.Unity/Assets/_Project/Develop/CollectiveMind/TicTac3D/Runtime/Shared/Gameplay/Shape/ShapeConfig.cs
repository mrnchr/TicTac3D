using System;
using System.Collections.Generic;
using UnityEngine;

namespace CollectiveMind.TicTac3D.Runtime.Shared.Gameplay.Shape
{
  [CreateAssetMenu(menuName = CAC.Names.SHAPE_CONFIG_MENU, fileName = CAC.Names.SHAPE_CONFIG_FILE)]
  public class ShapeConfig : ScriptableObject
  {
    [SerializeField]
    private List<ShapeColorTuple> _shapeColors;

    public Color GetColorForShape(ShapeType shape)
    {
      return _shapeColors.Find(x => x.Shape == shape).Color;
    }
  }

  [Serializable]
  public struct ShapeColorTuple
  {
    public ShapeType Shape;
    public Color Color;
  }
}
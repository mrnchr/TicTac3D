using UnityEngine;

namespace CollectiveMind.TicTac3D.Runtime.Gameplay
{
  [CreateAssetMenu(menuName = CAC.Names.ROTATION_CONFIG_MENU, fileName = "RotationConfig")]
  public class RotationConfig : ScriptableObject
  {
    public float Smoothing = 1;
  }
}
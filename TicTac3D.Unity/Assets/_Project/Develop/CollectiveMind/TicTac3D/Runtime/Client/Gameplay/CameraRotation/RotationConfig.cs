using CollectiveMind.TicTac3D.Runtime.Shared;
using UnityEngine;

namespace CollectiveMind.TicTac3D.Runtime.Client.Gameplay.CameraRotation
{
  [CreateAssetMenu(menuName = CAC.Names.ROTATION_CONFIG_MENU, fileName = CAC.Names.ROTATION_CONFIG_FILE)]
  public class RotationConfig : ScriptableObject
  {
    public float Smoothing = 1;
  }
}
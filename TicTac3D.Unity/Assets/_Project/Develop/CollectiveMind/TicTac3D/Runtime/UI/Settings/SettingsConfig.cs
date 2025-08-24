using UnityEngine;

namespace CollectiveMind.TicTac3D.Runtime.UI
{
  [CreateAssetMenu(menuName = CAC.Names.SETTINGS_CONFIG_MENU, fileName = "SettingsConfig")]
  public class SettingsConfig : ScriptableObject
  {
    public SettingsData DefaultSettings;
    public Vector2 MouseSensitivityLimit;
  }
}
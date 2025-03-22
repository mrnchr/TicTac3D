using CollectiveMind.TicTac3D.Runtime.Shared;
using UnityEngine;

namespace CollectiveMind.TicTac3D.Runtime.Client.UI.Settings
{
  [CreateAssetMenu(menuName = CAC.Names.SETTINGS_CONFIG_MENU, fileName = CAC.Names.SETTINGS_CONFIG_FILE)]
  public class SettingsConfig : ScriptableObject
  {
    public SettingsData DefaultSettings;
    public Vector2 MouseSensitivityLimit;
  }
}
using UnityEngine;
using UnityEngine.Localization;

namespace CollectiveMind.TicTac3D.Runtime.UI
{
  [CreateAssetMenu(menuName = CAC.CONFIG_MENU + "Lobby Settings", fileName = nameof(LobbySettingsConfig))]
  public class LobbySettingsConfig : ScriptableObject
  {
    public LocalizedString SearchText;
    public LocalizedString CreateLobbyText;
    public LocalizedString JoinLobbyText;
  }
}
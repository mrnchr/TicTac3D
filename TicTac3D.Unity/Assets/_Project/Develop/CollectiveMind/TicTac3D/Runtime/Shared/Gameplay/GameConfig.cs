using UnityEngine;

namespace CollectiveMind.TicTac3D.Runtime.Shared.Gameplay
{
  [CreateAssetMenu(menuName = CAC.Names.GAME_CONFIG_MENU, fileName = CAC.Names.GAME_CONFIG_FILE)]
  public class GameConfig : ScriptableObject
  {
    public GameRules DefaultRules;
  }
}
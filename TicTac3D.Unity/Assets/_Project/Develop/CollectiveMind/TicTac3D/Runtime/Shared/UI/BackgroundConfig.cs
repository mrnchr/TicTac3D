using System.Collections.Generic;
using UnityEngine;

namespace CollectiveMind.TicTac3D.Runtime.Shared.UI
{
  [CreateAssetMenu(menuName = CAC.Names.BACKGROUND_CONFIG_MENU, fileName = CAC.Names.BACKGROUND_CONFIG_FILE)]
  public class BackgroundConfig : ScriptableObject
  {
    public List<Sprite> Backgrounds;
  }
}
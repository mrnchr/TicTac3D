using System.Collections.Generic;
using UnityEngine;

namespace CollectiveMind.TicTac3D.Runtime.UI
{
  [CreateAssetMenu(menuName = CAC.Names.BACKGROUND_CONFIG_MENU, fileName = "BackgroundConfig")]
  public class BackgroundConfig : ScriptableObject
  {
    public List<Sprite> Backgrounds;
  }
}
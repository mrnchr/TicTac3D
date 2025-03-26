using CollectiveMind.TicTac3D.Runtime.Shared;
using UnityEngine;

namespace CollectiveMind.TicTac3D.Runtime.Client.SFX
{
  [CreateAssetMenu(menuName = CAC.Names.SOUND_CONFIG_MENU, fileName = CAC.Names.SOUND_CONFIG_FILE)]
  public class SoundConfig : ScriptableObject
  {
    public AudioClip ClickSound;
  }
}
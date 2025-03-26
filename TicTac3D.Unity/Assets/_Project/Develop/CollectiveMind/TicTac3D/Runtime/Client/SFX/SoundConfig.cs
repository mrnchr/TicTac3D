using System;
using System.Collections.Generic;
using CollectiveMind.TicTac3D.Runtime.Shared;
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay.Shape;
using UnityEngine;

namespace CollectiveMind.TicTac3D.Runtime.Client.SFX
{
  [CreateAssetMenu(menuName = CAC.Names.SOUND_CONFIG_MENU, fileName = CAC.Names.SOUND_CONFIG_FILE)]
  public class SoundConfig : ScriptableObject
  {
    public AudioClip ClickSound;
    
    [SerializeField]
    private List<ShapeAudioTuple> _shapeSounds;

    public AudioClip GetShapeSound(ShapeType shape)
    {
      return _shapeSounds.Find(x => x.Shape == shape)?.Clip;
    }
    
    [Serializable]
    private class ShapeAudioTuple
    {
      public ShapeType Shape;
      public AudioClip Clip;
    }
  }
}
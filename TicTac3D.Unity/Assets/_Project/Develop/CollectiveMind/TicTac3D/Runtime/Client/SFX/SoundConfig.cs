using System;
using System.Collections.Generic;
using CollectiveMind.TicTac3D.Runtime.Shared;
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay.Rules;
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay.Shape;
using TriInspector;
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
    
    [SerializeField]
    private List<ResultAudioTuple> _resultSounds;

    public AudioClip GetResultSound(GameResultType result)
    {
      return _resultSounds.Find(x => x.Result == result)?.Clip;
    }
    
    [Serializable]
    [DeclareHorizontalGroup(nameof(ShapeAudioTuple))]
    private class ShapeAudioTuple
    {
      [GroupNext(nameof(ShapeAudioTuple))]
      [HideLabel]
      public ShapeType Shape;
      [HideLabel]
      public AudioClip Clip;
    }

    [Serializable]
    [DeclareHorizontalGroup(nameof(ResultAudioTuple))]
    private class ResultAudioTuple
    {
      [GroupNext(nameof(ResultAudioTuple))]
      [HideLabel]
      public GameResultType Result;
      [HideLabel]
      public AudioClip Clip;
    }
  }
}
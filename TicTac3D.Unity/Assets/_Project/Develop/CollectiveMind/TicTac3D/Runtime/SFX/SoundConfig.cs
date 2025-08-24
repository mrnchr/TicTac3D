using System;
using System.Collections.Generic;
using CollectiveMind.TicTac3D.Runtime.Gameplay;
using TriInspector;
using UnityEngine;

namespace CollectiveMind.TicTac3D.Runtime.SFX
{
  [CreateAssetMenu(menuName = CAC.Names.SOUND_CONFIG_MENU, fileName = "SoundConfig")]
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
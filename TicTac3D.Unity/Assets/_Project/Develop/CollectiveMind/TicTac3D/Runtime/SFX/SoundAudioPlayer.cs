using System;
using UnityEngine;

namespace CollectiveMind.TicTac3D.Runtime.SFX
{
  public class SoundAudioPlayer : ISoundAudioPlayer
  {
    public event Action<AudioClip> OnSoundPlaying;

    public void PlaySound(AudioClip audioClip)
    {
      OnSoundPlaying?.Invoke(audioClip);
    }
  }
}
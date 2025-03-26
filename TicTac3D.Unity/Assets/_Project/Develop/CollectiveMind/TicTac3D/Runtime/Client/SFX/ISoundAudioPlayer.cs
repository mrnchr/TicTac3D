using System;
using UnityEngine;

namespace CollectiveMind.TicTac3D.Runtime.Client.SFX
{
  public interface ISoundAudioPlayer
  {
    event Action<AudioClip> OnSoundPlaying;
    void PlaySound(AudioClip audioClip);
  }
}
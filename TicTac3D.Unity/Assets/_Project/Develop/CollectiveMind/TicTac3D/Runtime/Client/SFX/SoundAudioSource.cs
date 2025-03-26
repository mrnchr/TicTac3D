using UnityEngine;
using Zenject;

namespace CollectiveMind.TicTac3D.Runtime.Client.SFX
{
  public class SoundAudioSource : MonoBehaviour
  {
    private AudioSource _audioSource;
    private ISoundAudioPlayer _player;

    [Inject]
    public void Construct(ISoundAudioPlayer player)
    {
      _player = player;
      _audioSource = GetComponent<AudioSource>();
    }

    private void Awake()
    {
      _player.OnSoundPlaying += PlaySound;
    }

    private void PlaySound(AudioClip audioClip)
    {
      _audioSource.PlayOneShot(audioClip);
    }
  }
}
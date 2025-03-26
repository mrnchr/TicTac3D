using CollectiveMind.TicTac3D.Runtime.Client.SFX;
using CollectiveMind.TicTac3D.Runtime.Client.WindowManagement;
using CollectiveMind.TicTac3D.Runtime.Shared.AssetManagement;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace CollectiveMind.TicTac3D.Runtime.Client.UI
{
  public class ButtonSoundPlayer : MonoBehaviour
  {
    private ISoundAudioPlayer _soundAudioPlayer;
    private IConfigLoader _configLoader;
    private SoundConfig _config;
    private Button _button;

    [Inject]
    public void Construct(ISoundAudioPlayer soundAudioPlayer, IConfigLoader configLoader)
    {
      _soundAudioPlayer = soundAudioPlayer;
      _configLoader = configLoader;
      _config = configLoader.LoadConfig<SoundConfig>();
      _button = GetComponent<Button>();
      
      _button.AddListener(PlaySound);
    }

    private void PlaySound()
    {
      _soundAudioPlayer.PlaySound(_config.ClickSound);
    }

    private void OnDestroy()
    {
      _button.RemoveListener(PlaySound);
      _configLoader.UnloadConfig<SoundConfig>();
    }
  }
}
using CollectiveMind.TicTac3D.Runtime.Client.WindowManagement;
using CollectiveMind.TicTac3D.Runtime.Shared.AssetManagement;
using CollectiveMind.TicTac3D.Runtime.Shared.Utils;
using R3;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace CollectiveMind.TicTac3D.Runtime.Client.UI.Settings
{
  public class SettingsWindow : BaseWindow
  {
    [SerializeField]
    private Slider _soundVolumeSlider;

    [SerializeField]
    private Slider _musicVolumeSlider;

    [SerializeField]
    private Slider _mouseSensitivitySlider;

    [SerializeField]
    private Button _continueButton;

    [SerializeField]
    private Button _backButton;

    private IWindowManager _windowManager;
    private IConfigLoader _configLoader;
    private SettingsDataProvider _settingsDataProvider;
    private SettingsConfig _config;

    [Inject]
    public void Construct(IWindowManager windowManager,
      IConfigLoader configLoader,
      SettingsDataProvider settingsDataProvider)
    {
      _windowManager = windowManager;
      _configLoader = configLoader;
      _settingsDataProvider = settingsDataProvider;
      _config = configLoader.LoadConfig<SettingsConfig>();

      _settingsDataProvider.Data.SoundVolume.Subscribe(ChangeSoundVolumeSlider);
      _settingsDataProvider.Data.MusicVolume.Subscribe(ChangeMusicVolumeSlider);
      _settingsDataProvider.Data.MouseSensitivity.Subscribe(ChangeMouseSensitivitySlider);

      _soundVolumeSlider.AddListener(ChangeSoundVolume);
      _musicVolumeSlider.AddListener(ChangeMusicVolume);
      _mouseSensitivitySlider.AddListener(ChangeMouseSensitivity);

      _continueButton.AddListener(CloseWindow);
      _backButton.AddListener(Back);
    }

    private void Start()
    {
      ChangeSoundVolumeSlider(_settingsDataProvider.Data.SoundVolume.Value);
      ChangeMusicVolumeSlider(_settingsDataProvider.Data.MusicVolume.Value);
      if (_mouseSensitivitySlider)
      {
        _mouseSensitivitySlider.minValue = _config.MouseSensitivityLimit.x;
        _mouseSensitivitySlider.maxValue = _config.MouseSensitivityLimit.y;
        ChangeMouseSensitivitySlider(_settingsDataProvider.Data.MouseSensitivity.Value);
      }
    }

    private void ChangeSoundVolumeSlider(float value)
    {
      _soundVolumeSlider.ObjOrNull()?.SetValueWithoutNotify(value);
    }

    private void ChangeMusicVolumeSlider(float value)
    {
      _musicVolumeSlider.ObjOrNull()?.SetValueWithoutNotify(value);
    }

    private void ChangeMouseSensitivitySlider(float value)
    {
      _mouseSensitivitySlider.ObjOrNull()?.SetValueWithoutNotify(value);
    }

    private void ChangeSoundVolume(float value)
    {
      _settingsDataProvider.Data.SoundVolume.Value = value;
    }

    private void ChangeMusicVolume(float value)
    {
      _settingsDataProvider.Data.MusicVolume.Value = value;
    }

    private void ChangeMouseSensitivity(float value)
    {
      _settingsDataProvider.Data.MouseSensitivity.Value = value;
    }


    private void CloseWindow()
    {
      _windowManager.CloseWindow<SettingsWindow>();
    }

    private void Back()
    {
      CloseWindow();
    }

    private void OnDestroy()
    {
      _soundVolumeSlider.RemoveListener(ChangeSoundVolume);
      _musicVolumeSlider.RemoveListener(ChangeMusicVolume);
      _mouseSensitivitySlider.RemoveListener(ChangeMouseSensitivity);

      _continueButton.RemoveListener(Back);
      _backButton.RemoveListener(CloseWindow);
      _configLoader.UnloadConfig<SettingsConfig>();
    }
  }
}
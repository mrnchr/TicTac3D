using System;
using CollectiveMind.TicTac3D.Runtime.Shared.AssetManagement;
using R3;
using UnityEngine;
using UnityEngine.Audio;
using Zenject;

namespace CollectiveMind.TicTac3D.Runtime.Client.UI.Settings
{
  public class SettingsApplier : IInitializable, IDisposable
  {
    private const string SOUNDS_VOLUME = "SoundsVolume";
    private const string MUSIC_VOLUME = "MusicVolume";

    private readonly SettingsDataProvider _settingsDataProvider;
    private readonly IConfigLoader _configLoader;
    private readonly AudioMixer _audioMixer;
    private readonly SettingsConfig _config;

    public SettingsApplier(SettingsDataProvider settingsDataProvider, IConfigLoader configLoader, AudioMixer audioMixer)
    {
      _settingsDataProvider = settingsDataProvider;
      _configLoader = configLoader;
      _audioMixer = audioMixer;
      _config = configLoader.LoadConfig<SettingsConfig>();

      _settingsDataProvider.Data.SoundVolume.Subscribe(ChangeSoundVolume);
      _settingsDataProvider.Data.MusicVolume.Subscribe(ChangeMusicVolume);
    }

    public void Initialize()
    {
      _settingsDataProvider.Data.Copy(_config.DefaultSettings);
    }

    private void ChangeSoundVolume(float soundVolume)
    {
      _audioMixer.SetFloat(SOUNDS_VOLUME, ReflectVolume(soundVolume));
    }

    private void ChangeMusicVolume(float musicVolume)
    {
      _audioMixer.SetFloat(MUSIC_VOLUME, ReflectVolume(musicVolume));
    }

    private float ReflectVolume(float volume)
    {
      return Mathf.Log10(volume) * 20;
    }

    public void Dispose()
    {
      _configLoader.UnloadConfig<SettingsConfig>();
    }
  }
}
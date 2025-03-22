using System;
using CollectiveMind.TicTac3D.Runtime.Shared.AssetManagement;
using R3;
using Zenject;

namespace CollectiveMind.TicTac3D.Runtime.Client.UI.Settings
{
  public class SettingsApplier : IInitializable, IDisposable
  {
    private readonly SettingsDataProvider _settingsDataProvider;
    private readonly IConfigLoader _configLoader;
    private readonly SettingsConfig _config;

    public SettingsApplier(SettingsDataProvider settingsDataProvider, IConfigLoader configLoader)
    {
      _settingsDataProvider = settingsDataProvider;
      _configLoader = configLoader;
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
    }

    private void ChangeMusicVolume(float musicVolume)
    {
      
    }

    public void Dispose()
    {
      _configLoader.UnloadConfig<SettingsConfig>();
    }
  }
}
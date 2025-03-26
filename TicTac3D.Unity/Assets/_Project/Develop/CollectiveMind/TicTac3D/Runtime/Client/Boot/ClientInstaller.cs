using CollectiveMind.TicTac3D.Runtime.Client.Gameplay;
using CollectiveMind.TicTac3D.Runtime.Client.Input;
using CollectiveMind.TicTac3D.Runtime.Client.SFX;
using CollectiveMind.TicTac3D.Runtime.Client.UI.Settings;
using CollectiveMind.TicTac3D.Runtime.Shared.Boot;
using UnityEngine;
using Zenject;

namespace CollectiveMind.TicTac3D.Runtime.Client.Boot
{
  public class ClientInstaller : Installer<ClientInstaller>
  {
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Entry()
    {
      InstallerBridge.Subscribe<ProjectInstaller>(Install);
    }

    public override void InstallBindings()
    {
      InstallInput();
      InstallSettings();

      BindGameRulesProvider();
      BindSoundAudioPlayer();
    }

    private void InstallInput()
    {
      InputInstaller.Install(Container);
    }

    private void InstallSettings()
    {
      SettingsInstaller.Install(Container);
    }

    private void BindGameRulesProvider()
    {
      Container
        .Bind<GameRulesProvider>()
        .AsSingle();
    }

    private void BindSoundAudioPlayer()
    {
      Container
        .BindInterfacesTo<SoundAudioPlayer>()
        .AsSingle();
    }
  }
}
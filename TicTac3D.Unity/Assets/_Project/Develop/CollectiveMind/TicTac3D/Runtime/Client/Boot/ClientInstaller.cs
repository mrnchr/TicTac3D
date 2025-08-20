using CollectiveMind.TicTac3D.Runtime.Client.Gameplay;
using CollectiveMind.TicTac3D.Runtime.Client.Input;
using CollectiveMind.TicTac3D.Runtime.Client.LobbyManagement;
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
      InputInstaller.Install(Container);
      SettingsInstaller.Install(Container);

      Container
        .Bind<LobbyManager>()
        .AsSingle();

      Container
        .Bind<GameRulesProvider>()
        .AsSingle();

      Container
        .BindInterfacesTo<SoundAudioPlayer>()
        .AsSingle();

      Container
        .BindInterfacesTo<ClientInitializer>()
        .AsSingle();
    }
  }
}
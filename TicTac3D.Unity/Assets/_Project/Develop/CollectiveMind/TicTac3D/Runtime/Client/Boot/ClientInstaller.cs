using CollectiveMind.TicTac3D.Runtime.Client.Gameplay;
using CollectiveMind.TicTac3D.Runtime.Client.Input;
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

      BindGameRulesProvider();
    }

    private void InstallInput()
    {
      InputInstaller.Install(Container);
    }

    private void BindGameRulesProvider()
    {
      Container
        .Bind<GameRulesProvider>()
        .AsSingle();
    }
  }
}
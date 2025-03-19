using CollectiveMind.TicTac3D.Runtime.Shared.Boot;
using UnityEngine;
using Zenject;

namespace CollectiveMind.TicTac3D.Runtime.Server.Boot
{
  public class ServerInstaller : Installer<ServerInstaller>
  {
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Entry()
    {
      InstallerBridge.Subscribe<ProjectInstaller>(Install);
    }

    public override void InstallBindings()
    {
      BindGameStarter();
    }

    private void BindGameStarter()
    {
      Container
        .BindInterfacesTo<GameStarter>()
        .AsSingle()
        .NonLazy();
    }
  }
}
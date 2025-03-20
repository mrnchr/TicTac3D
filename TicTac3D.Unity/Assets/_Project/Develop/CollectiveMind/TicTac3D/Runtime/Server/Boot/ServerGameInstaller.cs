using CollectiveMind.TicTac3D.Runtime.Shared.Boot;
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay.Boot;
using UnityEngine;
using Zenject;

namespace CollectiveMind.TicTac3D.Runtime.Server.Boot
{
  public class ServerGameInstaller : Installer<ServerGameInstaller>
  {
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Entry()
    {
      InstallerBridge.Subscribe<GameInstaller>(Install);
    }

    public override void InstallBindings()
    {
      BindGameStarter();
      BindFieldCreator();
    }

    private void BindGameStarter()
    {
      Container
        .BindInterfacesTo<GameStarter>()
        .AsSingle()
        .NonLazy();
    }

    private void BindFieldCreator()
    {
      Container
        .BindInterfacesTo<FieldCreator>()
        .AsSingle();
    }
  }
}
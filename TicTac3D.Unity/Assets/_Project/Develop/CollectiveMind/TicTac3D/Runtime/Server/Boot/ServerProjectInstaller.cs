using CollectiveMind.TicTac3D.Runtime.Server.Session;
using CollectiveMind.TicTac3D.Runtime.Shared.Boot;
using UnityEngine;
using Zenject;

namespace CollectiveMind.TicTac3D.Runtime.Server.Boot
{
  public class ServerProjectInstaller : Installer<ServerProjectInstaller>
  {
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Entry()
    {
      InstallerBridge.Subscribe<ProjectInstaller>(Install);
    }
    
    public override void InstallBindings()
    {
      BindClientManager();
      BindGameStarter();
      BindSessionRegistry();
      BindGameRuleProcessor();
    }

    private void BindClientManager()
    {
      Container
        .BindInterfacesTo<PlayerManager>()
        .AsSingle();
    }

    private void BindGameStarter()
    {
      Container
        .Bind<IGameStarter>()
        .To<GameStarter>()
        .AsSingle();
    }

    private void BindSessionRegistry()
    {
      Container
        .Bind<SessionRegistry>()
        .AsSingle();
    }

    private void BindGameRuleProcessor()
    {
      Container
        .BindInterfacesTo<GameRulesProcessor>()
        .AsSingle();
    }
  }
}
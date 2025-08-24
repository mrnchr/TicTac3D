using Zenject;

namespace CollectiveMind.TicTac3D.Runtime.GameStateComponents
{
  public class GameStateComponentsInstaller : Installer<GameStateComponentsInstaller>
  {
    public override void InstallBindings()
    {
      Container
        .Bind<IGameStateFactory>()
        .To<GameStateFactory>()
        .AsSingle();
      
      Container
        .Bind<IGameStateMachine>()
        .To<GameStateMachine>()
        .AsSingle();
      
      Container
        .Bind<GameStateMachineInitializer>()
        .AsSingle();
    }
  }
}
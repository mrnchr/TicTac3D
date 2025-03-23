using Zenject;

namespace CollectiveMind.TicTac3D.Runtime.Client.GameStateComponents
{
  public class GameStateComponentsInstaller : Installer<GameStateComponentsInstaller>
  {
    public override void InstallBindings()
    {
      BindGameStateFactory();
      BindGameStateMachine();
      BindGameStateMachineInitializer();
    }

    private void BindGameStateFactory()
    {
      Container
        .Bind<IGameStateFactory>()
        .To<GameStateFactory>()
        .AsSingle();
    }

    private void BindGameStateMachine()
    {
      Container
        .Bind<IGameStateMachine>()
        .To<GameStateMachine>()
        .AsSingle();
    }

    private void BindGameStateMachineInitializer()
    {
      Container
        .BindInterfacesTo<GameStateMachineInitializer>()
        .AsSingle();
    }
  }
}
using Zenject;

namespace CollectiveMind.TicTac3D.Runtime.Input
{
  public class InputInstaller : Installer<InputInstaller>
  {
    public override void InstallBindings()
    {
      Container
        .Bind<InputProvider>()
        .AsSingle();
      
      Container
        .BindInterfacesAndSelfTo<PlayerInputActions>()
        .AsSingle();
      
      Container
        .BindInterfacesTo<InputHandler>()
        .AsSingle();
    }
  }
}
using Zenject;

namespace CollectiveMind.TicTac3D.Runtime.WindowManagement
{
  public class WindowInstaller : Installer<WindowInstaller>
  {
    public override void InstallBindings()
    {
      Container
        .Bind<IWindowManager>()
        .To<WindowManager>()
        .AsSingle();
      Container
        .BindInterfacesTo<WindowInitializer>()
        .AsSingle();
    }
  }
}
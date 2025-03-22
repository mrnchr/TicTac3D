using Zenject;

namespace CollectiveMind.TicTac3D.Runtime.Client.WindowManagement.Boot
{
  public class WindowInstaller : Installer<WindowInstaller>
  {
    public override void InstallBindings()
    {
      BindWindowManager();
      BindWindowInitializer();
    }

    private void BindWindowManager()
    {
      Container
        .Bind<IWindowManager>()
        .To<WindowManager>()
        .AsSingle();
    }

    private void BindWindowInitializer()
    {
      Container
        .BindInterfacesTo<WindowInitializer>()
        .AsSingle();
    }
  }
}
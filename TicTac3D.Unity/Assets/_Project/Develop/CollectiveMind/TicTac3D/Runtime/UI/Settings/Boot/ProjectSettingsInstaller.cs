using Zenject;

namespace CollectiveMind.TicTac3D.Runtime.UI
{
  public class SettingsInstaller : Installer<SettingsInstaller>
  {
    public override void InstallBindings()
    {
      Container
        .Bind<SettingsDataProvider>()
        .AsSingle();
      
      Container
        .BindInterfacesTo<SettingsApplier>()
        .AsSingle();
    }
  }
}
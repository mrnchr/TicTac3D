using Zenject;

namespace CollectiveMind.TicTac3D.Runtime.Client.UI.Settings
{
  public class SettingsInstaller : Installer<SettingsInstaller>
  {
    public override void InstallBindings()
    {
      BindSettingsDataProvider();
      BindSettingsApplier();
    }

    private void BindSettingsDataProvider()
    {
      Container
        .Bind<SettingsDataProvider>()
        .AsSingle();
    }

    private void BindSettingsApplier()
    {
      Container
        .BindInterfacesTo<SettingsApplier>()
        .AsSingle();
    }
  }
}
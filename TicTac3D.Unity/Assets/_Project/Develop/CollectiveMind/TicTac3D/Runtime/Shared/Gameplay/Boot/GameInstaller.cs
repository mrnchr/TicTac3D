using CollectiveMind.TicTac3D.Runtime.Shared.Boot;
using Zenject;

namespace CollectiveMind.TicTac3D.Runtime.Shared.Gameplay.Boot
{
  public class GameInstaller : MonoInstaller
  {
    public override void InstallBindings()
    {
      InstallGameInstallerBridge();
    }

    private void InstallGameInstallerBridge()
    {
      InstallerBridge.Install<GameInstaller>(Container);
    }
  }
}
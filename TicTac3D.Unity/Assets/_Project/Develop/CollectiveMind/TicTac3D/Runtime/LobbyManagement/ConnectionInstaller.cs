using Zenject;

namespace CollectiveMind.TicTac3D.Runtime.LobbyManagement
{
  public class ConnectionInstaller : Installer<ConnectionInstaller>
  {
    public override void InstallBindings()
    {
      Container
        .Bind<ConnectionInfo>()
        .AsSingle();

      Container
        .Bind<LobbyHelper>()
        .AsSingle();
    }
  }
}
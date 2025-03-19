using Unity.Netcode;
using Zenject;

namespace CollectiveMind.TicTac3D.Runtime.Client.Boot
{
  public class ProjectInstaller : MonoInstaller
  {
    public override void InstallBindings()
    {
      BindNetworkManager();
    }

    private void BindNetworkManager()
    {
      var network = FindAnyObjectByType<NetworkManager>();
      Container
        .BindInstance(network)
        .AsSingle();
    }
  }
}
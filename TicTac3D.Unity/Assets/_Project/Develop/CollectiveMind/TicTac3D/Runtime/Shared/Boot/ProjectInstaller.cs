using Unity.Netcode;
using Zenject;

namespace CollectiveMind.TicTac3D.Runtime.Shared.Boot
{
  public class ProjectInstaller : MonoInstaller
  {
    public override void InstallBindings()
    {
      BindNetworkManager();
      BindRpcProvider();

      BindNetworkBus();

      BridgeProjectInstaller();
    }

    private void BindNetworkManager()
    {
      var network = FindAnyObjectByType<NetworkManager>();
      Container
        .BindInstance(network)
        .AsSingle();
    }

    private void BindRpcProvider()
    {
      Container
        .Bind<IRpcProvider>()
        .To<RpcProvider>()
        .AsSingle();
    }

    private void BindNetworkBus()
    {
      Container
        .Bind<INetworkBus>()
        .To<NetworkBus>()
        .AsSingle();
    }

    private void BridgeProjectInstaller()
    {
      InstallerBridge.Install<ProjectInstaller>(Container);
    }
  }
}
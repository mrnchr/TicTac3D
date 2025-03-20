using CollectiveMind.TicTac3D.Runtime.Shared.AssetManagement;
using CollectiveMind.TicTac3D.Runtime.Shared.Network;
using Unity.Netcode;
using Zenject;

namespace CollectiveMind.TicTac3D.Runtime.Shared.Boot
{
  public class ProjectInstaller : MonoInstaller
  {
    public override void InstallBindings()
    {
      BindConfigLoader();
      BindPrefabLoader();
      BindPrefabFactory();
      
      BindNetworkManager();
      BindRpcProvider();

      BindNetworkBus();

      InstallProjectInstallerBridge();
    }

    private void BindConfigLoader()
    {
      Container
        .BindInterfacesTo<ConfigLoader>()
        .AsSingle();
    }

    private void BindPrefabLoader()
    {
      Container
        .BindInterfacesTo<PrefabLoader>()
        .AsSingle();
    }

    private void BindPrefabFactory()
    {
      Container
        .BindInterfacesTo<PrefabFactory>()
        .AsSingle();
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

    private void InstallProjectInstallerBridge()
    {
      InstallerBridge.Install<ProjectInstaller>(Container);
    }
  }
}
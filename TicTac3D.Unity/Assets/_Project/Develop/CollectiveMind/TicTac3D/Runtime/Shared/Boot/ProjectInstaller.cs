using CollectiveMind.TicTac3D.Runtime.Shared.AssetManagement;
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay.Cell;
using CollectiveMind.TicTac3D.Runtime.Shared.Network;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace CollectiveMind.TicTac3D.Runtime.Shared.Boot
{
  public class ProjectInstaller : MonoInstaller
  {
    [SerializeField]
    private EventSystem _eventSystem;

    public override void InstallBindings()
    {
      BindEventSystem();

      BindNetworkInitializer();

      BindConfigLoader();
      BindPrefabLoader();
      BindPrefabFactory();

      BindNetworkManager();
      BindRpcProvider();

      BindNetworkBus();

      BindCellModelFactory();
      BindCellCreator();
      BindCellModelListMonitor();

      InstallProjectInstallerBridge();
    }

    private void BindEventSystem()
    {
      Container
        .BindInstance(_eventSystem)
        .AsSingle();
    }

    private void BindNetworkInitializer()
    {
      Container
        .BindInterfacesTo<NetworkInitializer>()
        .AsSingle();
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

    private void BindCellModelFactory()
    {
      Container
        .BindInterfacesTo<CellModelFactory>()
        .AsSingle();
    }

    private void BindCellCreator()
    {
      Container
        .BindInterfacesTo<CellCreator>()
        .AsSingle();
    }

    private void BindCellModelListMonitor()
    {
      Container
        .Bind<CellListMonitor>()
        .FromNewComponentOnNewGameObject()
        .WithGameObjectName(nameof(CellListMonitor))
        .AsCached()
        .NonLazy();
    }

    private void InstallProjectInstallerBridge()
    {
      InstallerBridge.Install<ProjectInstaller>(Container);
    }
  }
}
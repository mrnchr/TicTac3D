using System.Collections.Generic;
using CollectiveMind.TicTac3D.Runtime.Shared.Boot;
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay.Cell;
using Zenject;

namespace CollectiveMind.TicTac3D.Runtime.Shared.Gameplay.Boot
{
  public class GameInstaller : MonoInstaller
  {
    public override void InstallBindings()
    {
      BindCellModelList();
      BindCellModelFactory();

      BindGameInitializer();

      BindCellModelListMonitor();
      
      InstallGameInstallerBridge();
    }

    private void BindCellModelList()
    {
      Container
        .Bind<List<CellModel>>()
        .AsSingle();
    }

    private void BindCellModelFactory()
    {
      Container
        .Bind<CellModelFactory>()
        .AsSingle();
    }

    private void BindGameInitializer()
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
        .WithGameObjectName("CellListMonitor")
        .AsCached()
        .NonLazy();
    }

    private void InstallGameInstallerBridge()
    {
      InstallerBridge.Install<GameInstaller>(Container);
    }
  }
}
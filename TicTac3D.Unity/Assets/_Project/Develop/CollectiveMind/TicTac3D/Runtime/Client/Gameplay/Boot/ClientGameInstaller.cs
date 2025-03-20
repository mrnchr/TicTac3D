using System.Collections.Generic;
using CollectiveMind.TicTac3D.Runtime.Client.Gameplay.Cell;
using CollectiveMind.TicTac3D.Runtime.Client.Gameplay.Shape;
using CollectiveMind.TicTac3D.Runtime.Shared.Boot;
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay.Boot;
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay.Cell;
using UnityEngine;
using Zenject;

namespace CollectiveMind.TicTac3D.Runtime.Client.Gameplay
{
  public class ClientGameInstaller : Installer<ClientGameInstaller>
  {
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Entry()
    {
      InstallerBridge.Subscribe<GameInstaller>(Install);
    }

    public override void InstallBindings()
    {
      BindCellVisualFactory();
      BindCellModelList();
      BindFieldCreator();

      BindCellRaycaster();
      BindCellSelector();
      BindCellShapeUpdater();

      BindShapeFactory();

      BindGameInfo();
      BindCurrentMoveChanger();
      BindCellVisualList();

      BindFieldCleaner();
    }

    private void BindCellVisualFactory()
    {
      Container
        .BindInterfacesTo<CellVisualFactory>()
        .AsSingle();
    }

    private void BindCellModelList()
    {
      Container
        .Bind<List<CellModel>>()
        .AsSingle();
    }

    private void BindFieldCreator()
    {
      Container
        .BindInterfacesTo<FieldCreator>()
        .AsSingle();
    }

    private void BindCellRaycaster()
    {
      Container
        .BindInterfacesTo<CellRaycaster>()
        .AsSingle();
    }

    private void BindCellSelector()
    {
      Container
        .BindInterfacesTo<CellSelector>()
        .AsSingle();
    }

    private void BindCellShapeUpdater()
    {
      Container
        .BindInterfacesTo<CellShapeUpdater>()
        .AsSingle();
    }

    private void BindShapeFactory()
    {
      Container
        .BindInterfacesTo<ShapeFactory>()
        .AsSingle();
    }

    private void BindGameInfo()
    {
      Container
        .Bind<GameInfo>()
        .AsSingle();
    }

    private void BindCurrentMoveChanger()
    {
      Container
        .BindInterfacesTo<CurrentMoveChanger>()
        .AsSingle();
    }

    private void BindCellVisualList()
    {
      Container
        .Bind<List<CellVisual>>()
        .AsSingle();
    }

    private void BindFieldCleaner()
    {
      Container
        .BindInterfacesTo<FieldCleaner>()
        .AsSingle();
    }
  }
}
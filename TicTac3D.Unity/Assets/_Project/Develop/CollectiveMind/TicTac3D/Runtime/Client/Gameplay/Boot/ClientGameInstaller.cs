using System.Collections.Generic;
using CollectiveMind.TicTac3D.Runtime.Client.Gameplay.Cell;
using CollectiveMind.TicTac3D.Runtime.Client.Gameplay.Shape;
using CollectiveMind.TicTac3D.Runtime.Client.GameStateComponents;
using CollectiveMind.TicTac3D.Runtime.Client.UI.SetShape;
using CollectiveMind.TicTac3D.Runtime.Client.WindowManagement.Boot;
using CollectiveMind.TicTac3D.Runtime.Shared.Boot;
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay.Boot;
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay.Cell;
using CollectiveMind.TicTac3D.Runtime.Shared.Network;
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
      BindGameplayTickableManager();

      InstallWindow();
      InstallGameStateComponents();

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

      BindSetShapeFacade();

      BindMoveTimeFollower();

      BindGameInitializer();
    }

    private void BindGameplayTickableManager()
    {
      Container
        .BindInterfacesTo<GameplayTickableManager>()
        .AsSingle();
    }

    private void InstallWindow()
    {
      WindowInstaller.Install(Container);
    }

    private void InstallGameStateComponents()
    {
      GameStateComponentsInstaller.Install(Container);
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

    private void BindSetShapeFacade()
    {
      Container
        .Bind<ConfirmationContext>()
        .AsSingle();
    }

    private void BindMoveTimeFollower()
    {
      Container
        .BindInterfacesTo<MoveTimeFollower>()
        .AsSingle();
    }

    private void BindGameInitializer()
    {
      if (NetworkRole.IsClient)
        Container
          .BindInterfacesTo<GameInitializer>()
          .AsSingle();
    }
  }
}
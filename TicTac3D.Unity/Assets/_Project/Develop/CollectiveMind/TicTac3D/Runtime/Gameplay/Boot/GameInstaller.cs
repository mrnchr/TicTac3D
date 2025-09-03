using System.Collections.Generic;
using CollectiveMind.TicTac3D.Runtime.GameStateComponents;
using CollectiveMind.TicTac3D.Runtime.UI;
using CollectiveMind.TicTac3D.Runtime.WindowManagement;
using Zenject;

namespace CollectiveMind.TicTac3D.Runtime.Gameplay
{
  public class GameInstaller : MonoInstaller
  {
    public override void InstallBindings()
    {
      Container
        .BindInterfacesTo<GameplayTickableManager>()
        .AsSingle();

      WindowInstaller.Install(Container);
      GameStateComponentsInstaller.Install(Container);

      Container
        .BindInterfacesTo<CellVisualFactory>()
        .AsSingle();
      Container
        .Bind<List<CellModel>>()
        .AsSingle();
      Container
        .BindInterfacesTo<FieldCreator>()
        .AsSingle();

      Container
        .BindInterfacesTo<CellRaycaster>()
        .AsSingle();
      Container
        .BindInterfacesTo<CellSelector>()
        .AsSingle();
      Container
        .BindInterfacesTo<CellShapeUpdater>()
        .AsSingle();

      Container
        .BindInterfacesTo<ShapeFactory>()
        .AsSingle();

      Container
        .Bind<GameInfo>()
        .AsSingle();
      Container
        .BindInterfacesTo<CurrentMoveChanger>()
        .AsSingle();
      Container
        .Bind<List<CellVisual>>()
        .AsSingle();

      Container
        .BindInterfacesTo<GameFinish>()
        .AsSingle();

      Container
        .Bind<ConfirmationContext>()
        .AsSingle();

      Container
        .BindInterfacesTo<MoveTimeFollower>()
        .AsSingle();

      Container
        .BindInterfacesTo<ShapeSetter>()
        .AsSingle();
      
      Container
        .BindInterfacesTo<GameInitializer>()
        .AsSingle();
    }
  }
}
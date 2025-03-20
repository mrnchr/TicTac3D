using CollectiveMind.TicTac3D.Runtime.Client.Gameplay.Cell;
using CollectiveMind.TicTac3D.Runtime.Shared.Boot;
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay.Boot;
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
      BindFieldCreator();
    }

    private void BindCellVisualFactory()
    {
      Container
        .BindInterfacesTo<CellVisualFactory>()
        .AsSingle();
    }

    private void BindFieldCreator()
    {
      Container
        .BindInterfacesTo<FieldCreator>()
        .AsSingle();
    }
  }
}
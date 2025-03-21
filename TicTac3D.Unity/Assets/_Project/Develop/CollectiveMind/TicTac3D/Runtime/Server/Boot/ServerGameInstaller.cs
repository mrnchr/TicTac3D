using CollectiveMind.TicTac3D.Runtime.Shared.Boot;
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay.Boot;
using CollectiveMind.TicTac3D.Runtime.Shared.Network;
using UnityEngine;
using Zenject;

namespace CollectiveMind.TicTac3D.Runtime.Server.Boot
{
  public class ServerGameInstaller : Installer<ServerGameInstaller>
  {
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Entry()
    {
      if (NetworkRole.IsServer)
        InstallerBridge.Subscribe<GameInstaller>(Install);
    }

    public override void InstallBindings()
    {
      BindShapeSetter();
    }

    private void BindShapeSetter()
    {
      Container
        .BindInterfacesTo<ShapeSetter>()
        .AsSingle();
    }
  }
}
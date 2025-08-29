using CollectiveMind.TicTac3D.Runtime.AssetManagement;
using CollectiveMind.TicTac3D.Runtime.Gameplay;
using CollectiveMind.TicTac3D.Runtime.Input;
using CollectiveMind.TicTac3D.Runtime.LobbyManagement;
using CollectiveMind.TicTac3D.Runtime.Network;
using CollectiveMind.TicTac3D.Runtime.Session;
using CollectiveMind.TicTac3D.Runtime.SFX;
using CollectiveMind.TicTac3D.Runtime.UI;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using Zenject;

namespace CollectiveMind.TicTac3D.Runtime.Boot
{
  public class ProjectInstaller : MonoInstaller
  {
    [SerializeField]
    private EventSystem _eventSystem;

    [SerializeField]
    private AudioMixer _audioMixer;

    public override void InstallBindings()
    {
      Container
        .BindInstance(_eventSystem)
        .AsSingle();
      Container
        .BindInstance(_audioMixer)
        .AsSingle();

      Container
        .BindInterfacesTo<ConfigLoader>()
        .AsSingle();
      Container
        .BindInterfacesTo<PrefabLoader>()
        .AsSingle();
      Container
        .BindInterfacesTo<PrefabFactory>()
        .AsSingle();

      var network = FindAnyObjectByType<NetworkManager>();
      Container
        .BindInstance(network)
        .AsSingle();
      Container
        .Bind<IRpcProvider>()
        .To<RpcProvider>()
        .AsSingle();

      Container
        .Bind<INetworkBus>()
        .To<NetworkBus>()
        .AsSingle();

      Container
        .BindInterfacesTo<CellModelFactory>()
        .AsSingle();
      Container
        .BindInterfacesTo<CellCreator>()
        .AsSingle();

#if UNITY_EDITOR
      Container
        .Bind<CellListMonitor>()
        .FromNewComponentOnNewGameObject()
        .WithGameObjectName(nameof(CellListMonitor))
        .AsSingle()
        .NonLazy();
#endif

      InputInstaller.Install(Container);
      SettingsInstaller.Install(Container);
      ConnectionInstaller.Install(Container);
      
      Container
        .BindInterfacesAndSelfTo<LobbyManager>()
        .AsSingle();

      Container
        .Bind<GameRulesProvider>()
        .AsSingle();

      Container
        .BindInterfacesTo<SoundAudioPlayer>()
        .AsSingle();

      Container
        .BindInterfacesTo<PlayerManager>()
        .AsSingle();
      Container
        .Bind<IGameStarter>()
        .To<GameStarter>()
        .AsSingle();
      Container
        .Bind<SessionProvider>()
        .AsSingle();
      Container
        .BindInterfacesTo<GameRulesProcessor>()
        .AsSingle();
      Container
        .Bind<IBotBrain>()
        .To<BotBrain>()
        .AsSingle();
      Container
        .BindInterfacesTo<MoveTimeUpdater>()
        .AsSingle();

#if UNITY_EDITOR
      Container
        .Bind<SessionRegistryMonitor>()
        .FromNewComponentOnNewGameObject()
        .WithGameObjectName(nameof(SessionRegistryMonitor))
        .AsCached()
        .NonLazy();
#endif

      Container
        .BindInterfacesAndSelfTo<ProjectInitializer>()
        .AsSingle();
    }
  }
}
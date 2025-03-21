using System;
using Unity.Multiplayer;
using Unity.Netcode;
using Zenject;

namespace CollectiveMind.TicTac3D.Runtime.Shared.Boot
{
  public class NetworkInitializer : IInitializable
  {
    private NetworkManager _networkManager;

    public NetworkInitializer(NetworkManager networkManager)
    {
      _networkManager = networkManager;
    }
    
    public void Initialize()
    {
      switch (MultiplayerRolesManager.ActiveMultiplayerRoleMask)
      {
        case MultiplayerRoleFlags.Client:
          _networkManager.StartClient();
          break;
        case MultiplayerRoleFlags.Server:
          _networkManager.StartServer();
          break;
        case MultiplayerRoleFlags.ClientAndServer:
          _networkManager.StartHost();
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
    }
  }
}
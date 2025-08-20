using CollectiveMind.TicTac3D.Runtime.Client.LobbyManagement;
using Zenject;

namespace CollectiveMind.TicTac3D.Runtime.Client.Boot
{
  public class ClientInitializer : IInitializable
  {
    private readonly LobbyManager _lobbyManager;

    public ClientInitializer(LobbyManager lobbyManager)
    {
      _lobbyManager = lobbyManager;
    }
    
    public async void Initialize()
    {
      await _lobbyManager.Initialize();
    }
  }
}
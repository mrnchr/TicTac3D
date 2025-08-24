using CollectiveMind.TicTac3D.Runtime.LobbyManagement;
using Zenject;

namespace CollectiveMind.TicTac3D.Runtime.Boot
{
  public class ProjectInitializer : IInitializable
  {
    private readonly LobbyManager _lobbyManager;

    public ProjectInitializer(LobbyManager lobbyManager)
    {
      _lobbyManager = lobbyManager;
    }
    
    public async void Initialize()
    {
      await _lobbyManager.Initialize();
    }
  }
}
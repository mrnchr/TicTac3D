using CollectiveMind.TicTac3D.Runtime.LobbyManagement;
using CollectiveMind.TicTac3D.Runtime.UI;
using Zenject;

namespace CollectiveMind.TicTac3D.Runtime.Boot
{
  public class ProjectInitializer : IInitializable
  {
    private readonly LobbyManager _lobbyManager;
    private readonly SettingsApplier _settingsApplier;

    public ProjectInitializer(LobbyManager lobbyManager, SettingsApplier settingsApplier)
    {
      _lobbyManager = lobbyManager;
      _settingsApplier = settingsApplier;
    }
    
    public async void Initialize()
    {
      _settingsApplier.Initialize();
      await _lobbyManager.Initialize();
    }
  }
}
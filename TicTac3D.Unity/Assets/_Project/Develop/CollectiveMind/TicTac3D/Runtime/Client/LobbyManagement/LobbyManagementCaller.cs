using TriInspector;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using UnityEngine;
using Zenject;

namespace CollectiveMind.TicTac3D.Runtime.Client.LobbyManagement
{
  public class LobbyManagementCaller : MonoBehaviour
  {
    private LobbyManager _lobbyManager;

    [Inject]
    public void Construct(LobbyManager lobbyManager)
    {
      _lobbyManager = lobbyManager;
    }

    [Button]
    public async void InitializeLobbyStub()
    {
      if (!AuthenticationService.Instance.IsAuthorized)
      {
        await _lobbyManager.Authorize();
        Debug.Log("Lobby Initialized");
      }
      else
      {
        Debug.Log("Already signed in");
      }
    }

    [Button]
    public async void CollectLobbies()
    {
      await LobbyService.Instance.QueryLobbiesAsync();
    }
  }
}
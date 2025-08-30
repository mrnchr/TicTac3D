using Unity.Services.Lobbies.Models;
using Unity.Services.Relay.Models;

namespace CollectiveMind.TicTac3D.Runtime.LobbyManagement
{
  public class ConnectionInfo
  {
    public Lobby Lobby;
    public string LobbyId;

    public bool CreatedLobby;
    public bool JoinedLobby;

    public bool RelayCodeCreated;
    public Allocation Allocation;
    public string RelayCode;
    public bool IsRelayCodeUpdated;
    
    public bool NeedReconnect;
    public JoinAllocation JoinAllocation;

    public bool IsConnected;

    public bool GameStarted;

    public void ClearAllocation()
    {
      RelayCodeCreated = false;
      Allocation = null;
      RelayCode = null;
      
      NeedReconnect = false;
      JoinAllocation = null;
      IsRelayCodeUpdated = false;
    }

    public void ClearLobby()
    {
      Lobby = null;
      LobbyId = null;
      CreatedLobby = false;
      JoinedLobby = false;
    }

    public void ClearAll()
    {
      IsConnected = false;
      GameStarted = false;
      
      ClearLobby();
      ClearAllocation();
    }
  }
}
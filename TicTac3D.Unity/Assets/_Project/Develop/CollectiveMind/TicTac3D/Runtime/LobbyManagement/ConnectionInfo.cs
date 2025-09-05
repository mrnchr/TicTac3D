using System;
using Unity.Networking.Transport.Relay;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay.Models;

namespace CollectiveMind.TicTac3D.Runtime.LobbyManagement
{
  [Serializable]
  public class ConnectionInfo
  {
    public LobbyInfo ActiveLobby => CreatedLobby.IsActive ? CreatedLobby : JoinedLobby.IsActive ? JoinedLobby : null;

    public LobbyInfo CreatedLobby = new LobbyInfo();
    public LobbyInfo JoinedLobby = new LobbyInfo();

    public bool GameStarted;

    public bool IsActive => ActiveLobby != null;

    public void ClearAllocation()
    {
      CreatedLobby.ClearAllocation();
      JoinedLobby.ClearAllocation();
    }

    public void ClearLobby()
    {
      CreatedLobby.ClearLobby();
      JoinedLobby.ClearLobby();
    }

    public void ClearAll()
    {
      GameStarted = false;

      ClearLobby();
      ClearAllocation();
    }

    [Serializable]
    public class LobbyInfo
    {
      public bool IsActive;
      public string LobbyId;
      public Lobby Lobby;

      public Guid AllocationId;
      public RelayServerData RelayServerData;
        
      public string RelayCode;
      public bool IsRelayCodeUpdated;

      public bool NeedReconnect;

      public void SetLobby(Lobby lobby)
      {
        Lobby = lobby;
        LobbyId = lobby.Id;
      }

      public void SetAllocation(Allocation allocation)
      {
        AllocationId = allocation.AllocationId;
        RelayServerData = allocation.ToRelayServerData("wss");
      }

      public void SetAllocation(JoinAllocation allocation)
      {
        AllocationId = allocation.AllocationId;
        RelayServerData = allocation.ToRelayServerData("wss");
      }

      public void ClearLobby()
      {
        IsActive = false;
        Lobby = null;
        LobbyId = null;
      }

      public void ClearAllocation()
      {
        AllocationId = new Guid();
        RelayCode = null;
        IsRelayCodeUpdated = false;
        NeedReconnect = false;
        RelayServerData = new RelayServerData();
      }

      public void ClearAll()
      {
        ClearLobby();
        ClearAllocation();
      }
    }
  }
}
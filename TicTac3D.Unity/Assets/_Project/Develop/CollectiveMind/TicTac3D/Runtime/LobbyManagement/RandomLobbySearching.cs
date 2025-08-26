using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CollectiveMind.TicTac3D.Runtime.Gameplay;
using CollectiveMind.TicTac3D.Runtime.Network;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

namespace CollectiveMind.TicTac3D.Runtime.LobbyManagement
{
  public class RandomLobbySearching
  {
    private readonly NetworkManager _networkManager;
    private readonly IRpcProvider _rpcProvider;
    private readonly GameRulesProvider _rulesProvider;
    private readonly LobbyManager _lobbyManager;

    private string _lobbyId;
    private Lobby _lobby;
    private ConnectionStatus _status;

    private GameRulesData GameRulesData => _rulesProvider.Rules.Data;

    public RandomLobbySearching(NetworkManager networkManager,
      IRpcProvider rpcProvider,
      GameRulesProvider rulesProvider,
      LobbyManager lobbyManager)
    {
      _networkManager = networkManager;
      _rpcProvider = rpcProvider;
      _rulesProvider = rulesProvider;
      _lobbyManager = lobbyManager;
    }

    public async UniTask<CancelableResult<Lobby>> SearchFreeLobby(CancellationToken token = default(CancellationToken))
    {
      _status = ConnectionStatus.Searching;
      CancelableResult result = await SearchFreeLobbyInternal(token);
      if (!result)
        _status = ConnectionStatus.None;
      return result.Convert(_lobby);
    }

    private async UniTask<CancelableResult> SearchFreeLobbyInternal(
      CancellationToken token = default(CancellationToken))
    {
      Debug.Log("Searching for free lobby");
      if (!await _lobbyManager.SignIn(token))
        return true;

      Debug.Log("Signed in");

      while (_status != ConnectionStatus.Connected)
      {
        if (_status != ConnectionStatus.JoinLobby)
        {
          Debug.Log("Querying lobbies");

          CancelableResult<Lobby> result = await FindMatchedLobby(token);
          if (!result)
            return true;

          if (_status >= ConnectionStatus.JoinLobby)
          {
            Debug.Log("Already joined lobby");
            continue;
          }

          Debug.Log("Checking lobbies");
          if (result.Value != null)
          {
            Lobby lobby = await JoinLobbyById(result.Value.Id);
            if (lobby != null)
            {
              Debug.Log("Joined lobby");
              DeleteConnectionAsHost();

              if (RemovePlayerIfTokenCancelled(lobby.Id, token))
                return true;

              Debug.Log("Delete connection as host");
              _lobbyId = lobby.Id;
              _lobby = lobby;
              _status = ConnectionStatus.WaitJoinCode;

              while (!_lobby.Data.ContainsKey(NC.RELAY_CODE_NAME))
              {
                Debug.Log("Try get join code");
                _lobby = await LobbyService.Instance.GetLobbyAsync(_lobby.Id);
                await UniTask.WaitForSeconds(0.5f, cancellationToken: token);
                if (RemovePlayerIfTokenCancelled(_lobby.Id, token))
                  return true;
              }

              Debug.Log("Got join code");
              if (!await JoinRelayAsync(token))
              {
                RemovePlayerIfTokenCancelled(_lobby.Id, token);
                return true;
              }

              Debug.Log("Lobby connected as client");
            }
          }
        }

        if (_status < ConnectionStatus.CreateLobby)
        {
          Lobby lobby = await LobbyService.Instance.CreateLobbyAsync("LobbyName", 2,
            new CreateLobbyOptions
            {
              Data = new Dictionary<string, DataObject>
              {
                {
                  NC.RULES_NAME,
                  new DataObject(DataObject.VisibilityOptions.Public, JsonConvert.SerializeObject(GameRulesData))
                }
              }
            });

          _lobbyId = lobby.Id;
          _lobby = lobby;
          _status = ConnectionStatus.CreateLobby;

          Debug.Log("Lobby created");

          if (DeleteLobbyFastIfTokenCancelled(token))
          {
            Debug.Log("Lobby deleted");
            return true;
          }

          var callbacks = new LobbyEventCallbacks();
          callbacks.PlayerJoined += CreateConnection;
          await LobbyService.Instance.SubscribeToLobbyEventsAsync(lobby.Id, callbacks);
          if (DeleteLobbyFastIfTokenCancelled(token))
            return true;

          Debug.Log("Subscribed to player joined");
        }

        if (_status >= ConnectionStatus.JoinLobby)
        {
          Debug.Log("Waiting for connection");
          await UniTask.WaitUntil(() => _status == ConnectionStatus.Connected, cancellationToken: token);
          if (DeleteLobbyFastIfTokenCancelled(token))
            return true;
          Debug.Log("Lobby connected as host");
        }
      }

      return false;
    }

    private async UniTask<CancelableResult<Lobby>> FindMatchedLobby(
      CancellationToken token = default(CancellationToken))
    {
      bool wasFirst = false;
      QueryResponse lobbies = null;
      string continuationToken = null;
      while (!wasFirst || lobbies.Results.Count is > 0 and < 100)
      {
        wasFirst = true;
        try
        {
          lobbies = await LobbyService.Instance.QueryLobbiesAsync(CreateQueryLobbiesOptions(continuationToken));
          continuationToken = lobbies.ContinuationToken;
        }
        catch (LobbyServiceException e)
        {
          Debug.Log(e.Message);
          return false;
        }

        if (token.IsCancellationRequested)
        {
          Debug.Log("Token cancelled while querying");
          DeleteConnectionAsHost();
          return true;
        }

        if (_status >= ConnectionStatus.JoinLobby)
          return false;

        Debug.Log("Lobbies queried");
        foreach (Lobby lobby in lobbies.Results)
        {
          var rules = JsonConvert.DeserializeObject<GameRulesData>(lobby.Data[NC.RULES_NAME].Value);
          if (_lobbyId != lobby.Id && lobby.Players.Count > 0 && rules.Match(GameRulesData))
            return lobby;
        }

        await UniTask.WaitForSeconds(0.5f, cancellationToken: token);
      }

      return false;
    }

    private QueryLobbiesOptions CreateQueryLobbiesOptions(string continuationToken)
    {
      var options = new QueryLobbiesOptions
      {
        Count = 100,
        ContinuationToken = continuationToken,
        Filters = new List<QueryFilter>()
      };

      if (_status == ConnectionStatus.CreateLobby)
      {
        options.Filters.Add(new QueryFilter(QueryFilter.FieldOptions.Created, _lobby.Created.ToString("o"),
          QueryFilter.OpOptions.GT));
      }

      return options;
    }

    private void DeleteConnectionAsHost()
    {
      if (_status >= ConnectionStatus.CreateLobby)
      {
        Debug.Log("Deleting lobby");
        DeleteLobby();
        if (_status >= ConnectionStatus.JoinLobby)
        {
          Debug.Log("Deleting connection");
          _networkManager.Shutdown();
        }
      }
    }

    private static async Task<Lobby> JoinLobbyById(string lobbyId)
    {
      try
      {
        return await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId);
      }
      catch (LobbyServiceException)
      {
      }

      return null;
    }

    private bool RemovePlayerIfTokenCancelled(string lobbyId, CancellationToken token = default(CancellationToken))
    {
      if (token.IsCancellationRequested)
      {
        Debug.Log("Removing player");
        _ = LobbyService.Instance.RemovePlayerAsync(lobbyId, AuthenticationService.Instance.PlayerId);
        _lobbyId = "";
        _lobby = null;
        return true;
      }

      return false;
    }

    private async UniTask<CancelableResult> JoinRelayAsync(CancellationToken token = default(CancellationToken))
    {
      if (token.IsCancellationRequested)
        return true;

      Debug.Log("Joining relay");

      string joinCode = _lobby.Data[NC.RELAY_CODE_NAME].Value;
      JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
      if (token.IsCancellationRequested)
        return true;

      Debug.Log("Relay joined");

      _networkManager.GetComponent<UnityTransport>().SetRelayServerData(allocation.ToRelayServerData("wss"));
      _networkManager.StartClient();

      if (!await StartGame(token))
      {
        Debug.Log("Not started");
        _networkManager.Shutdown();
        return true;
      }

      Debug.Log("Game started");

      _status = ConnectionStatus.Connected;
      return false;
    }

    private bool DeleteLobbyFastIfTokenCancelled(CancellationToken token = default(CancellationToken))
    {
      if (token.IsCancellationRequested)
      {
        Debug.Log("Deleting lobby");
        DeleteLobby();
        return true;
      }

      return false;
    }

    private void DeleteLobby()
    {
      LobbyService.Instance.DeleteLobbyAsync(_lobbyId);

      _lobbyId = "";
      _lobby = null;
    }

    private void CreateConnection(List<LobbyPlayerJoined> players)
    {
      CreateConnectionAsync(_lobbyManager.SearchingCancellationToken).Forget();
    }

    private async UniTask CreateConnectionAsync(CancellationToken token = default(CancellationToken))
    {
      if (token.IsCancellationRequested)
        return;

      Debug.Log("Connection creating");

      _status = ConnectionStatus.JoinLobby;

      Allocation allocation = await RelayService.Instance.CreateAllocationAsync(2);
      string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
      
      if (token.IsCancellationRequested)
        return;

      Debug.Log("Join code created");

      Dictionary<string, DataObject> data = _lobby.Data;
      data[NC.RELAY_CODE_NAME] = new DataObject(DataObject.VisibilityOptions.Member, joinCode);
      await LobbyService.Instance.UpdateLobbyAsync(_lobbyId, new UpdateLobbyOptions
      {
        Data = data,
        IsPrivate = true
      });

      if (token.IsCancellationRequested)
        return;

      Debug.Log("Join code updated");
      _networkManager.GetComponent<UnityTransport>().SetRelayServerData(allocation.ToRelayServerData("wss"));
      _networkManager.StartHost();

      if (!await StartGame(token))
      {
        Debug.Log("Game not started");
        _networkManager.Shutdown();
        return;
      }

      Debug.Log("Game started");

      _status = ConnectionStatus.Connected;
    }

    private async UniTask<CancelableResult> StartGame(CancellationToken token = default(CancellationToken))
    {
      await UniTask.WaitUntil(() => _rpcProvider.IsReady, cancellationToken: token);
      if (token.IsCancellationRequested)
        return true;

      Debug.Log("RPC ready");

      _rpcProvider.SendRequest(new StartGameRequest { Rules = GameRulesData });
      return false;
    }

    private enum ConnectionStatus
    {
      None = 0,
      Searching = 1,
      CreateLobby = Searching + 1,
      JoinLobby = CreateLobby + 1,
      WaitJoinCode = JoinLobby + 1,
      Connected = WaitJoinCode + 1
    }
  }
}
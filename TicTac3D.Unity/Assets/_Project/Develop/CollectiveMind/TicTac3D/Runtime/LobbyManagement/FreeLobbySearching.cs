using System.Collections.Generic;
using System.Threading;
using CollectiveMind.TicTac3D.Runtime.Gameplay;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;

namespace CollectiveMind.TicTac3D.Runtime.LobbyManagement
{
  public class FreeLobbySearching
  {
    private readonly NetworkManager _networkManager;
    private readonly GameRulesProvider _rulesProvider;
    private readonly LobbyManager _lobbyManager;
    private readonly LobbyHelper _lobbyHelper;
    private readonly ConnectionInfo _connectionInfo;

    private Lobby _freeLobby;
    private Lobby _createdLobby;
    private bool _isPlayerJoined;

    private GameRulesData GameRulesData => _rulesProvider.Rules.Data;

    public FreeLobbySearching(NetworkManager networkManager,
      GameRulesProvider rulesProvider,
      LobbyManager lobbyManager,
      LobbyHelper lobbyHelper,
      ConnectionInfo connectionInfo)
    {
      _networkManager = networkManager;
      _rulesProvider = rulesProvider;
      _lobbyManager = lobbyManager;
      _lobbyHelper = lobbyHelper;
      _connectionInfo = connectionInfo;
    }

    public async UniTask<AsyncResult> SearchFreeLobby(CancellationToken token = default(CancellationToken))
    {
      _createdLobby = null;
      _freeLobby = null;
      _isPlayerJoined = false;
      AsyncResult result = await SearchFreeLobbyInternal(token);
      return result;
    }

    private async UniTask<AsyncResult> SearchFreeLobbyInternal(CancellationToken token =
      default(CancellationToken))
    {
      if (!await _lobbyManager.SignIn(token))
        return AsyncReturn.Cancel();

      UniTask task = UniTask.WhenAll(FindMatchedLobby(token),
        JoinLobby(token),
        CheckUpdatedRelayCode(token),
        JoinAllocation(token),
        ConnectClient(token),
        CreateLobby(token),
        CheckJoinedPlayer(token),
        CheckCreatedAllocation(token),
        GetRelayCode(token),
        CreateRelayConnection(token),
        StartGame(token));

      await UniTask.WaitUntil(() => _connectionInfo.GameStarted, cancellationToken: token);
      if (token.IsCancellationRequested)
      {
        await task;

        if (!_connectionInfo.CreatedLobby && _createdLobby != null)
          LobbyWrapper.TryDeleteLobbyAsync(_createdLobby.Id, CancellationToken.None).Forget();
        
        _lobbyHelper.LeaveLobby(true).Forget();
        return AsyncReturn.Cancel();
      }

      return AsyncReturn.Ok();
    }

    private async UniTask CreateLobby(CancellationToken token = default(CancellationToken))
    {
      while (!token.IsCancellationRequested)
      {
        await UniTask.WaitUntil(() => _createdLobby == null && !_connectionInfo.JoinedLobby, cancellationToken: token);
        if (token.IsCancellationRequested)
          return;

        AsyncResult<Lobby> result =
          await LobbyWrapper.TryCreateLobbyUntilExitAsync("LobbyName", 2, CreateLobbyOptions(), token);
        if (result.Value != null)
          _createdLobby = result.Value;

        if (token.IsCancellationRequested)
          return;
      }
    }

    private CreateLobbyOptions CreateLobbyOptions()
    {
      return new CreateLobbyOptions
      {
        Data = new Dictionary<string, DataObject>
        {
          {
            NC.RULES_NAME,
            new DataObject(DataObject.VisibilityOptions.Public, JsonConvert.SerializeObject(GameRulesData))
          }
        }
      };
    }

    private async UniTask CheckJoinedPlayer(CancellationToken token = default(CancellationToken))
    {
      while (!token.IsCancellationRequested)
      {
        await UniTask.WaitUntil(() => _createdLobby != null, cancellationToken: token);
        if (token.IsCancellationRequested)
          return;

        AsyncResult<Lobby> result = await LobbyWrapper.TryGetLobbyAsync(_createdLobby.Id, token);
        if (token.IsCancellationRequested)
          return;

        if (result && result.Value != null)
          _isPlayerJoined = result.Value.Players.Count > 1;

        await UniTask.WaitForSeconds(3, cancellationToken: token);
      }
    }

    private async UniTask CheckCreatedAllocation(CancellationToken token = default(CancellationToken))
    {
      while (!token.IsCancellationRequested)
      {
        await UniTask.WaitUntil(() => _createdLobby != null && _isPlayerJoined && _connectionInfo.Allocation == null,
          cancellationToken: token);
        if (token.IsCancellationRequested)
          return;

        _connectionInfo.CreatedLobby = true;
        _connectionInfo.Lobby = _createdLobby;
        _connectionInfo.LobbyId = _createdLobby.Id;

        AsyncResult<Allocation> allocation =
          await RelayWrapper.TryCreateAllocationUntilExitAsync(2, token: token);
        if (token.IsCancellationRequested)
          return;

        _connectionInfo.Allocation = allocation.Value;
      }
    }

    private async UniTask GetRelayCode(CancellationToken token = default(CancellationToken))
    {
      while (!token.IsCancellationRequested)
      {
        await UniTask.WaitUntil(() => _connectionInfo.Allocation != null, cancellationToken: token);
        if (token.IsCancellationRequested)
          return;


        AsyncResult<string> relayCode =
          await RelayWrapper.TryGetJoinCodeUntilExitAsync(_connectionInfo.Allocation.AllocationId, token);
        if (token.IsCancellationRequested)
          return;

        if (relayCode.ReturnCode == (int)RelayExceptionReason.AllocationNotFound)
        {
          _connectionInfo.ClearAllocation();
          continue;
        }

        _connectionInfo.RelayCode = relayCode.Value;
        _connectionInfo.RelayCodeCreated = true;

        if (_networkManager.IsHost)
          _connectionInfo.IsConnected = true;
      }
    }

    private async UniTask CreateRelayConnection(CancellationToken token = default(CancellationToken))
    {
      while (!token.IsCancellationRequested)
      {
        await UniTask.WaitUntil(() => _connectionInfo.RelayCodeCreated, cancellationToken: token);
        if (token.IsCancellationRequested)
          return;

        await UniTask.WhenAll(UpdateRelayCode(token), ConnectHost(token));
        _connectionInfo.RelayCodeCreated = false;
      }
    }

    private async UniTask UpdateRelayCode(CancellationToken token = default(CancellationToken))
    {
      while (!token.IsCancellationRequested)
      {
        await UniTask.WaitUntil(() => _connectionInfo.RelayCodeCreated, cancellationToken: token);
        if (token.IsCancellationRequested)
          return;

        Dictionary<string, DataObject> data = _connectionInfo.Lobby.Data;
        data[NC.RELAY_CODE_NAME] = new DataObject(DataObject.VisibilityOptions.Member, _connectionInfo.RelayCode);
        await LobbyWrapper.TryUpdateLobbyUntilExitAsync(_connectionInfo.LobbyId,
          new UpdateLobbyOptions { Data = data }, token);

        if (token.IsCancellationRequested)
          return;
      }
    }

    private async UniTask ConnectHost(CancellationToken token = default(CancellationToken))
    {
      while (!token.IsCancellationRequested)
      {
        await UniTask.WaitUntil(() => _connectionInfo.RelayCodeCreated, cancellationToken: token);
        if (token.IsCancellationRequested)
          return;

        if (_networkManager.IsHost)
        {
          _networkManager.Shutdown();
          await UniTask.WaitWhile(() => _networkManager.ShutdownInProgress, cancellationToken: token);
          if (token.IsCancellationRequested)
            return;
        }

        _networkManager.GetComponent<UnityTransport>()
          .SetRelayServerData(_connectionInfo.Allocation.ToRelayServerData("wss"));
        _networkManager.StartHost();
      }
    }

    private async UniTask FindMatchedLobby(CancellationToken token = default(CancellationToken))
    {
      while (!token.IsCancellationRequested)
      {
        await UniTask.WaitUntil(
          () => _freeLobby == null && !_connectionInfo.JoinedLobby && !_connectionInfo.CreatedLobby,
          cancellationToken: token);
        if (token.IsCancellationRequested)
          return;

        QueryResponse lobbies;
        string continuationToken = null;
        do
        {
          lobbies = await LobbyWrapper.TryQueryLobbiesUntilExitAsync(CreateQueryLobbiesOptions(continuationToken),
            token);
          continuationToken = lobbies.ContinuationToken;

          if (token.IsCancellationRequested)
            return;

          await UniTask.WaitUntil(() => !_connectionInfo.JoinedLobby && _freeLobby == null,
            cancellationToken: token);
          if (token.IsCancellationRequested)
            return;

          foreach (Lobby lobby in lobbies.Results)
          {
            if (_createdLobby?.Id == lobby.Id || lobby.Players.Count == 0)
              continue;

            var rules = JsonConvert.DeserializeObject<GameRulesData>(lobby.Data[NC.RULES_NAME].Value);
            if (rules.Match(GameRulesData))
              _freeLobby = lobby;
          }

          await UniTask.WaitForSeconds(0.5f, cancellationToken: token);
        } while (!_connectionInfo.JoinedLobby && _freeLobby == null && lobbies.Results.Count == 100
          && !token.IsCancellationRequested);
      }
    }

    private QueryLobbiesOptions CreateQueryLobbiesOptions(string continuationToken)
    {
      var options = new QueryLobbiesOptions
      {
        Count = 100,
        ContinuationToken = continuationToken,
        Filters = new List<QueryFilter>()
      };

      if (_createdLobby != null)
      {
        options.Filters.Add(new QueryFilter(QueryFilter.FieldOptions.Created,
          _createdLobby.Created.ToString("o"),
          QueryFilter.OpOptions.GT));
      }

      return options;
    }

    private async UniTask JoinLobby(CancellationToken token = default(CancellationToken))
    {
      while (!token.IsCancellationRequested)
      {
        await UniTask.WaitUntil(
          () => _freeLobby != null && !_connectionInfo.JoinedLobby && !_connectionInfo.CreatedLobby,
          cancellationToken: token);
        if (token.IsCancellationRequested)
          return;

        AsyncResult<Lobby> result = await LobbyWrapper.TryJoinLobbyUntilExitAsync(_freeLobby.Id, token: token);
        if (_connectionInfo.CreatedLobby)
        {
          LobbyWrapper.TryRemovePlayerAsync(_freeLobby.Id, AuthenticationService.Instance.PlayerId,
            CancellationToken.None).Forget();
          _freeLobby = null;
          continue;
        }

        if (result.Value != null)
        {
          _connectionInfo.LobbyId = result.Value.Id;
          _connectionInfo.Lobby = result.Value;
          _connectionInfo.JoinedLobby = true;
        }

        if (token.IsCancellationRequested)
          return;

        DeleteConnectionAsHost();
      }
    }

    private void DeleteConnectionAsHost()
    {
      if (_createdLobby != null)
      {
        LobbyWrapper.TryDeleteLobbyAsync(_createdLobby.Id).Forget();

        if (_networkManager.IsHost)
          _networkManager.Shutdown();

        _createdLobby = null;
      }
    }

    private async UniTask CheckUpdatedRelayCode(CancellationToken token = default(CancellationToken))
    {
      while (!token.IsCancellationRequested)
      {
        await UniTask.WaitUntil(() => _connectionInfo.JoinedLobby, cancellationToken: token);
        if (token.IsCancellationRequested)
          return;

        AsyncResult<Lobby> lobby = await LobbyWrapper.TryGetLobbyAsync(_connectionInfo.LobbyId, token);
        if (token.IsCancellationRequested)
          return;

        if (lobby.Value == null)
        {
          _connectionInfo.ClearAll();
          continue;
        }

        lobby.Value.Data.TryGetValue(NC.RELAY_CODE_NAME, out DataObject relayCode);
        string match = relayCode?.Value;
        if (match != _connectionInfo.RelayCode)
        {
          _connectionInfo.ClearAllocation();
          _connectionInfo.RelayCode = match;
        }

        await UniTask.WaitForSeconds(3, cancellationToken: token);
      }
    }

    private async UniTask JoinAllocation(CancellationToken token = default(CancellationToken))
    {
      while (!token.IsCancellationRequested)
      {
        await UniTask.WaitUntil(
          () => _connectionInfo.IsRelayCodeUpdated && !string.IsNullOrWhiteSpace(_connectionInfo.RelayCode),
          cancellationToken: token);
        if (token.IsCancellationRequested)
          return;

        _connectionInfo.IsRelayCodeUpdated = false;

        AsyncResult<JoinAllocation> result =
          await RelayWrapper.TryJoinAllocationUntilExitAsync(_connectionInfo.RelayCode, token);
        if (token.IsCancellationRequested)
          return;

        if (result.ReturnCode == (int)RelayExceptionReason.JoinCodeNotFound || _connectionInfo.IsRelayCodeUpdated)
          continue;

        _connectionInfo.JoinAllocation = result.Value;
        _connectionInfo.NeedReconnect = true;
      }
    }

    private async UniTask ConnectClient(CancellationToken token)
    {
      while (!token.IsCancellationRequested)
      {
        await UniTask.WaitUntil(() => _connectionInfo.NeedReconnect, cancellationToken: token);
        if (token.IsCancellationRequested)
          return;

        if (_networkManager.IsClient)
        {
          _networkManager.Shutdown();
          await UniTask.WaitWhile(() => _networkManager.ShutdownInProgress, cancellationToken: token);
          if (token.IsCancellationRequested)
            return;
        }

        _networkManager.GetComponent<UnityTransport>()
          .SetRelayServerData(_connectionInfo.JoinAllocation.ToRelayServerData("wss"));
        _networkManager.StartClient();

        _connectionInfo.NeedReconnect = false;
        _connectionInfo.IsConnected = true;
      }
    }

    private async UniTask StartGame(CancellationToken token = default(CancellationToken))
    {
      while (!token.IsCancellationRequested)
      {
        await UniTask.WaitUntil(() => _connectionInfo.IsConnected && (!_connectionInfo.CreatedLobby || _isPlayerJoined),
          cancellationToken: token);
        if (token.IsCancellationRequested)
          return;

        await _lobbyHelper.StartGame(GameRulesData, token);
        if (token.IsCancellationRequested)
          return;

        _connectionInfo.GameStarted = true;
      }
    }
  }
}
using System;
using System.Collections.Generic;
using System.Threading;
using CollectiveMind.TicTac3D.Runtime.Gameplay;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;

namespace CollectiveMind.TicTac3D.Runtime.LobbyManagement
{
  public class FreeLobbySearching
  {
    private readonly NetworkManager _networkManager;
    private readonly GameRulesProvider _rulesProvider;
    private readonly LobbyManager _lobbyManager;
    private readonly LobbyHelper _lobbyHelper;
    private readonly ConnectionInfo _connectionInfo;

    private GameRulesData GameRulesData => _rulesProvider.Rules.Data;

    private ConnectionInfo.LobbyInfo CreatedLobby => _connectionInfo.CreatedLobby;

    private ConnectionInfo.LobbyInfo JoinedLobby => _connectionInfo.JoinedLobby;

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
      AsyncResult result = await SearchFreeLobbyInternal(token);
      return result;
    }

    private async UniTask<AsyncResult> SearchFreeLobbyInternal(CancellationToken token =
      default(CancellationToken))
    {
      if (!await _lobbyManager.SignIn(token))
        return AsyncReturn.Cancel();

      await _lobbyHelper.DebugWithDelay("Start searching for free lobby");

      UniTask task = UniTask.WhenAll(FindMatchedLobby(token),
          JoinLobby(token),
          _lobbyHelper.CheckUpdatedRelayCode(token),
          _lobbyHelper.JoinAllocation(token),
          _lobbyHelper.ConnectNetwork(JoinedLobby, false, x => x.IsActive && x.NeedReconnect, token),
          _lobbyHelper.StartGameOnClient(GameRulesData, token),
          _lobbyHelper.CreateLobby(false, token),
          _lobbyHelper.CheckJoinedPlayer(token),
          _lobbyHelper.CreateRelayAllocation(token),
          _lobbyHelper.GetRelayCode(token),
          _lobbyHelper.UpdateRelayCode(token),
          _lobbyHelper.ConnectNetwork(CreatedLobby, true, x => x.IsActive && x.NeedReconnect, token),
          _lobbyHelper.SendReadyResponse(token),
          _lobbyHelper.WaitClientReadiness(GameRulesData, token))
        .SuppressCancellationThrow();

      await UniTask.WaitUntil(() => _connectionInfo.GameStarted, cancellationToken: token).SuppressCancellationThrow();
      await _lobbyHelper.DebugWithDelay("Game started");
      if (token.IsCancellationRequested)
      {
        _lobbyHelper.DebugWithDelay("Game cancelled", true).Forget();
        await task;
        _lobbyHelper.DebugWithDelay("Game finished", true).Forget();

        _lobbyHelper.LeaveLobby(true).Forget();
        return AsyncReturn.Cancel();
      }

      return AsyncReturn.Ok();
    }

    private async UniTask FindMatchedLobby(CancellationToken token = default(CancellationToken))
    {
      while (!token.IsCancellationRequested)
      {
        await UniTask.WaitUntil(() => JoinedLobby.Lobby == null && !_connectionInfo.IsActive, cancellationToken: token)
          .SuppressCancellationThrow();
        if (token.IsCancellationRequested)
          return;

        await _lobbyHelper.DebugWithDelay("Searching for free lobby");

        QueryResponse lobbies;
        string continuationToken = null;
        do
        {
          await _lobbyHelper.DebugWithDelay($"Querying lobbies: {continuationToken}");

          DateTime dateTime = DateTime.UtcNow;
          lobbies = await LobbyWrapper.TryQueryLobbiesUntilExitAsync(CreateQueryLobbiesOptions(continuationToken),
            token);
          continuationToken = lobbies.ContinuationToken;

          if (token.IsCancellationRequested)
            return;

          await _lobbyHelper.DebugWithDelay($"Found {lobbies.Results.Count} lobbies");

          await UniTask
            .WaitUntil(() => JoinedLobby.Lobby == null && !_connectionInfo.IsActive, cancellationToken: token)
            .SuppressCancellationThrow();
          if (token.IsCancellationRequested)
            return;

          await _lobbyHelper.DebugWithDelay("Searching for free lobby");
          foreach (Lobby lobby in lobbies.Results)
          {
            if (CreatedLobby.Lobby?.Id == lobby.Id || lobby.Players.Count == 0
              || DateTime.UtcNow - lobby.Players[0].LastUpdated > TimeSpan.FromSeconds(10))
              continue;

            var rules = JsonConvert.DeserializeObject<GameRulesData>(lobby.Data[NC.RULES_NAME].Value);
            if (rules.Match(GameRulesData))
              JoinedLobby.SetLobby(lobby);
          }

          _lobbyHelper.DebugWithDelay($"Is free lobby found: {JoinedLobby.LobbyId}", true).Forget();
          await UniTask.WaitForSeconds(0.5f, cancellationToken: token).SuppressCancellationThrow();
        } while (JoinedLobby.Lobby == null && !_connectionInfo.IsActive && lobbies.Results.Count == 100
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

      if (CreatedLobby.Lobby != null)
      {
        options.Filters.Add(new QueryFilter(QueryFilter.FieldOptions.Created,
          CreatedLobby.Lobby.Created.ToString("o"),
          QueryFilter.OpOptions.LT));
      }

      return options;
    }

    private async UniTask JoinLobby(CancellationToken token = default(CancellationToken))
    {
      while (!token.IsCancellationRequested)
      {
        await UniTask.WaitUntil(() => JoinedLobby.Lobby != null && !_connectionInfo.IsActive, cancellationToken: token)
          .SuppressCancellationThrow();
        if (token.IsCancellationRequested)
          return;

        await _lobbyHelper.DebugWithDelay("Joining lobby");
        AsyncResult<Lobby> result = await LobbyWrapper.TryJoinLobbyUntilExitAsync(JoinedLobby.LobbyId, token: token);
        if (CreatedLobby.IsActive)
        {
          LobbyWrapper.TryRemovePlayerAsync(JoinedLobby.LobbyId, AuthenticationService.Instance.PlayerId,
            CancellationToken.None).Forget();
          JoinedLobby.ClearLobby();
          _lobbyHelper.DebugWithDelay("Lobby joined but created", true).Forget();
          continue;
        }

        if (result.Value != null)
        {
          JoinedLobby.SetLobby(result.Value);
          JoinedLobby.IsActive = true;
        }

        if (token.IsCancellationRequested)
          return;

        _lobbyHelper.DebugWithDelay("Lobby joined", true).Forget();

        DeleteConnectionAsHost();
      }
    }

    private void DeleteConnectionAsHost()
    {
      if (CreatedLobby.Lobby != null)
      {
        LobbyWrapper.TryDeleteLobbyAsync(CreatedLobby.Lobby.Id).Forget();

        if (_networkManager.IsHost)
          _networkManager.Shutdown();

        CreatedLobby.ClearAll();
      }
    }
  }
}
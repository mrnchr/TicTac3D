using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
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
  public class LobbyHelper
  {
    private readonly ConnectionInfo _connectionInfo;
    private readonly IRpcProvider _rpcProvider;
    private readonly NetworkManager _networkManager;
    private readonly INetworkBus _networkBus;
    private readonly GameRulesProvider _rulesProvider;

    private ConnectionInfo.LobbyInfo CreatedLobby => _connectionInfo.CreatedLobby;
    private ConnectionInfo.LobbyInfo JoinedLobby => _connectionInfo.JoinedLobby;
    private GameRulesData GameRulesData => _rulesProvider.Rules.Data;

    public LobbyHelper(ConnectionInfo connectionInfo,
      IRpcProvider rpcProvider,
      NetworkManager networkManager,
      INetworkBus networkBus,
      GameRulesProvider rulesProvider)
    {
      _connectionInfo = connectionInfo;
      _rpcProvider = rpcProvider;
      _networkManager = networkManager;
      _networkBus = networkBus;
      _rulesProvider = rulesProvider;
    }

    public async UniTask CreateLobby(bool isPrivate, CancellationToken token = default(CancellationToken))
    {
      while (!token.IsCancellationRequested)
      {
        await UniTask.WaitUntil(() => CreatedLobby.Lobby == null && !JoinedLobby.IsActive,
            cancellationToken: token)
          .SuppressCancellationThrow();
        if (token.IsCancellationRequested)
          return;

        await DebugWithDelay("Creating lobby");
        AsyncResult<Lobby> result =
          await LobbyWrapper.TryCreateLobbyUntilExitAsync("LobbyName", 2, CreateLobbyOptions(isPrivate), token);
        if (result.Value != null)
          CreatedLobby.SetLobby(result.Value);

        DebugWithDelay("Lobby created", true).Forget();
      }
    }

    private CreateLobbyOptions CreateLobbyOptions(bool isPrivate)
    {
      return new CreateLobbyOptions
      {
        Data = new Dictionary<string, DataObject>
        {
          {
            NC.RULES_NAME,
            new DataObject(DataObject.VisibilityOptions.Public, JsonConvert.SerializeObject(GameRulesData))
          }
        },
        IsPrivate = isPrivate
      };
    }

    public async UniTask CheckJoinedPlayer(CancellationToken token = default(CancellationToken))
    {
      while (!token.IsCancellationRequested)
      {
        await UniTask.WaitUntil(() => CreatedLobby.Lobby != null && !JoinedLobby.IsActive, cancellationToken: token)
          .SuppressCancellationThrow();
        if (token.IsCancellationRequested)
          return;

        await DebugWithDelay("Waiting for player");
        AsyncResult<Lobby> result = await LobbyWrapper.TryGetLobbyAsync(CreatedLobby.Lobby.Id, token);
        if (token.IsCancellationRequested)
          return;

        if (JoinedLobby.IsActive)
          continue;

        int count = 0;
        if (result && result.Value != null)
        {
          foreach (var player in result.Value.Players)
          {
            if (player.Id != AuthenticationService.Instance.PlayerId
              && DateTime.UtcNow - player.LastUpdated > TimeSpan.FromSeconds(10))
              LobbyWrapper.TryRemovePlayerAsync(CreatedLobby.Lobby.Id, player.Id, token).Forget();
            else
              count++;
          }
        }

        CreatedLobby.IsActive = result && result.Value != null && count == 2;

        DebugWithDelay($"Is player joined: {CreatedLobby.IsActive}", true).Forget();
        await UniTask.WaitForSeconds(3, cancellationToken: token).SuppressCancellationThrow();
      }
    }

    public async UniTask CreateRelayAllocation(CancellationToken token = default(CancellationToken))
    {
      while (!token.IsCancellationRequested)
      {
        await UniTask
          .WaitUntil(
            () => CreatedLobby.IsActive && _connectionInfo.CreatedLobby.AllocationId == Guid.Empty,
            cancellationToken: token)
          .SuppressCancellationThrow();
        if (token.IsCancellationRequested)
          return;

        await DebugWithDelay("Creating allocation");
        AsyncResult<Allocation> allocation =
          await RelayWrapper.TryCreateAllocationUntilExitAsync(2, token: token);
        if (token.IsCancellationRequested)
          return;

        DebugWithDelay("Allocation created", true).Forget();

        CreatedLobby.SetAllocation(allocation.Value);
      }
    }

    public async UniTask GetRelayCode(CancellationToken token = default(CancellationToken))
    {
      while (!token.IsCancellationRequested)
      {
        await UniTask.WaitUntil(() => CreatedLobby.AllocationId != Guid.Empty, cancellationToken: token)
          .SuppressCancellationThrow();
        if (token.IsCancellationRequested)
          return;

        await DebugWithDelay("Getting relay code");

        AsyncResult<string> relayCode =
          await RelayWrapper.TryGetJoinCodeUntilExitAsync(CreatedLobby.AllocationId, token);
        if (token.IsCancellationRequested)
          return;

        if (relayCode.ReturnCode == (int)RelayExceptionReason.AllocationNotFound)
        {
          CreatedLobby.ClearAllocation();
          DebugWithDelay("Allocation not found", true).Forget();
          continue;
        }

        if (relayCode.Value != CreatedLobby.RelayCode)
        {
          CreatedLobby.RelayCode = relayCode.Value;
          CreatedLobby.IsRelayCodeUpdated = true;
          CreatedLobby.NeedReconnect = true;
          DebugWithDelay("Relay code received", true).Forget();
        }
      }
    }

    public async UniTask UpdateRelayCode(CancellationToken token = default(CancellationToken))
    {
      while (!token.IsCancellationRequested)
      {
        await UniTask.WaitUntil(() => CreatedLobby.IsRelayCodeUpdated, cancellationToken: token)
          .SuppressCancellationThrow();
        if (token.IsCancellationRequested)
          return;

        await DebugWithDelay("Updating relay code");
        CreatedLobby.IsRelayCodeUpdated = false;

        Dictionary<string, DataObject> data = CreatedLobby.Lobby.Data;
        data[NC.RELAY_CODE_NAME] = new DataObject(DataObject.VisibilityOptions.Member, CreatedLobby.RelayCode);
        await LobbyWrapper.TryUpdateLobbyUntilExitAsync(CreatedLobby.LobbyId,
          new UpdateLobbyOptions { Data = data }, token);
        DebugWithDelay("Relay code updated", true).Forget();
      }
    }

    public async UniTask CheckUpdatedRelayCode(CancellationToken token = default(CancellationToken))
    {
      while (!token.IsCancellationRequested)
      {
        await UniTask.WaitUntil(() => JoinedLobby.IsActive, cancellationToken: token).SuppressCancellationThrow();
        if (token.IsCancellationRequested)
          return;

        await DebugWithDelay("Checking relay code");

        AsyncResult<Lobby> lobby = await LobbyWrapper.TryGetLobbyAsync(JoinedLobby.LobbyId, token);
        if (token.IsCancellationRequested)
          return;

        if (lobby.Value == null || DateTime.UtcNow - lobby.Value.LastUpdated > TimeSpan.FromSeconds(10))
        {
          LobbyWrapper.TryRemovePlayerAsync(JoinedLobby.LobbyId, AuthenticationService.Instance.PlayerId, token)
            .Forget();
          JoinedLobby.ClearAll();
          DebugWithDelay("Lobby not found", true).Forget();
          continue;
        }

        lobby.Value.Data.TryGetValue(NC.RELAY_CODE_NAME, out DataObject relayCode);
        string match = relayCode?.Value;
        if (match != JoinedLobby.RelayCode)
        {
          JoinedLobby.ClearAllocation();
          JoinedLobby.RelayCode = match;
          JoinedLobby.IsRelayCodeUpdated = true;
        }

        DebugWithDelay("Relay code checked", true).Forget();

        await UniTask.WaitForSeconds(3, cancellationToken: token).SuppressCancellationThrow();
      }
    }

    public async UniTask JoinAllocation(CancellationToken token = default(CancellationToken))
    {
      while (!token.IsCancellationRequested)
      {
        await UniTask
          .WaitUntil(() => JoinedLobby.IsRelayCodeUpdated && !string.IsNullOrWhiteSpace(JoinedLobby.RelayCode),
            cancellationToken: token).SuppressCancellationThrow();
        if (token.IsCancellationRequested)
          return;

        JoinedLobby.IsRelayCodeUpdated = false;

        await DebugWithDelay("Joining allocation");

        AsyncResult<JoinAllocation> result =
          await RelayWrapper.TryJoinAllocationUntilExitAsync(JoinedLobby.RelayCode, token);
        if (token.IsCancellationRequested)
          return;

        if (result.ReturnCode == (int)RelayExceptionReason.JoinCodeNotFound || JoinedLobby.IsRelayCodeUpdated)
          continue;

        DebugWithDelay("Allocation joined", true).Forget();

        JoinedLobby.SetAllocation(result.Value);
        JoinedLobby.NeedReconnect = true;
      }
    }

    public async UniTask ConnectNetwork(ConnectionInfo.LobbyInfo lobbyInfo,
      bool isServer,
      Func<ConnectionInfo.LobbyInfo, bool> whenConnect,
      CancellationToken token = default(CancellationToken))
    {
      while (!token.IsCancellationRequested)
      {
        await UniTask.WaitUntil(() => whenConnect.Invoke(lobbyInfo), cancellationToken: token)
          .SuppressCancellationThrow();
        if (token.IsCancellationRequested)
          return;

        lobbyInfo.NeedReconnect = false;

        await DebugWithDelay("Connecting network");

        if (_networkManager.IsClient || _networkManager.IsServer)
        {
          _networkManager.Shutdown();
          await UniTask.WaitWhile(() => _networkManager.IsClient || _networkManager.IsServer, cancellationToken: token)
            .SuppressCancellationThrow();
          if (token.IsCancellationRequested)
            return;
        }

        _networkManager.GetComponent<UnityTransport>().SetRelayServerData(lobbyInfo.RelayServerData);

        if (isServer)
          _networkManager.StartHost();
        else
          _networkManager.StartClient();

        DebugWithDelay("Network connected", true).Forget();
      }
    }

    public async UniTask WaitClientReadiness(GameRulesData gameRules,
      CancellationToken token = default(CancellationToken))
    {
      while (!token.IsCancellationRequested)
      {
        await DebugWithDelay("Waiting for ready clients...");

        var isClientReady = false;
        _networkBus.SubscribeOnRpc<ClientReadyRequest>(() => isClientReady = true);
        await UniTask.WaitUntil(() => isClientReady, cancellationToken: token).SuppressCancellationThrow();
        _networkBus.UnsubscribeFromRpc<ClientReadyRequest>();

        if (token.IsCancellationRequested)
          return;

        DebugWithDelay("All clients are ready.", true).Forget();

        _rpcProvider.SendRequest(new StartGameRequest { Rules = gameRules });
        _connectionInfo.GameStarted = true;
      }
    }

    public async UniTask SendReadyResponse(CancellationToken token = default(CancellationToken))
    {
      while (!token.IsCancellationRequested)
      {
        await UniTask.WaitUntil(() => _networkManager.IsHost && _rpcProvider.IsReady, cancellationToken: token)
          .SuppressCancellationThrow();
        if (token.IsCancellationRequested)
          return;

        await DebugWithDelay("Sending ready response...");

        _rpcProvider.SendRequest<ServerReadyResponse>(_networkManager.RpcTarget.NotServer);
        await UniTask.WaitUntil(() => _connectionInfo.GameStarted, cancellationToken: token)
          .TimeoutWithoutException(TimeSpan.FromSeconds(10)).SuppressCancellationThrow();
      }
    }

    public async UniTask StartGameOnClient(GameRulesData gameRules,
      CancellationToken token = default(CancellationToken))
    {
      while (!token.IsCancellationRequested)
      {
        await UniTask
          .WaitUntil(() => _networkManager.IsClient && _rpcProvider.IsReady && _networkManager.IsConnectedClient,
            cancellationToken: token).SuppressCancellationThrow();
        if (token.IsCancellationRequested)
          return;

        await DebugWithDelay("Waiting for host...");

        var isServerReady = false;
        _networkBus.SubscribeOnRpc<ServerReadyResponse>(() => isServerReady = true);
        await UniTask.WaitUntil(() => isServerReady || !_networkManager.IsConnectedClient, cancellationToken: token)
          .SuppressCancellationThrow();
        _networkBus.UnsubscribeFromRpc<ServerReadyResponse>();

        if (token.IsCancellationRequested)
          return;

        if (!_networkManager.IsConnectedClient)
          continue;

        DebugWithDelay("Host is ready.", true).Forget();

        _rpcProvider.SendRequest<ClientReadyRequest>(_networkManager.RpcTarget.Server);
        _rpcProvider.SendRequest(new StartGameRequest { Rules = gameRules });
        _connectionInfo.GameStarted = true;
      }
    }

    public async UniTask LeaveLobby(bool forget = false)
    {
      if (_connectionInfo.CreatedLobby.Lobby != null)
      {
        DebugWithDelay("Deleting lobby", true).Forget();
        await LobbyWrapper.TryDeleteLobbyAsync(_connectionInfo.CreatedLobby.LobbyId).TryForget(forget);
      }

      if (_connectionInfo.JoinedLobby.Lobby != null)
      {
        DebugWithDelay("Removing player", true).Forget();
        await LobbyWrapper
          .TryRemovePlayerAsync(_connectionInfo.JoinedLobby.LobbyId, AuthenticationService.Instance.PlayerId)
          .TryForget(forget);
      }

      _networkManager.Shutdown();
      _connectionInfo.ClearAll();
    }

    [HideInCallstack]
    [DebuggerHidden]
    public async UniTask DebugWithDelay(string message, bool forget = false)
    {
      // Debug.Log(message);
      if (forget)
        return;
      await UniTask.Delay(0000).SuppressCancellationThrow();
      // Debug.Log("Continue...");
    }
  }
}
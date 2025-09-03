using System.Collections.Generic;
using System.Threading;
using CollectiveMind.TicTac3D.Runtime.Gameplay;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;

namespace CollectiveMind.TicTac3D.Runtime.LobbyManagement
{
  public class LobbyCreating
  {
    private readonly NetworkManager _networkManager;
    private readonly GameRulesProvider _rulesProvider;
    private readonly LobbyManager _lobbyManager;
    private readonly LobbyHelper _lobbyHelper;
    private readonly ConnectionInfo _connectionInfo;

    private bool _isPlayerJoined;

    public bool IsLobbyCreating { get; private set; }

    private GameRulesData GameRulesData => _rulesProvider.Rules.Data;

    public LobbyCreating(NetworkManager networkManager,
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

    public async UniTask<AsyncResult> CreateLobby(CancellationToken token = default(CancellationToken))
    {
      IsLobbyCreating = true;
      _isPlayerJoined = false;
      AsyncResult result = await CreateLobbyInternal(token);
      IsLobbyCreating = false;
      return result;
    }

    private async UniTask<AsyncResult> CreateLobbyInternal(CancellationToken token = default(CancellationToken))
    {
      if (!await _lobbyManager.SignIn(token))
        return AsyncReturn.Cancel();

      AsyncResult<Lobby> result =
        await LobbyWrapper.TryCreateLobbyUntilExitAsync("LobbyName", 2, CreateLobbyOptions(), token);

      _connectionInfo.LobbyId = result.Value.Id;
      _connectionInfo.Lobby = result.Value;
      _connectionInfo.CreatedLobby = true;

      UniTask task = UniTask.WhenAll(CheckJoinedPlayer(token),
        CreateAllocation(token),
        GetRelayCode(token),
        CreateRelayConnection(token),
        _lobbyHelper.StartGameOnHost(GameRulesData, token)).SuppressCancellationThrow();

      await UniTask.WaitUntil(() => _connectionInfo.GameStarted, cancellationToken: token).SuppressCancellationThrow();
      if (token.IsCancellationRequested)
      {
        await task;
        _lobbyHelper.LeaveLobby(true).Forget();
        return AsyncReturn.Cancel();
      }

      return AsyncReturn.Ok();
    }

    private CreateLobbyOptions CreateLobbyOptions()
    {
      return new CreateLobbyOptions
      {
        Data = new Dictionary<string, DataObject>
        {
          {
            NC.RULES_NAME,
            new DataObject(DataObject.VisibilityOptions.Member, JsonConvert.SerializeObject(GameRulesData))
          }
        },
        IsPrivate = true
      };
    }

    private async UniTask CheckJoinedPlayer(CancellationToken token = default(CancellationToken))
    {
      while (!token.IsCancellationRequested)
      {
        AsyncResult<Lobby> result =
          await LobbyWrapper.TryGetLobbyUntilExitAsync(_connectionInfo.LobbyId, token);

        if (result && result.Value != null)
          _isPlayerJoined = result.Value.Players.Count > 1;

        await UniTask.WaitForSeconds(3, cancellationToken: token).SuppressCancellationThrow();
      }
    }

    private async UniTask CreateAllocation(CancellationToken token = default(CancellationToken))
    {
      while (!token.IsCancellationRequested)
      {
        await UniTask.WaitUntil(() => _isPlayerJoined && _connectionInfo.Allocation == null, cancellationToken: token)
          .SuppressCancellationThrow();
        if (token.IsCancellationRequested)
          return;

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
        await UniTask.WaitUntil(() => _connectionInfo.Allocation != null, cancellationToken: token)
          .SuppressCancellationThrow();
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

    private async UniTask CreateRelayConnection(CancellationToken token)
    {
      while (!token.IsCancellationRequested)
      {
        await UniTask.WaitUntil(() => _connectionInfo.RelayCodeCreated, cancellationToken: token)
          .SuppressCancellationThrow();
        if (token.IsCancellationRequested)
          return;

        _connectionInfo.RelayCodeCreated = false;
        await UniTask.WhenAll(UpdateRelayCode(token), ConnectHost(token)).SuppressCancellationThrow();
      }
    }

    private async UniTask UpdateRelayCode(CancellationToken token = default(CancellationToken))
    {
      Dictionary<string, DataObject> data = _connectionInfo.Lobby.Data;
      data[NC.RELAY_CODE_NAME] = new DataObject(DataObject.VisibilityOptions.Member, _connectionInfo.RelayCode);
      await LobbyWrapper.TryUpdateLobbyUntilExitAsync(_connectionInfo.LobbyId,
        new UpdateLobbyOptions { Data = data }, token);
    }

    private async UniTask ConnectHost(CancellationToken token = default(CancellationToken))
    {
      if (_networkManager.IsHost)
      {
        _networkManager.Shutdown();
        await UniTask.WaitWhile(() => _networkManager.ShutdownInProgress, cancellationToken: token)
          .SuppressCancellationThrow();
        if (token.IsCancellationRequested)
          return;
      }

      _networkManager.GetComponent<UnityTransport>()
        .SetRelayServerData(_connectionInfo.Allocation.ToRelayServerData("wss"));
      _networkManager.StartHost();
    }
  }
}
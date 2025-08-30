using System.Threading;
using CollectiveMind.TicTac3D.Runtime.Gameplay;
using Cysharp.Threading.Tasks;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;

namespace CollectiveMind.TicTac3D.Runtime.LobbyManagement
{
  public class LobbyJoining
  {
    private readonly LobbyHelper _lobbyHelper;
    private readonly LobbyManager _lobbyManager;
    private readonly ConnectionInfo _connectionInfo;
    private readonly NetworkManager _networkManager;

    public LobbyJoining(LobbyHelper lobbyHelper,
      LobbyManager lobbyManager,
      ConnectionInfo connectionInfo,
      NetworkManager networkManager)
    {
      _lobbyHelper = lobbyHelper;
      _lobbyManager = lobbyManager;
      _connectionInfo = connectionInfo;
      _networkManager = networkManager;
    }

    public async UniTask<AsyncResult> JoinLobby(string lobbyCode,
      CancellationToken token = default(CancellationToken))
    {
      AsyncResult result = await JoinLobbyInternal(lobbyCode, token);
      return result;
    }

    private async UniTask<AsyncResult> JoinLobbyInternal(string lobbyCode,
      CancellationToken token = default(CancellationToken))
    {
      if (!await _lobbyManager.SignIn(token))
        return AsyncReturn.Cancel();

      UniTask task = UniTask.WhenAll(JoinLobbyByCode(lobbyCode, token),
        CheckUpdatedRelayCode(token),
        JoinRelay(token),
        ConnectClient(token),
        StartGame(token));

      await UniTask.WaitUntil(() => _connectionInfo.GameStarted, cancellationToken: token);
      if (token.IsCancellationRequested)
      {
        await task;
        _lobbyHelper.LeaveLobby(true).Forget();
        return AsyncReturn.Cancel();
      }

      return AsyncReturn.Ok();
    }

    private async UniTask JoinLobbyByCode(string lobbyCode, CancellationToken token)
    {
      while (!token.IsCancellationRequested)
      {
        await UniTask.WaitUntil(() => !_connectionInfo.JoinedLobby, cancellationToken: token);

        AsyncResult<Lobby> result = await LobbyWrapper.TryJoinLobbyByCodeUntilExitAsync(lobbyCode, token: token);
        if (result.Value != null)
        {
          _connectionInfo.LobbyId = result.Value.Id;
          _connectionInfo.Lobby = result.Value;
          _connectionInfo.JoinedLobby = true;
        }

        if (token.IsCancellationRequested)
          return;
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
          _connectionInfo.IsRelayCodeUpdated = true;
        }

        await UniTask.WaitForSeconds(3, cancellationToken: token);
      }
    }

    private async UniTask JoinRelay(CancellationToken token = default(CancellationToken))
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
        await UniTask.WaitUntil(() => _connectionInfo.IsConnected, cancellationToken: token);
        if (token.IsCancellationRequested)
          return;

        await _lobbyHelper.StartGame(GameRulesData.CreateRandom(), token);
        if (token.IsCancellationRequested)
          return;

        _connectionInfo.GameStarted = true;
      }
    }
  }
}
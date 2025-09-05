using System.Threading;
using CollectiveMind.TicTac3D.Runtime.Gameplay;
using Cysharp.Threading.Tasks;
using Unity.Services.Lobbies.Models;

namespace CollectiveMind.TicTac3D.Runtime.LobbyManagement
{
  public class LobbyJoining
  {
    private readonly LobbyHelper _lobbyHelper;
    private readonly LobbyManager _lobbyManager;
    private readonly ConnectionInfo _connectionInfo;

    private ConnectionInfo.LobbyInfo JoinedLobby => _connectionInfo.JoinedLobby;

    public LobbyJoining(LobbyHelper lobbyHelper,
      LobbyManager lobbyManager,
      ConnectionInfo connectionInfo)
    {
      _lobbyHelper = lobbyHelper;
      _lobbyManager = lobbyManager;
      _connectionInfo = connectionInfo;
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

      await _lobbyHelper.DebugWithDelay("Start joining lobby");

      UniTask task = UniTask.WhenAll(JoinLobbyByCode(lobbyCode, token),
          _lobbyHelper.CheckUpdatedRelayCode(token),
          _lobbyHelper.JoinAllocation(token),
          _lobbyHelper.ConnectNetwork(JoinedLobby, false, x => x.NeedReconnect, token),
          _lobbyHelper.StartGameOnClient(GameRulesData.CreateRandom(), token))
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

    private async UniTask JoinLobbyByCode(string lobbyCode, CancellationToken token)
    {
      while (!token.IsCancellationRequested)
      {
        await UniTask.WaitUntil(() => !JoinedLobby.IsActive, cancellationToken: token)
          .SuppressCancellationThrow();
        if (token.IsCancellationRequested)
          return;

        await _lobbyHelper.DebugWithDelay("Try join lobby");
        
        AsyncResult<Lobby> result = await LobbyWrapper.TryJoinLobbyByCodeUntilExitAsync(lobbyCode, token: token);
        if (result.Value != null)
        {
          JoinedLobby.SetLobby(result.Value);
          JoinedLobby.IsActive = true;
        }
        
        _lobbyHelper.DebugWithDelay("Joined lobby", true).Forget();
      }
    }
  }
}
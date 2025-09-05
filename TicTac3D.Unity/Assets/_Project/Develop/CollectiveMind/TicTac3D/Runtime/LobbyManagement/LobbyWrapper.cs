using System.Threading;
using Cysharp.Threading.Tasks;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;

namespace CollectiveMind.TicTac3D.Runtime.LobbyManagement
{
  public class LobbyWrapper
  {
    public static async UniTask<AsyncResult<Lobby>> TryCreateLobbyUntilExitAsync(string lobbyName,
      int maxPlayers,
      CreateLobbyOptions options = default(CreateLobbyOptions),
      CancellationToken token = default(CancellationToken))
    {
      return await UniTaskUtils.ExecuteMethodUntilExitAsync(
        async () => await TryCreateLobbyAsync(lobbyName, maxPlayers, options, token), 3, token);
    }

    public static async UniTask<AsyncResult<Lobby>> TryCreateLobbyAsync(string lobbyName,
      int maxPlayers,
      CreateLobbyOptions options = default(CreateLobbyOptions),
      CancellationToken token = default(CancellationToken))
    {
      return await ConnectionUtils.TryExecuteMethodAsync(
        async () => await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options), token);
    }

    public static async UniTask<AsyncResult<Lobby>> TryGetLobbyUntilExitAsync(string lobbyId,
      CancellationToken token = default(CancellationToken))
    {
      return await UniTaskUtils.ExecuteMethodUntilExitAsync(async () => await TryGetLobbyAsync(lobbyId, token), 3,
        token);
    }

    public static async UniTask<AsyncResult<Lobby>> TryGetLobbyAsync(string lobbyId,
      CancellationToken token = default(CancellationToken))
    {
      return await ConnectionUtils.TryExecuteMethodAsync(async () => await LobbyService.Instance.GetLobbyAsync(lobbyId),
        token);
    }

    public static async UniTask<AsyncResult<Lobby>> TryUpdateLobbyUntilExitAsync(string lobbyId,
      UpdateLobbyOptions options,
      CancellationToken token = default(CancellationToken))
    {
      return await UniTaskUtils.ExecuteMethodUntilExitAsync(
        async () => await TryUpdateLobbyAsync(lobbyId, options, token), 3, token);
    }

    public static async UniTask<AsyncResult<Lobby>> TryUpdateLobbyAsync(string lobbyId,
      UpdateLobbyOptions options,
      CancellationToken token = default(CancellationToken))
    {
      return await ConnectionUtils.TryExecuteMethodAsync(
        async () => await LobbyService.Instance.UpdateLobbyAsync(lobbyId, options), token);
    }

    public static async UniTask<AsyncResult<Lobby>> TryJoinLobbyUntilExitAsync(string lobbyId,
      JoinLobbyByIdOptions options = default(JoinLobbyByIdOptions),
      CancellationToken token = default(CancellationToken))
    {
      return await UniTaskUtils.ExecuteMethodUntilExitAsync(
        async () => await TryJoinLobbyByIdAsync(lobbyId, options, token), 3, token);
    }

    public static async UniTask<AsyncResult<Lobby>> TryJoinLobbyByIdAsync(string lobbyId,
      JoinLobbyByIdOptions options = default(JoinLobbyByIdOptions),
      CancellationToken token = default(CancellationToken))
    {
      return await ConnectionUtils.TryExecuteMethodAsync(
        async () => await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId, options), token);
    }

    public static async UniTask<AsyncResult<Lobby>> TryJoinLobbyByCodeUntilExitAsync(string lobbyCode,
      JoinLobbyByCodeOptions options = default(JoinLobbyByCodeOptions),
      CancellationToken token = default(CancellationToken))
    {
      return await UniTaskUtils.ExecuteMethodUntilExitAsync(
        async () => await TryJoinLobbyByCodeAsync(lobbyCode, options, token), 3, token);
    }

    public static async UniTask<AsyncResult<Lobby>> TryJoinLobbyByCodeAsync(string lobbyId,
      JoinLobbyByCodeOptions options = default(JoinLobbyByCodeOptions),
      CancellationToken token = default(CancellationToken))
    {
      return await ConnectionUtils.TryExecuteMethodAsync(
        async () => await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyId, options), token);
    }

    public static async UniTask<AsyncResult> TrySendHeartbeatPingUntilExitAsync(string lobbyId,
      CancellationToken token = default(CancellationToken))
    {
      return await UniTaskUtils.ExecuteMethodUntilExitAsync(
        async () =>
        {
          AsyncResult result = await TrySendHeartbeatPingAsync(lobbyId, token);
          if (!result)
            return result.Convert<bool>();

          return result.Convert(result.Exception == null);
        }, 3, token);
    }

    public static async UniTask<AsyncResult> TrySendHeartbeatPingAsync(string lobbyId,
      CancellationToken token = default(CancellationToken))
    {
      return await ConnectionUtils.TryExecuteMethodAsync(
        async () => await LobbyService.Instance.SendHeartbeatPingAsync(lobbyId), token);
    }

    public static async UniTask<AsyncResult<QueryResponse>> TryQueryLobbiesUntilExitAsync(
      QueryLobbiesOptions options =
        default(QueryLobbiesOptions),
      CancellationToken token = default(CancellationToken))
    {
      return await UniTaskUtils.ExecuteMethodUntilExitAsync(async () => await TryQueryLobbiesAsync(options, token), 3,
        token);
    }

    public static async UniTask<AsyncResult<QueryResponse>> TryQueryLobbiesAsync(QueryLobbiesOptions options =
        default(QueryLobbiesOptions),
      CancellationToken token = default(CancellationToken))
    {
      return await ConnectionUtils.TryExecuteMethodAsync(
        async () => await LobbyService.Instance.QueryLobbiesAsync(options), token);
    }

    public static async UniTask<AsyncResult> TryRemovePlayerAsync(string lobbyId,
      string playerId,
      CancellationToken token = default(CancellationToken))
    {
      return await ConnectionUtils.TryExecuteMethodAsync(
        async () => await LobbyService.Instance.RemovePlayerAsync(lobbyId, playerId), token);
    }

    public static async UniTask<AsyncResult> TryDeleteLobbyAsync(string lobbyId,
      CancellationToken token = default(CancellationToken))
    {
      return await ConnectionUtils.TryExecuteMethodAsync(
        async () => await LobbyService.Instance.DeleteLobbyAsync(lobbyId), token);
    }

    public static async UniTask<AsyncResult<Lobby>> TryUpdatePlayerAsync(string lobbyId,
      string playerId,
      UpdatePlayerOptions options,
      CancellationToken token = default(CancellationToken))
    {
      return await ConnectionUtils.TryExecuteMethodAsync(
        async () => await LobbyService.Instance.UpdatePlayerAsync(lobbyId, playerId, options), token);
    }
  }
}
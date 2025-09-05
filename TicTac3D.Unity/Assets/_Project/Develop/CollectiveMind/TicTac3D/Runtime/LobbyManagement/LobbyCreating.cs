using System.Threading;
using CollectiveMind.TicTac3D.Runtime.Gameplay;
using Cysharp.Threading.Tasks;

namespace CollectiveMind.TicTac3D.Runtime.LobbyManagement
{
  public class LobbyCreating
  {
    private readonly GameRulesProvider _rulesProvider;
    private readonly LobbyManager _lobbyManager;
    private readonly LobbyHelper _lobbyHelper;
    private readonly ConnectionInfo _connectionInfo;

    public bool IsLobbyCreating { get; private set; }

    private ConnectionInfo.LobbyInfo CreatedLobby => _connectionInfo.CreatedLobby;

    public LobbyCreating(GameRulesProvider rulesProvider,
      LobbyManager lobbyManager,
      LobbyHelper lobbyHelper,
      ConnectionInfo connectionInfo)
    {
      _rulesProvider = rulesProvider;
      _lobbyManager = lobbyManager;
      _lobbyHelper = lobbyHelper;
      _connectionInfo = connectionInfo;
    }

    public async UniTask<AsyncResult> CreateLobby(CancellationToken token = default(CancellationToken))
    {
      IsLobbyCreating = true;
      AsyncResult result = await CreateLobbyInternal(token);
      IsLobbyCreating = false;
      return result;
    }

    private async UniTask<AsyncResult> CreateLobbyInternal(CancellationToken token = default(CancellationToken))
    {
      if (!await _lobbyManager.SignIn(token))
        return AsyncReturn.Cancel();
      
      await _lobbyHelper.DebugWithDelay("Start lobby creation");

      UniTask task = UniTask.WhenAll(_lobbyHelper.CreateLobby(true, token),
          _lobbyHelper.CheckJoinedPlayer(token),
          _lobbyHelper.CreateRelayAllocation(token),
          _lobbyHelper.GetRelayCode(token),
          _lobbyHelper.UpdateRelayCode(token),
          _lobbyHelper.ConnectNetwork(CreatedLobby, true, x => x.NeedReconnect, token),
          _lobbyHelper.SendReadyResponse(token),
          _lobbyHelper.WaitClientReadiness(_rulesProvider.Rules.Data, token))
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
  }
}
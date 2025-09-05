using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using CollectiveMind.TicTac3D.Runtime.UI;
using Cysharp.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Zenject;

namespace CollectiveMind.TicTac3D.Runtime.LobbyManagement
{
  public class LobbyManager : IDisposable
  {
    private readonly IInstantiator _instantiator;
    private readonly ConnectionInfo _connectionInfo;
    private readonly LobbyHelper _lobbyHelper;
    private readonly AuthorizationService _authorizationService;
    private readonly FreeLobbySearching _freeLobbySearching;
    private readonly LobbyCreating _lobbyCreating;

    private CancellationTokenSource _pingCts;
    private CancellationTokenSource _searchCts;
    private readonly LobbyJoining _lobbyJoining;

    public string JoinCode => _connectionInfo.CreatedLobby.Lobby?.LobbyCode;
    public bool IsLobbyCreating => _lobbyCreating.IsLobbyCreating;
    public bool IsLobbyCreated => _connectionInfo.CreatedLobby.Lobby != null;

    public LobbyManager(IInstantiator instantiator,
      ConnectionInfo connectionInfo,
      LobbyHelper lobbyHelper)
    {
      _instantiator = instantiator;
      _connectionInfo = connectionInfo;
      _lobbyHelper = lobbyHelper;
      _authorizationService = new AuthorizationService();

      _freeLobbySearching = _instantiator.Instantiate<FreeLobbySearching>(new[] { this });
      _lobbyCreating = _instantiator.Instantiate<LobbyCreating>(new[] { this });
      _lobbyJoining = _instantiator.Instantiate<LobbyJoining>(new[] { this });
    }

    public async UniTask Initialize()
    {
      await UnityServices.InitializeAsync();

      _pingCts = new CancellationTokenSource();
      PingHost(_pingCts.Token).Forget();
      PingPlayer(() => _connectionInfo.CreatedLobby.Lobby, _pingCts.Token).Forget();
      PingPlayer(() => _connectionInfo.JoinedLobby.Lobby, _pingCts.Token).Forget();
    }

    public async UniTask<AsyncResult> SignIn(CancellationToken token = default(CancellationToken))
    {
      return await _authorizationService.SignIn(token);
    }

    public async UniTask CreateLobby()
    {
      await ConnectToLobby(_lobbyCreating.CreateLobby);
    }

    public async UniTask JoinLobby(string lobbyCode)
    {
      await ConnectToLobby(async token => await _lobbyJoining.JoinLobby(lobbyCode, token));
    }

    public async UniTask SearchFreeLobby()
    {
      await ConnectToLobby(_freeLobbySearching.SearchFreeLobby);
    }

    private async UniTask ConnectToLobby(Func<CancellationToken, UniTask<AsyncResult>> connector)
    {
      _connectionInfo.ClearAll();
      _searchCts = new CancellationTokenSource();

      await connector.Invoke(_searchCts.Token);

      _searchCts = _searchCts?.CancelDisposeAndForget();
    }

    public void CancelSearch()
    {
      _searchCts = _searchCts?.CancelDisposeAndForget();
    }

    private async UniTask PingHost(CancellationToken token = default(CancellationToken))
    {
      while (!token.IsCancellationRequested)
      {
        await UniTask.WaitUntil(() => _connectionInfo.CreatedLobby.Lobby != null, cancellationToken: token)
          .SuppressCancellationThrow();
        if (token.IsCancellationRequested)
          return;

        AsyncResult result = await LobbyWrapper.TrySendHeartbeatPingAsync(_connectionInfo.CreatedLobby.LobbyId, token);
        if (token.IsCancellationRequested)
          return;

        if (result.IsValid)
          await UniTask.WaitForSeconds(5f, cancellationToken: token).SuppressCancellationThrow();
      }
    }

    private async UniTask PingPlayer(Func<Lobby> when, CancellationToken token = default(CancellationToken))
    {
      while (!token.IsCancellationRequested)
      {
        await UniTask.WaitUntil(() => when.Invoke() != null, cancellationToken: token).SuppressCancellationThrow();
        if (token.IsCancellationRequested)
          return;

        AsyncResult result = await LobbyWrapper.TryUpdatePlayerAsync(when.Invoke().Id,
          AuthenticationService.Instance.PlayerId, CreateUpdatePlayerOptions(), token: token);
        if (token.IsCancellationRequested)
          return;

        if (result.IsValid)
          await UniTask.WaitForSeconds(5f, cancellationToken: token).SuppressCancellationThrow();
      }
    }

    private static UpdatePlayerOptions CreateUpdatePlayerOptions()
    {
      return new UpdatePlayerOptions
      {
        Data = new Dictionary<string, PlayerDataObject>
        {
          {
            "LastUpdateTime",
            new PlayerDataObject(PlayerDataObject.VisibilityOptions.Private,
              DateTime.UtcNow.ToString(CultureInfo.InvariantCulture))
          }
        }
      };
    }

    public async UniTask LeaveLobby()
    {
      await _lobbyHelper.LeaveLobby();
    }

    public void Dispose()
    {
      if (_searchCts != null)
        CancelSearch();
      else
        _lobbyHelper.LeaveLobby(true).Forget();

      _pingCts?.CancelDisposeAndForget();
    }
  }
}
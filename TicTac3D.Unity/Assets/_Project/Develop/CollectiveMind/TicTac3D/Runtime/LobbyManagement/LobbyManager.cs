using System;
using System.Collections.Generic;
using System.Threading;
using CollectiveMind.TicTac3D.Runtime.Gameplay;
using CollectiveMind.TicTac3D.Runtime.Network;
using CollectiveMind.TicTac3D.Runtime.UI;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using Zenject;

namespace CollectiveMind.TicTac3D.Runtime.LobbyManagement
{
  public class LobbyManager : IDisposable
  {
    private readonly NetworkManager _networkManager;
    private readonly IRpcProvider _rpcProvider;
    private readonly GameRulesProvider _rulesProvider;
    private readonly IInstantiator _instantiator;
    private readonly AuthorizationService _authorizationService;
    private readonly RandomLobbySearching _randomLobbySearching;

    private string _lobbyId;
    private bool _isJoinedLobby;
    private Lobby _lobby;
    private CancellationTokenSource _pingCts;
    private CancellationTokenSource _searchCts;

    public string JoinCode => _lobby?.LobbyCode;
    public bool IsPrivateLobby => _lobby?.IsPrivate ?? false;
    public bool IsHost => _networkManager.IsHost;
    public bool IsLobbyCreating { get; private set; }
    
    public CancellationToken SearchingCancellationToken => _searchCts?.Token ?? CancellationToken.None;

    private GameRulesData GameRulesData => _rulesProvider.Rules.Data;

    public LobbyManager(NetworkManager networkManager,
      IRpcProvider rpcProvider,
      GameRulesProvider rulesProvider,
      IInstantiator instantiator)
    {
      _networkManager = networkManager;
      _rpcProvider = rpcProvider;
      _rulesProvider = rulesProvider;
      _instantiator = instantiator;
      _authorizationService = new AuthorizationService();
      _randomLobbySearching = _instantiator.Instantiate<RandomLobbySearching>(new[] { this });
    }

    public async UniTask Initialize()
    {
      await UnityServices.InitializeAsync();

      _pingCts = new CancellationTokenSource();
      Ping(_pingCts.Token).Forget();
    }

    public async UniTask<CancelableResult> SignIn(CancellationToken token = default(CancellationToken))
    {
      return await _authorizationService.SignIn(token);
    }

    public async UniTask CreateLobby(string lobbyName)
    {
      _searchCts = new CancellationTokenSource();
      IsLobbyCreating = true;
      await CreateLobbyInternal(lobbyName, SearchingCancellationToken);
      IsLobbyCreating = false;
      _searchCts?.Dispose();
      _searchCts = null;
    }

    private async UniTask CreateLobbyInternal(string lobbyName, CancellationToken token = default(CancellationToken))
    {
      if (!await SignIn(token))
          return;

      if (!string.IsNullOrWhiteSpace(lobbyName))
      {
        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(2);
        string relayCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

        Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, 2, new CreateLobbyOptions
        {
          Data = new Dictionary<string, DataObject>
          {
            {
              NC.RULES_NAME,
              new DataObject(DataObject.VisibilityOptions.Public, JsonConvert.SerializeObject(GameRulesData),
                DataObject.IndexOptions.S1)
            },
            {
              NC.RELAY_CODE_NAME,
              new DataObject(DataObject.VisibilityOptions.Member, relayCode)
            }
          },
          IsPrivate = true
        });

        if (token.IsCancellationRequested)
        {
          await LobbyService.Instance.DeleteLobbyAsync(lobby.Id);
          return;
        }

        _lobbyId = lobby.Id;
        _lobby = lobby;

        _networkManager.GetComponent<UnityTransport>().SetRelayServerData(allocation.ToRelayServerData("wss"));
        _networkManager.StartHost();

        if (token.IsCancellationRequested)
        {
          await LeaveLobby();
          return;
        }

        if (!await StartGame(GameRulesData, token))
        {
          await LeaveLobby();
          return;
        }

        _isJoinedLobby = true;
      }
    }
    
    public async UniTask JoinLobby(string lobbyCode)
    {
      _searchCts = new CancellationTokenSource();
      await JoinLobbyInternal(lobbyCode, SearchingCancellationToken);
      _searchCts?.Dispose();
      _searchCts = null;
    }

    private async UniTask JoinLobbyInternal(string lobbyCode, CancellationToken token = default(CancellationToken))
    {
      if (!await SignIn(token))
        return;

      if (!string.IsNullOrWhiteSpace(lobbyCode))
      {
        Lobby lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode);
        if (token.IsCancellationRequested)
        {
          await LobbyService.Instance.RemovePlayerAsync(_lobbyId, AuthenticationService.Instance.PlayerId);
          return;
        }

        _lobbyId = lobby.Id;
        _lobby = lobby;
        string relayCode = lobby.Data[NC.RELAY_CODE_NAME].Value;
        JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(relayCode);
        _networkManager.GetComponent<UnityTransport>().SetRelayServerData(allocation.ToRelayServerData("wss"));
        _networkManager.StartClient();

        if (token.IsCancellationRequested)
        {
          await LeaveLobby();
          return;
        }

        if (!await StartGame(GameRulesData.CreateRandom(), token))
        {
          await LeaveLobby();
          return;
        }

        _isJoinedLobby = true;
      }
    }

    public async UniTask SearchFreeLobby()
    {
      _searchCts = new CancellationTokenSource();
      CancelableResult<Lobby> result = await _randomLobbySearching.SearchFreeLobby(_searchCts.Token);
      if (result)
      {
        _lobby = result.Value;
        Debug.Log($"Found lobby {_lobby.Id}");
        _lobbyId = _lobby.Id;
        _isJoinedLobby = true;
      }

      _searchCts = _searchCts?.DisposeAndForget();
    }

    public void CancelSearch()
    {
      Debug.Log("Cancel search");
      _searchCts = _searchCts?.CancelDisposeAndForget();
    }

    private async UniTask<bool> StartGame(GameRulesData rules, CancellationToken token = default(CancellationToken))
    {
      await UniTask.WaitUntil(() => _rpcProvider.IsReady, cancellationToken: token);
      if (token.IsCancellationRequested)
        return false;

      _rpcProvider.SendRequest(new StartGameRequest { Rules = rules });
      return true;
    }

    private async UniTask Ping(CancellationToken token = default(CancellationToken))
    {
      while (!token.IsCancellationRequested)
      {
        if (_networkManager.IsHost && _isJoinedLobby)
          await LobbyService.Instance.SendHeartbeatPingAsync(_lobbyId);

        await UniTask.WaitForSeconds(5f, cancellationToken: token);
      }
    }

    public async UniTask LeaveLobby()
    {
      if (_networkManager.IsHost)
      {
        await LobbyService.Instance.DeleteLobbyAsync(_lobbyId);
      }
      else
      {
        try
        {
          await LobbyService.Instance.RemovePlayerAsync(_lobbyId, AuthenticationService.Instance.PlayerId);
        }
        catch (LobbyServiceException)
        {
        }
      }

      _networkManager.Shutdown();

      _isJoinedLobby = false;
      _lobbyId = "";
      _lobby = null;
    }

    public void Dispose()
    {
      if (_isJoinedLobby)
        LobbyService.Instance.DeleteLobbyAsync(_lobbyId);

      _pingCts?.CancelDisposeAndForget();
      
      CancelSearch();
    }
  }
}
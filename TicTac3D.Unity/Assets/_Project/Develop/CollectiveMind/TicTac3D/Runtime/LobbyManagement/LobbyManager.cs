using System;
using System.Collections.Generic;
using System.Threading;
using CollectiveMind.TicTac3D.Runtime.Gameplay;
using CollectiveMind.TicTac3D.Runtime.Network;
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

namespace CollectiveMind.TicTac3D.Runtime.LobbyManagement
{
  public class LobbyManager : IDisposable
  {
    private readonly NetworkManager _networkManager;
    private readonly IRpcProvider _rpcProvider;
    private string _lobbyId;
    private bool _isJoinedToLobby;
    private CancellationTokenSource _pingCts;
    private Lobby _lobby;

    public string JoinCode => _lobby?.LobbyCode;
    public bool IsPrivate => _lobby?.IsPrivate ?? false;

    public LobbyManager(NetworkManager networkManager, IRpcProvider rpcProvider)
    {
      _networkManager = networkManager;
      _rpcProvider = rpcProvider;
    }

    public async UniTask Initialize()
    {
      await UnityServices.InitializeAsync();

      _pingCts = new CancellationTokenSource();
      Ping(_pingCts.Token).Forget();
    }

    public async UniTask CreateLobby(GameRulesData userRules,
      string lobbyName,
      CancellationToken token = default(CancellationToken))
    {
      if (!AuthenticationService.Instance.IsSignedIn)
      {
        await Authorize(token);
      }

      if (!string.IsNullOrWhiteSpace(lobbyName))
      {
        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(2);
        string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

        Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, 2, new CreateLobbyOptions
        {
          Data = new Dictionary<string, DataObject>
          {
            {
              "Rules",
              new DataObject(DataObject.VisibilityOptions.Public, JsonConvert.SerializeObject(userRules))
            },
            {
              "JoinCode",
              new DataObject(DataObject.VisibilityOptions.Member, joinCode)
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

        await UniTask.WaitUntil(() => _rpcProvider.IsReady, cancellationToken: token);
        if (token.IsCancellationRequested)
        {
          await LeaveLobby();
          return;
        }
        
        _rpcProvider.SendRequest(new StartGameRequest { Rules = userRules }, _networkManager.RpcTarget.Server);

        _isJoinedToLobby = true;
      }
    }

    public async UniTask JoinLobby(GameRulesData userRules,
      string lobbyCode,
      CancellationToken token = default(CancellationToken))
    {
      if (!AuthenticationService.Instance.IsSignedIn)
      {
        await Authorize(token);
      }

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
        string joinCode = lobby.Data["JoinCode"].Value;
        JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
        _networkManager.GetComponent<UnityTransport>().SetRelayServerData(allocation.ToRelayServerData("wss"));
        _networkManager.StartClient();
        
        if (token.IsCancellationRequested)
        {
          await LeaveLobby();
          return;
        }

        await UniTask.WaitUntil(() => _rpcProvider.IsReady, cancellationToken: token);
        if (token.IsCancellationRequested)
        {
          await LeaveLobby();
          return;
        }
        
        _rpcProvider.SendRequest(new StartGameRequest { Rules = userRules });

        _isJoinedToLobby = true;
      }
    }

    public async UniTask CreateOrJoinLobby(GameRulesData userRules, CancellationToken token = default(CancellationToken))
    {
      
    }

    public async UniTask Authorize(CancellationToken token = default(CancellationToken))
    {
      while (!AuthenticationService.Instance.IsAuthorized)
      {
        if (token.IsCancellationRequested)
          break;

        try
        {
          await AuthenticationService.Instance.SignInAnonymouslyAsync();
          Debug.Log("Signed in.");
        }
        catch (RequestFailedException)
        {
          Debug.Log("Can not sign in. Retrying...");
          await UniTask.WaitForSeconds(0.5f, cancellationToken: token);
        }
      }
    }

    private async UniTask Ping(CancellationToken token = default(CancellationToken))
    {
      while (!token.IsCancellationRequested)
      {
        if (_networkManager.IsHost)
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
      _isJoinedToLobby = false;
      _lobbyId = "";
      _lobby = null;
    }

    public void Dispose()
    {
      if (_isJoinedToLobby)
        LobbyService.Instance.DeleteLobbyAsync(_lobbyId);

      _pingCts.Cancel();
      _pingCts.Dispose();
    }
  }
}
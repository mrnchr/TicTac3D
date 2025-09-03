using System;
using System.Diagnostics;
using System.Threading;
using CollectiveMind.TicTac3D.Runtime.Gameplay;
using CollectiveMind.TicTac3D.Runtime.Network;
using Cysharp.Threading.Tasks;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace CollectiveMind.TicTac3D.Runtime.LobbyManagement
{
  public class LobbyHelper
  {
    private readonly ConnectionInfo _connectionInfo;
    private readonly IRpcProvider _rpcProvider;
    private readonly NetworkManager _networkManager;
    private readonly INetworkBus _networkBus;

    public LobbyHelper(ConnectionInfo connectionInfo,
      IRpcProvider rpcProvider,
      NetworkManager networkManager,
      INetworkBus networkBus)
    {
      _connectionInfo = connectionInfo;
      _rpcProvider = rpcProvider;
      _networkManager = networkManager;
      _networkBus = networkBus;
    }
    
    public async UniTask StartGameOnHost(GameRulesData gameRules, CancellationToken token = default(CancellationToken))
    {
      while (!token.IsCancellationRequested)
      {
        await UniTask
          .WaitUntil(() => _networkManager.IsHost && _rpcProvider.IsReady && _networkManager.ConnectedClients.Count > 1,
            cancellationToken: token).SuppressCancellationThrow();
        if(token.IsCancellationRequested) 
          return;

        SendReadyResponse(token).Forget();

        var isClientReady = false;
        _networkBus.SubscribeOnRpc<ClientReadyRequest>(() => isClientReady = true);
        await UniTask.WaitUntil(() => isClientReady, cancellationToken: token).SuppressCancellationThrow();
        _networkBus.UnsubscribeFromRpc<ClientReadyRequest>();

        if(token.IsCancellationRequested) 
          return;

        _rpcProvider.SendRequest(new StartGameRequest { Rules = gameRules });
        _connectionInfo.GameStarted = true;
      }
    }

    private async UniTask SendReadyResponse(CancellationToken token = default(CancellationToken))
    {
      while (!token.IsCancellationRequested)
      {
        await UniTask.WaitUntil(() => _networkManager.ConnectedClients.Count > 1, cancellationToken: token)
          .SuppressCancellationThrow();
        if(token.IsCancellationRequested) 
          return;

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

        var isServerReady = false;
        _networkBus.SubscribeOnRpc<ServerReadyResponse>(() => isServerReady = true);
        await UniTask.WaitUntil(() => isServerReady, cancellationToken: token).SuppressCancellationThrow();
        _networkBus.UnsubscribeFromRpc<ServerReadyResponse>();

        await UniTask.WaitUntil(() => _networkManager.IsConnectedClient, cancellationToken: token)
          .SuppressCancellationThrow();
        if (token.IsCancellationRequested)
          return;

        _rpcProvider.SendRequest<ClientReadyRequest>(_networkManager.RpcTarget.Server);
        _rpcProvider.SendRequest(new StartGameRequest { Rules = gameRules });
        _connectionInfo.GameStarted = true;
      }
    }

    public async UniTask LeaveLobby(bool forget = false)
    {
      if (_connectionInfo.CreatedLobby)
      {
        await LobbyWrapper.TryDeleteLobbyAsync(_connectionInfo.LobbyId).TryForget(forget);
      }
      else if (_connectionInfo.JoinedLobby)
      {
        await LobbyWrapper.TryRemovePlayerAsync(_connectionInfo.LobbyId, AuthenticationService.Instance.PlayerId)
          .TryForget(forget);
      }

      _networkManager.Shutdown();
      _connectionInfo.ClearAll();
    }

    [HideInCallstack]
    [DebuggerHidden]
    public async UniTask DebugWithDelay(string message)
    {
      Debug.Log(message);
      await UniTask.Delay(5000).SuppressCancellationThrow();
      Debug.Log("Continue...");
    }
  }
}
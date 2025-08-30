using System.Threading;
using CollectiveMind.TicTac3D.Runtime.Gameplay;
using CollectiveMind.TicTac3D.Runtime.Network;
using Cysharp.Threading.Tasks;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;

namespace CollectiveMind.TicTac3D.Runtime.LobbyManagement
{
  public class LobbyHelper
  {
    private readonly ConnectionInfo _connectionInfo;
    private readonly IRpcProvider _rpcProvider;
    private readonly NetworkManager _networkManager;

    public LobbyHelper(ConnectionInfo connectionInfo, IRpcProvider rpcProvider, NetworkManager networkManager)
    {
      _connectionInfo = connectionInfo;
      _rpcProvider = rpcProvider;
      _networkManager = networkManager;
    }

    public async UniTask<AsyncResult> StartGame(GameRulesData rules,
      CancellationToken token = default(CancellationToken))
    {
      await UniTask.WaitUntil(() => _rpcProvider.IsReady, cancellationToken: token);
      if (token.IsCancellationRequested)
        return AsyncReturn.Cancel();

      _rpcProvider.SendRequest(new StartGameRequest { Rules = rules });
      return AsyncReturn.Ok();
    }

    public async UniTask LeaveLobby(bool forget = false)
    {
      if (_connectionInfo.CreatedLobby)
      {
        await LobbyWrapper.TryDeleteLobbyAsync(_connectionInfo.LobbyId).TryForget(true);
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
    public async UniTask DebugWithDelay(string message)
    {
      Debug.Log(message);
      await UniTask.Delay(5000);
      Debug.Log("Continue...");
    }
  }
}
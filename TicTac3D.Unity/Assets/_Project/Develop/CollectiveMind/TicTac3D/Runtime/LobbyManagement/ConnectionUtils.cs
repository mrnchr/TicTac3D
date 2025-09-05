using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Unity.Services.Lobbies;
using Unity.Services.Relay;

namespace CollectiveMind.TicTac3D.Runtime.LobbyManagement
{
  public static class ConnectionUtils
  {
    public static async UniTask<AsyncResult<TValue>> TryExecuteMethodAsync<TValue>(
      Func<UniTask<AsyncResult<TValue>>> action,
      CancellationToken token = default(CancellationToken))
    {
      return await TryExecuteMethodInternalAsync(
        async internalToken => await UniTaskUtils.TryExecuteMethodAsync(action, internalToken), token);
    }

    public static async UniTask<AsyncResult<TValue>> TryExecuteMethodAsync<TValue>(
      Func<UniTask<TValue>> action,
      CancellationToken token = default(CancellationToken))
    {
      return await TryExecuteMethodInternalAsync(
        async internalToken => await UniTaskUtils.TryExecuteMethodAsync(action, internalToken), token);
    }
    
    public static async UniTask<AsyncResult> TryExecuteMethodAsync(
      Func<UniTask> action,
      CancellationToken token = default(CancellationToken))
    {
      return await TryExecuteMethodInternalAsync(async internalToken => await UniTaskUtils.TryExecuteMethodAsync(
        async () =>
        {
          await action.Invoke();
          return AsyncReturn.Ok();
        }, internalToken), token);
    }

    private static async UniTask<AsyncResult<TValue>> TryExecuteMethodInternalAsync<TValue>(
      Func<CancellationToken, UniTask<AsyncResult<TValue>>> action,
      CancellationToken token = default(CancellationToken))
    {
      AsyncResult<TValue> result = await action.Invoke(token);
      result.ReturnCode = result.Exception switch
      {
        RelayServiceException relayException => (int)relayException.Reason,
        LobbyServiceException lobbyServiceException => (int)lobbyServiceException.Reason,
        _ => 0
      };

      return result;
    }
    
    private static async UniTask<AsyncResult> TryExecuteMethodInternalAsync(
      Func<CancellationToken, UniTask<AsyncResult>> action,
      CancellationToken token = default(CancellationToken))
    {
      AsyncResult result = await action.Invoke(token);
      result.ReturnCode = result.Exception switch
      {
        RelayServiceException relayException => (int)relayException.Reason,
        LobbyServiceException lobbyServiceException => (int)lobbyServiceException.Reason,
        _ => 0
      };
      
      return result;
    }
  }
}
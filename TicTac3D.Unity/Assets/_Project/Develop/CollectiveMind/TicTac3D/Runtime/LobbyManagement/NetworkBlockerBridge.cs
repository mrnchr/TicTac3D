#if UNITY_EDITOR
using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace CollectiveMind.TicTac3D.Runtime.LobbyManagement
{
  public class NetworkBlockerBridge
  {
    public static Func<UniTask> BlockNetwork;
    public static Func<UniTask> UnblockNetwork;

    private static bool _isBlocked;

    public static async UniTask BlockOnTime(float time, CancellationToken token = default(CancellationToken))
    {
      await UniTask.WaitUntil(() => !_isBlocked, cancellationToken: token).SuppressCancellationThrow();
      BlockOnTimeInternal(time, token).Forget();
      await UniTask.WaitUntil(() => _isBlocked, cancellationToken: token).SuppressCancellationThrow();
    }

    private static async UniTask BlockOnTimeInternal(float time, CancellationToken token = default(CancellationToken))
    {
      if (token.IsCancellationRequested)
        return;
      
      await BlockNetwork.Invoke();
      _isBlocked = true;
      
      await UniTask.Delay(TimeSpan.FromSeconds(time), cancellationToken: token).SuppressCancellationThrow();
      await UnblockNetwork.Invoke();
      _isBlocked = false;
    }
  }
}
#endif
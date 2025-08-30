using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;

namespace CollectiveMind.TicTac3D.Runtime.LobbyManagement
{
  public class RelayWrapper
  {
    public static async UniTask<AsyncResult<Allocation>> TryCreateAllocationUntilExitAsync(int maxConnections,
      string region = null,
      CancellationToken token = default(CancellationToken))
    {
      return await UniTaskUtils.ExecuteMethodUntilExitAsync(
        async () => await TryCreateAllocationAsync(maxConnections, region, token), 3, token);
    }

    public static async UniTask<AsyncResult<Allocation>> TryCreateAllocationAsync(int maxConnections,
      string region = null,
      CancellationToken token = default(CancellationToken))
    {
      return await ConnectionUtils.TryExecuteMethodAsync(
        async () => await RelayService.Instance.CreateAllocationAsync(maxConnections, region), token);
    }

    public static async UniTask<AsyncResult<string>> TryGetJoinCodeUntilExitAsync(Guid allocationId,
      CancellationToken token = default(CancellationToken))
    {
      var result = new AsyncResult<string>();
      await UniTaskUtils.ExecuteMethodUntilExitAsync(
        async () =>
        {
          result = await TryGetJoinCodeAsync(allocationId, token);
          if (!result)
            return result.Convert<bool>();

          if (result.ReturnCode == (int)RelayExceptionReason.AllocationNotFound)
            return result.Convert(true);

          return result.Convert(result.Value != null);
        }, 3, token);

      return result;
    }


    public static async UniTask<AsyncResult<string>> TryGetJoinCodeAsync(Guid allocationId,
      CancellationToken token = default(CancellationToken))
    {
      return await ConnectionUtils.TryExecuteMethodAsync(
        async () => await RelayService.Instance.GetJoinCodeAsync(allocationId), token);
    }

    public static async UniTask<AsyncResult<JoinAllocation>> TryJoinAllocationUntilExitAsync(string joinCode,
      CancellationToken token = default(CancellationToken))
    {
      var result = new AsyncResult<JoinAllocation>();
      await UniTaskUtils.ExecuteMethodUntilExitAsync(
        async () =>
        {
          result = await TryJoinAllocationAsync(joinCode, token);
          if (!result)
            return result.Convert<bool>();

          if (result.ReturnCode == (int)RelayExceptionReason.JoinCodeNotFound)
            return result.Convert(true);

          return result.Convert(result.Value != null);
        }, 3, token);

      return result;
    }

    public static async UniTask<AsyncResult<JoinAllocation>> TryJoinAllocationAsync(string joinCode,
      CancellationToken token = default(CancellationToken))
    {
      return await ConnectionUtils.TryExecuteMethodAsync(
        async () => await RelayService.Instance.JoinAllocationAsync(joinCode), token);
    }
  }
}
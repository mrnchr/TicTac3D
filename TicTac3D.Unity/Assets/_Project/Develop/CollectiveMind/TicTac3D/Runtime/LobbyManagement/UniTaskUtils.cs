using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace CollectiveMind.TicTac3D.Runtime.LobbyManagement
{
  public static class UniTaskUtils
  {
    public static async UniTask TryForget(this UniTask task, bool forget)
    {
      if (forget)
        task.Forget();
      else
        await task;
    }

    public static async UniTask<T> TryForget<T>(this UniTask<T> task, bool forget)
    {
      if (!forget)
        return await task;

      task.Forget();
      return default(T);
    }
    
    public static async UniTask<AsyncResult> ExecuteMethodUntilExitAsync(
      Func<UniTask<AsyncResult<bool>>> action,
      CancellationToken token = default(CancellationToken))
    {
      return await ExecuteMethodUntilExitAsync(action, 0, token);
    }

    public static async UniTask<AsyncResult> ExecuteMethodUntilExitAsync(
      Func<UniTask<AsyncResult<bool>>> action,
      float delay,
      CancellationToken token = default(CancellationToken))
    {
      var actionResult = new AsyncResult<bool>(); 
      while (!token.IsCancellationRequested && !(actionResult = await action.Invoke()).Value)
        await UniTask.WaitForSeconds(delay, cancellationToken: token).SuppressCancellationThrow();

      actionResult.Cancelled = token.IsCancellationRequested;
      return actionResult;
    }
    
    public static async UniTask<AsyncResult<TValue>> TryExecuteMethodAsync<TValue>(
      Func<UniTask<AsyncResult<TValue>>> action,
      CancellationToken token = default(CancellationToken))
    {
      var result = new AsyncResult<TValue>();
      try
      {
        result = await action.Invoke();
      }
      catch (Exception exception)
      {
        result.Exception = exception;
      }
      finally
      {
        result.Cancelled = token.IsCancellationRequested;
      }

      return result;
    }
    
    public static async UniTask<AsyncResult> TryExecuteMethodAsync(
      Func<UniTask<AsyncResult>> action,
      CancellationToken token = default(CancellationToken))
    {
      var result = new AsyncResult();
      try
      {
        result = await action.Invoke();
      }
      catch (Exception exception)
      {
        result.Exception = exception;
      }
      finally
      {
        result.Cancelled = token.IsCancellationRequested;
      }

      return result;
    }
      
    public static async UniTask<bool> CheckCancellationAfter(Func<UniTask> task,
      CancellationToken token = default(CancellationToken))
    {
      await task.Invoke();
      return token.IsCancellationRequested;
    }

    public static async UniTask<AsyncResult<TValue>> ExecuteMethodUntilExitAsync<TValue>(
      Func<UniTask<AsyncResult<TValue>>> task,
      float delay,
      CancellationToken token = default(CancellationToken)) where TValue : class
    {
      var result = new AsyncResult<TValue>();
      await ExecuteMethodUntilExitAsync(async () =>
      {
        result = await task.Invoke();
        result.Cancelled = token.IsCancellationRequested;
        return result.Convert(result.Value != null);
      }, delay, token);

      return result;
    }

    public static async UniTask<AsyncResult<TValue>> TryExecuteMethodAsync<TValue>(Func<UniTask<TValue>> task,
      CancellationToken token = default(CancellationToken))
    {
      return await TryExecuteMethodAsync(async () => AsyncReturn.Ok(await task.Invoke()), token);
    }
  }
}
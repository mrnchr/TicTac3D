using System.Threading;

namespace CollectiveMind.TicTac3D.Runtime.LobbyManagement
{
  public static class AsyncReturn
  {
    public static AsyncResult Ok()
    {
      return new AsyncResult();
    }

    public static AsyncResult Cancel()
    {
      return new AsyncResult { Cancelled = true };
    }

    public static AsyncResult FromToken(CancellationToken token)
    {
      return new AsyncResult(token.IsCancellationRequested);
    }

    public static AsyncResult<TValue> Ok<TValue>()
    {
      return new AsyncResult<TValue>();
    }
    
    public static AsyncResult<TValue> Ok<TValue>(TValue value)
    {
      return new AsyncResult<TValue>(value);
    }

    public static AsyncResult<TValue> Cancel<TValue>()
    {
      return new AsyncResult<TValue> { Cancelled = true };
    }
    
    public static AsyncResult<TValue> Cancel<TValue>(TValue value)
    {
      return new AsyncResult<TValue>
      {
        Cancelled = true,
        Value = value
      };
    }

    public static AsyncResult<TValue> FromToken<TValue>(CancellationToken token)
    {
      return token.IsCancellationRequested ? Cancel<TValue>() : Ok<TValue>();
    }
    
    public static AsyncResult<TValue> FromToken<TValue>(TValue value, CancellationToken token)
    {
      return new AsyncResult<TValue>(value, token.IsCancellationRequested);
    }

    public static AsyncResult<TValue> FromResult<TValue, TResult>(AsyncResult<TResult> result, TValue value)
    {
      return result.Convert(value);
    }
    
    public static AsyncResult<TValue> FromResult<TResult, TValue>(AsyncResult<TResult> result)
    {
      return result.Convert<TValue>();
    }
  }
}
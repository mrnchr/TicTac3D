using System;

namespace CollectiveMind.TicTac3D.Runtime.LobbyManagement
{
  [Serializable]
  public struct AsyncResult
  {
    public bool Cancelled;
    public int ReturnCode;
    public Exception Exception;

    public bool IsValid => !Cancelled && Exception == null;

    public AsyncResult(bool cancelled)
    {
      Cancelled = cancelled;
      ReturnCode = 0;
      Exception = null;
    }

    public AsyncResult(bool cancelled, int returnCode, Exception exception)
    {
      Cancelled = cancelled;
      ReturnCode = returnCode;
      Exception = exception;
    }

    public static implicit operator bool(AsyncResult result)
    {
      return !result.Cancelled;
    }

    public AsyncResult<TValue> Convert<TValue>(TValue value = default(TValue))
    {
      return new AsyncResult<TValue>(value, Cancelled, ReturnCode, Exception);
    }
  }

  [Serializable]
  public struct AsyncResult<TValue>
  {
    public TValue Value;
    public bool Cancelled;
    public int ReturnCode;
    public Exception Exception;

    public bool IsValid => !Cancelled && Exception == null;

    public AsyncResult(TValue value)
    {
      Value = value;
      Cancelled = false;
      ReturnCode = 0;
      Exception = null;
    }

    public AsyncResult(TValue value, bool cancelled)
    {
      Value = value;
      Cancelled = cancelled;
      ReturnCode = 0;
      Exception = null;
    }

    public AsyncResult(TValue value, bool cancelled, int returnCode, Exception exception)
    {
      Value = value;
      Cancelled = cancelled;
      ReturnCode = returnCode;
      Exception = exception;
    }

    public AsyncResult<TNewValue> Convert<TNewValue>()
    {
      return new AsyncResult<TNewValue>
      {
        Cancelled = Cancelled,
        ReturnCode = ReturnCode,
        Exception = Exception
      };
    }

    public AsyncResult<TNewValue> Convert<TNewValue>(TNewValue value)
    {
      return new AsyncResult<TNewValue>(value, Cancelled, ReturnCode, Exception);
    }

    public static implicit operator AsyncResult(AsyncResult<TValue> result)
    {
      return new AsyncResult(result.Cancelled, result.ReturnCode, result.Exception);
    }

    public static implicit operator TValue(AsyncResult<TValue> result)
    {
      return result.Value;
    }

    public static implicit operator bool(AsyncResult<TValue> result)
    {
      return !result.Cancelled;
    }
  }
}
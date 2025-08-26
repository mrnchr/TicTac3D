using System;

namespace CollectiveMind.TicTac3D.Runtime.LobbyManagement
{
  [Serializable]
  public struct CancelableResult
  {
    public bool Cancelled;

    public CancelableResult<TValue> Convert<TValue>(TValue value = default(TValue))
    {
      return new CancelableResult<TValue>(value, Cancelled);
    }

    public static implicit operator CancelableResult(bool cancelled)
    {
      return new CancelableResult { Cancelled = cancelled };
    }

    public static implicit operator bool(CancelableResult result)
    {
      return !result.Cancelled;
    }
  }

  [Serializable]
  public struct CancelableResult<TValue>
  {
    public TValue Value;
    public bool Cancelled;

    public CancelableResult(TValue value)
    {
      Value = value;
      Cancelled = false;
    }
    
    public CancelableResult(TValue value, bool cancelled)
    {
      Value = value;
      Cancelled = cancelled;
    }

    public static implicit operator CancelableResult(CancelableResult<TValue> result)
    {
      return new CancelableResult { Cancelled = result.Cancelled };
    }

    public static implicit operator CancelableResult<TValue>(TValue value)
    {
      return new CancelableResult<TValue> { Value = value, Cancelled = false };
    }

    public static implicit operator TValue(CancelableResult<TValue> result)
    {
      return !result.Cancelled ? result.Value : default(TValue);
    }

    public static implicit operator bool(CancelableResult<TValue> result)
    {
      return !result.Cancelled;
    }

    public static implicit operator CancelableResult<TValue>(bool cancelled)
    {
      return new CancelableResult<TValue> { Cancelled = cancelled };
    }
  }
}
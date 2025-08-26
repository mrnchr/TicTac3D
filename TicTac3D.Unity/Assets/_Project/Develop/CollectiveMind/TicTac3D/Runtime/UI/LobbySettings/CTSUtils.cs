using System.Threading;

namespace CollectiveMind.TicTac3D.Runtime.UI
{
  public static class CTSUtils
  {
    public static void CancelAndDispose(ref CancellationTokenSource source)
    {
      source?.Cancel();
      source?.Dispose();
      source = null;
    } 
    
    public static CancellationTokenSource CancelDisposeAndForget(this CancellationTokenSource source)
    {
      source?.Cancel();
      source?.Dispose();
      return null;
    }

    public static CancellationTokenSource DisposeAndForget(this CancellationTokenSource source)
    {
      source?.Dispose();
      return null;
    }
  }
}
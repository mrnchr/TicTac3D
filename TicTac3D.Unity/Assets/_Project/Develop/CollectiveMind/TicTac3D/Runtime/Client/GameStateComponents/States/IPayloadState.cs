using Cysharp.Threading.Tasks;

namespace CollectiveMind.TicTac3D.Runtime.Client.GameStateComponents
{
  public interface IPaylodedState<TPayload> : IExitableState
  {
    public UniTask Enter(TPayload payload);
  }
}
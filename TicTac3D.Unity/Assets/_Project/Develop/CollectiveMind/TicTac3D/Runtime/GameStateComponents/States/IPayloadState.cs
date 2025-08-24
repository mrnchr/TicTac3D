using Cysharp.Threading.Tasks;

namespace CollectiveMind.TicTac3D.Runtime.GameStateComponents
{
  public interface IPaylodedState<TPayload> : IExitableState
  {
    public UniTask Enter(TPayload payload);
  }
}
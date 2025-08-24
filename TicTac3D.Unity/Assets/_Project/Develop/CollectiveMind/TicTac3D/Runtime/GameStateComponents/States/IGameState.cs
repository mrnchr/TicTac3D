using Cysharp.Threading.Tasks;

namespace CollectiveMind.TicTac3D.Runtime.GameStateComponents
{
  public interface IGameState : IExitableState
  {
    UniTask Enter();
  }
}
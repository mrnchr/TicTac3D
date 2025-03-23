using Cysharp.Threading.Tasks;

namespace CollectiveMind.TicTac3D.Runtime.Client.GameStateComponents
{
  public interface IGameState : IExitableState
  {
    UniTask Enter();
  }
}
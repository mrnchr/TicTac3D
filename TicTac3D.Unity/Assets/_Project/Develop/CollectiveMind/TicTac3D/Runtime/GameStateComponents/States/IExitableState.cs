using Cysharp.Threading.Tasks;

namespace CollectiveMind.TicTac3D.Runtime.GameStateComponents
{
  public interface IExitableState
  {
    UniTask Exit();
  }
}
using Cysharp.Threading.Tasks;

namespace CollectiveMind.TicTac3D.Runtime.Client.GameStateComponents
{
  public class EndGameState : IGameState
  {
    public EndGameState()
    {
      
    }
    
    public UniTask Enter()
    {
      return UniTask.CompletedTask;
    }

    public UniTask Exit()
    {
      return UniTask.CompletedTask;
    }
  }
}
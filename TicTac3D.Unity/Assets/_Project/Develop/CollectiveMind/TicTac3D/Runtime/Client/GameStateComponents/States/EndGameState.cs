using CollectiveMind.TicTac3D.Runtime.Client.Gameplay;
using CollectiveMind.TicTac3D.Runtime.Client.Gameplay.CameraRotation;
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay;
using CollectiveMind.TicTac3D.Runtime.Shared.Network;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CollectiveMind.TicTac3D.Runtime.Client.GameStateComponents
{
  public class EndGameState : IGameState
  {
    private readonly IGameplayTickableManager _gameplayTickableManager;
    private readonly IGameStateMachine _gameStateMachine;
    private readonly IFieldCleaner _fieldCleaner;
    private readonly IRpcProvider _rpcProvider;
    private readonly CameraRotator _cameraRotator;

    public EndGameState(IGameplayTickableManager gameplayTickableManager,
      IGameStateMachine gameStateMachine,
      IFieldCleaner fieldCleaner,
      IRpcProvider rpcProvider)
    {
      _gameplayTickableManager = gameplayTickableManager;
      _gameStateMachine = gameStateMachine;
      _fieldCleaner = fieldCleaner;
      _rpcProvider = rpcProvider;

      _cameraRotator = Object.FindAnyObjectByType<CameraRotator>();
    }
    
    public async UniTask Enter()
    {
      _gameplayTickableManager.IsPaused = true;
      _fieldCleaner.CleanField();
      _rpcProvider.SendRequest<LeaveGameRequest>();
      await _gameStateMachine.SwitchState<MenuGameState>();
    }

    public UniTask Exit()
    {
      _cameraRotator.ResetRotation();
      return UniTask.CompletedTask;
    }
  }
}
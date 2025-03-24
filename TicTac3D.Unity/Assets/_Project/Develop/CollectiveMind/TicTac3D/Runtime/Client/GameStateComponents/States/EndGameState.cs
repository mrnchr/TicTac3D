using CollectiveMind.TicTac3D.Runtime.Client.Gameplay;
using CollectiveMind.TicTac3D.Runtime.Client.Gameplay.CameraRotation;
using CollectiveMind.TicTac3D.Runtime.Client.UI;
using CollectiveMind.TicTac3D.Runtime.Client.WindowManagement;
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay;
using CollectiveMind.TicTac3D.Runtime.Shared.Network;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CollectiveMind.TicTac3D.Runtime.Client.GameStateComponents
{
  public class EndGameState : IGameState, IPaylodedState<LeaveGamePayload>
  {
    private readonly IGameplayTickableManager _gameplayTickableManager;
    private readonly IFieldCleaner _fieldCleaner;
    private readonly IRpcProvider _rpcProvider;
    private readonly IWindowManager _windowManager;
    private readonly IGameStateMachine _gameStateMachine;
    private readonly CameraRotator _cameraRotator;

    private bool _isLeaveGamePayloadReceived;

    public EndGameState(IGameplayTickableManager gameplayTickableManager,
      IFieldCleaner fieldCleaner,
      IRpcProvider rpcProvider,
      IWindowManager windowManager,
      IGameStateMachine gameStateMachine)
    {
      _gameplayTickableManager = gameplayTickableManager;
      _fieldCleaner = fieldCleaner;
      _rpcProvider = rpcProvider;
      _windowManager = windowManager;
      _gameStateMachine = gameStateMachine;

      _cameraRotator = Object.FindAnyObjectByType<CameraRotator>();
    }

    public async UniTask Enter()
    {
      _gameplayTickableManager.IsPaused = true;
      await _windowManager.OpenWindow<GameResultWindow>();
    }

    public async UniTask Enter(LeaveGamePayload payload)
    {
      _isLeaveGamePayloadReceived = true;
      _gameplayTickableManager.IsPaused = true;
      _rpcProvider.SendRequest<LeaveGameRequest>();
      await _gameStateMachine.SwitchState<MenuGameState>();
    }

    public async UniTask Exit()
    {
      _cameraRotator.ResetRotation();
      _fieldCleaner.CleanField();
      if (!_isLeaveGamePayloadReceived)
        _rpcProvider.SendRequest<LeaveGameRequest>();
      
      await _windowManager.CloseWindow<GameResultWindow>();
      _isLeaveGamePayloadReceived = false;
    }
  }
}
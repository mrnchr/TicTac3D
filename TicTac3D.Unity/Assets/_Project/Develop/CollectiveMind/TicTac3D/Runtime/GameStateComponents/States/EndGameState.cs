using CollectiveMind.TicTac3D.Runtime.Gameplay;
using CollectiveMind.TicTac3D.Runtime.LobbyManagement;
using CollectiveMind.TicTac3D.Runtime.Network;
using CollectiveMind.TicTac3D.Runtime.UI;
using CollectiveMind.TicTac3D.Runtime.WindowManagement;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CollectiveMind.TicTac3D.Runtime.GameStateComponents
{
  public class EndGameState : IGameState, IPaylodedState<LeaveGamePayload>
  {
    private readonly IGameplayTickableManager _gameplayTickableManager;
    private readonly IRpcProvider _rpcProvider;
    private readonly IWindowManager _windowManager;
    private readonly IGameStateMachine _gameStateMachine;
    private readonly LobbyManager _lobbyManager;
    private readonly FieldCreator _fieldCreator;
    private readonly CameraRotator _cameraRotator;

    private bool _isLeaveGamePayloadReceived;

    public EndGameState(IGameplayTickableManager gameplayTickableManager,
      IRpcProvider rpcProvider,
      IWindowManager windowManager,
      IGameStateMachine gameStateMachine,
      LobbyManager lobbyManager,
      FieldCreator fieldCreator)
    {
      _gameplayTickableManager = gameplayTickableManager;
      _rpcProvider = rpcProvider;
      _windowManager = windowManager;
      _gameStateMachine = gameStateMachine;
      _lobbyManager = lobbyManager;
      _fieldCreator = fieldCreator;

      _cameraRotator = Object.FindAnyObjectByType<CameraRotator>();
    }

    public async UniTask Enter()
    {
      _gameplayTickableManager.IsPaused = true;
      _lobbyManager.LeaveLobby().Forget();
      await _windowManager.OpenWindowAsRoot<GameResultWindow>();
    }

    public async UniTask Enter(LeaveGamePayload payload)
    {
      _isLeaveGamePayloadReceived = true;
      _gameplayTickableManager.IsPaused = true;
      _rpcProvider.SendRequest<LeaveGameRequest>();
      _lobbyManager.LeaveLobby().Forget();
      await _gameStateMachine.SwitchState<MenuGameState>();
    }

    public async UniTask Exit()
    {
      _cameraRotator.ResetRotation();
      _fieldCreator.CleanField();
      if (!_isLeaveGamePayloadReceived)
        _rpcProvider.SendRequest<LeaveGameRequest>();
      
      _isLeaveGamePayloadReceived = false;
      await _windowManager.CloseWindow<GameResultWindow>();
    }
  }
}
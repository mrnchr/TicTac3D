using CollectiveMind.TicTac3D.Runtime.GameStateComponents;
using CollectiveMind.TicTac3D.Runtime.WindowManagement;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace CollectiveMind.TicTac3D.Runtime.UI
{
  public class LeaveGameWindow : BaseWindow
  {
    [SerializeField]
    private Button _yesButton;
    
    [SerializeField]
    private Button _noButton;

    private IWindowManager _windowManager;
    private IGameStateMachine _gameStateMachine;

    [Inject]
    public void Construct(IWindowManager windowManager, IGameStateMachine gameStateMachine)
    {
      _windowManager = windowManager;
      _gameStateMachine = gameStateMachine;
      
      _yesButton.AddListener(LeaveGame);
      _noButton.AddListener(Back);
    }

    private void LeaveGame()
    {
      _gameStateMachine.SwitchState<EndGameState, LeaveGamePayload>(new LeaveGamePayload());
    }

    private void Back()
    {
      _windowManager.CloseWindow<LeaveGameWindow>();
    }

    private void OnDestroy()
    {
      _yesButton.RemoveListener(LeaveGame);
      _noButton.RemoveListener(Back);
    }
  }
}
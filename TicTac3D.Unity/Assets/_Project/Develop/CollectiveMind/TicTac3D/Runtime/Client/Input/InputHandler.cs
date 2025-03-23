using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Zenject;

namespace CollectiveMind.TicTac3D.Runtime.Client.Input
{
  public class InputHandler : ITickable
  {
    private readonly InputProvider _inputProvider;
    private readonly EventSystem _eventSystem;
    private readonly PlayerInputActions.GameplayActions _gameplayInputs;

    public InputHandler(InputProvider inputProvider,
      PlayerInputActions playerInputActions,
      PlayerInput playerInput,
      EventSystem eventSystem)
    {
      _inputProvider = inputProvider;
      _eventSystem = eventSystem;

      playerInput.actions = playerInputActions.asset;
      playerInputActions.Gameplay.Enable();
      _gameplayInputs = playerInputActions.Gameplay;
    }

    public void Tick()
    {
      _inputProvider.Reset();

      _inputProvider.Click = _gameplayInputs.Click.WasPerformedThisFrame() && !_eventSystem.IsPointerOverGameObject();
      _inputProvider.Rotate = _gameplayInputs.Rotate.ReadValue<float>() > 0;
      _inputProvider.Delta = _gameplayInputs.Delta.ReadValue<Vector2>();
      _inputProvider.MousePosition = _gameplayInputs.MousePosition.ReadValue<Vector2>();
    }
  }
}
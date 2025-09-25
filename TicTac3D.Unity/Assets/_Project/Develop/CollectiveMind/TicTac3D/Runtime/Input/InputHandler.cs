using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace CollectiveMind.TicTac3D.Runtime.Input
{
  public class InputHandler : ITickable
  {
    private readonly InputProvider _inputProvider;
    private readonly EventSystem _eventSystem;
    private readonly PlayerInputActions.GameplayActions _gameplayInputs;

    public InputHandler(InputProvider inputProvider,
      PlayerInputActions playerInputActions,
      EventSystem eventSystem)
    {
      _inputProvider = inputProvider;
      _eventSystem = eventSystem;

      playerInputActions.Gameplay.Enable();
      playerInputActions.UI.Enable();
      _gameplayInputs = playerInputActions.Gameplay;
    }

    public void Tick()
    {
      bool wasTouch = _inputProvider.Touch;
      _inputProvider.Reset();

      _inputProvider.PointerPosition = _gameplayInputs.PointerPosition.ReadValue<Vector2>();
      _inputProvider.Rotate = _gameplayInputs.Rotate.ReadValue<float>() > 0;
      _inputProvider.RotateValue = _gameplayInputs.RotateValue.ReadValue<Vector2>();
      
      _inputProvider.Click = (!OverridenPlatformSettings.IsMobilePlatform || !_inputProvider.Rotate)
        && _gameplayInputs.Click.WasPerformedThisFrame() && !_eventSystem.IsPointerOverGameObject();

      _inputProvider.Touch = !OverridenPlatformSettings.IsMobilePlatform
        || (_gameplayInputs.Touch.ReadValue<float>() > 0 && !_inputProvider.Rotate
          && (_gameplayInputs.Touch.WasPerformedThisFrame() || wasTouch));
    }
  }
}
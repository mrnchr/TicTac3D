using CollectiveMind.TicTac3D.Runtime.Gameplay;
using CollectiveMind.TicTac3D.Runtime.Input;
using CollectiveMind.TicTac3D.Runtime.WindowManagement;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace CollectiveMind.TicTac3D.Runtime.UI
{
  public class ConfirmationPopup : MonoBehaviour
  {
    [SerializeField]
    private Button _yesButton;

    [SerializeField]
    private Button _noButton;

    [SerializeField]
    private RectTransform _mouseTarget;

    private ConfirmationContext _confirmationContext;
    private IGameplayTickableManager _gameplayTickableManager;
    private InputProvider _input;

    private RectTransform _rectTransform;

    [Inject]
    public void Construct(ConfirmationContext confirmationContext,
      IGameplayTickableManager gameplayTickableManager,
      InputProvider input)
    {
      _confirmationContext = confirmationContext;
      _gameplayTickableManager = gameplayTickableManager;
      _input = input;

      _rectTransform = GetComponent<RectTransform>();

      _confirmationContext.OnAsked += ShowPopup;
      _yesButton.AddListener(Confirm);
      _noButton.AddListener(DenyAndContinue);
    }

    private async void ShowPopup()
    {
      await UniTask.NextFrame();
      _gameplayTickableManager.IsPaused = true;
      MoveToMousePosition();
      gameObject.SetActive(true);
    }

    private void MoveToMousePosition()
    {
      Vector2 delta = _rectTransform.position - _mouseTarget.position;
      Vector3 targetPosition = _input.MousePosition + delta;

      Rect rect = _rectTransform.rect;
      rect.center = targetPosition;
      var points = new Vector4(rect.xMin, rect.yMin, rect.xMax, rect.yMax);
      ClampToScreen(0, Screen.width);
      ClampToScreen(1, Screen.height);

      targetPosition.z = _rectTransform.position.z;
      _rectTransform.position = targetPosition;

      return;

      void ClampToScreen(int index, int clamp)
      {
        targetPosition[index] = Clamp(targetPosition[index], new Vector2(0, clamp),
          new Vector2(points[index], points[index + 2]));
      }

      float Clamp(float value, Vector2 bound, Vector2 offset)
      {
        return Mathf.Clamp(value, value + bound.x - offset.x, value + bound.y - offset.y);
      }
    }

    private void HidePopup(bool continueGame)
    {
      if (continueGame)
        _gameplayTickableManager.IsPaused = false;

      gameObject.SetActive(false);
    }

    private void Confirm()
    {
      HidePopup(true);
      _confirmationContext.Answer(true);
    }

    private void DenyAndContinue()
    {
      Deny(true);
    }

    public void Deny(bool continueGame)
    {
      HidePopup(continueGame);
      _confirmationContext.Answer(false);
    }

    private void OnDestroy()
    {
      _confirmationContext.OnAsked -= ShowPopup;
      _yesButton.RemoveListener(Confirm);
      _noButton.RemoveListener(DenyAndContinue);
    }
  }
}
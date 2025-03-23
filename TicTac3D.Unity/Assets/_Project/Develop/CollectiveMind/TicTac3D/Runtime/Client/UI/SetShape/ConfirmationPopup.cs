using CollectiveMind.TicTac3D.Runtime.Client.Gameplay;
using CollectiveMind.TicTac3D.Runtime.Client.WindowManagement;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace CollectiveMind.TicTac3D.Runtime.Client.UI.SetShape
{
  public class ConfirmationPopup : MonoBehaviour
  {
    [SerializeField]
    private Button _yesButton;

    [SerializeField]
    private Button _noButton;

    private ConfirmationContext _confirmationContext;
    private IGameplayTickableManager _gameplayTickableManager;

    [Inject]
    public void Construct(ConfirmationContext confirmationContext,
      IGameplayTickableManager gameplayTickableManager)
    {
      _confirmationContext = confirmationContext;
      _gameplayTickableManager = gameplayTickableManager;

      _confirmationContext.OnAsked += ShowPopup;
      _yesButton.AddListener(Confirm);
      _noButton.AddListener(DenyAndContinue);
    }

    private void ShowPopup()
    {
      _gameplayTickableManager.IsPaused = true;
      gameObject.SetActive(true);
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
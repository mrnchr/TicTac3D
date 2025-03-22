using CollectiveMind.TicTac3D.Runtime.Client.WindowManagement;
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay.Rules;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace CollectiveMind.TicTac3D.Runtime.Client.UI.LobbySettings
{
  public class SelectShapeFadingButton : MonoBehaviour
  {
    public ShapeFadingType ShapeFading;

    private LobbySettingsWindow _lobbyWindow;
    private Button _button;

    [Inject]
    public void Construct()
    {
      _lobbyWindow = GetComponentInParent<LobbySettingsWindow>();
      _button = GetComponent<Button>();

      _button.AddListener(SelectShapeFading);
    }

    private void SelectShapeFading()
    {
      _lobbyWindow.SelectShapeFading(ShapeFading);
    }
  }
}
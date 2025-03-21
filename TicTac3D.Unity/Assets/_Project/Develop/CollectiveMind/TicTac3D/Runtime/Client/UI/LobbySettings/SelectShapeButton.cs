using CollectiveMind.TicTac3D.Runtime.Client.WindowManagement;
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay.Shape;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace CollectiveMind.TicTac3D.Runtime.Client.UI.LobbySettings
{
  public class SelectShapeButton : MonoBehaviour
  {
    public ShapeType Shape;

    private LobbySettingsWindow _lobbyWindow;
    private Button _button;

    [Inject]
    public void Construct()
    {
      _lobbyWindow = GetComponentInParent<LobbySettingsWindow>(true);
      _button = GetComponent<Button>();

      _button.AddListener(SelectShape);
    }

    private void SelectShape()
    {
      _lobbyWindow.SelectShape(Shape);
    }
  }
}
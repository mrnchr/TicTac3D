using CollectiveMind.TicTac3D.Runtime.Client.Gameplay;
using CollectiveMind.TicTac3D.Runtime.Client.WindowManagement;
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay;
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay.Rules;
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay.Shape;
using CollectiveMind.TicTac3D.Runtime.Shared.Network;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace CollectiveMind.TicTac3D.Runtime.Client.UI.LobbySettings
{
  public class LobbySettingsWindow : BaseWindow
  {
    [SerializeField]
    private Button _searchGameButton;

    [SerializeField]
    private Button _backButton;

    private GameRulesProvider _rulesProvider;
    private IWindowManager _windowManager;
    private IRpcProvider _rpcProvider;

    private GameRules Rules => _rulesProvider.Rules;

    [Inject]
    public void Construct(GameRulesProvider gameRulesProvider, IWindowManager windowManager, IRpcProvider rpcProvider)
    {
      _rulesProvider = gameRulesProvider;
      _windowManager = windowManager;
      _rpcProvider = rpcProvider;

      _searchGameButton.AddListener(SearchGame);
      _backButton.AddListener(CloseWindow);
    }

    public void SelectShape(ShapeType shapeType)
    {
      Rules.Data.DesiredShape = shapeType;
    }

    public void SelectBotMoveCount(int count)
    {
      Rules.Data.BotMoveCount = count;
    }

    public void SelectMoveTime(float time)
    {
      Rules.Data.MoveTime = time;
    }

    public void SelectShapeFading(ShapeFadingType fading)
    {
      Rules.Data.ShapeFading = fading;
    }

    public void SelectFadingMoveCount(int count)
    {
      Rules.Data.FadingMoveCount = count;
    }

    private void SearchGame()
    {
      _rpcProvider.SendRequest(new SearchGameRequest { Rules = Rules.Data });
      _windowManager.OpenWindow<SearchGameWindow>();
    }

    private void CloseWindow()
    {
      _windowManager.CloseWindow<LobbySettingsWindow>().Forget();
    }

    private void OnDestroy()
    {
      _searchGameButton.RemoveListener(SearchGame);
      _backButton.RemoveListener(CloseWindow);
    }
  }
}
using CollectiveMind.TicTac3D.Runtime.Gameplay;
using CollectiveMind.TicTac3D.Runtime.Network;
using CollectiveMind.TicTac3D.Runtime.WindowManagement;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace CollectiveMind.TicTac3D.Runtime.UI
{
  public class SearchGameWindow : BaseWindow
  {
    [SerializeField]
    private Button _backButton;

    private IWindowManager _windowManager;
    private IRpcProvider _rpcProvider;

    [Inject]
    public void Construct(IWindowManager windowManager, IRpcProvider rpcProvider)
    {
      _windowManager = windowManager;
      _rpcProvider = rpcProvider;
      
      _backButton.AddListener(CloseWindow);
    }

    private void CloseWindow()
    {
      _rpcProvider.SendRequest<StopSearchGameRequest>();
      _windowManager.CloseWindow<SearchGameWindow>();
    }
  }
}
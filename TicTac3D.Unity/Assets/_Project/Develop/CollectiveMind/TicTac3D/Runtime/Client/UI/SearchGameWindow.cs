using CollectiveMind.TicTac3D.Runtime.Client.WindowManagement;
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay;
using CollectiveMind.TicTac3D.Runtime.Shared.Network;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace CollectiveMind.TicTac3D.Runtime.Client.UI
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
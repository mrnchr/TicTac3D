using System.Threading;
using CollectiveMind.TicTac3D.Runtime.Gameplay;
using CollectiveMind.TicTac3D.Runtime.LobbyManagement;
using CollectiveMind.TicTac3D.Runtime.Network;
using CollectiveMind.TicTac3D.Runtime.WindowManagement;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.SmartFormat.PersistentVariables;
using UnityEngine.UI;
using Zenject;

namespace CollectiveMind.TicTac3D.Runtime.UI
{
  public class SearchGameWindow : BaseWindow
  {
    [SerializeField]
    private LocalizeStringEvent _joinCodeText;

    [SerializeField]
    private Button _copyCodeButton;

    [SerializeField]
    private Button _backButton;

    private IWindowManager _windowManager;
    private IRpcProvider _rpcProvider;
    private LobbyManager _lobbyManager;
    private CancellationTokenSource _cts;

    [Inject]
    public void Construct(IWindowManager windowManager, IRpcProvider rpcProvider, LobbyManager lobbyManager)
    {
      _windowManager = windowManager;
      _rpcProvider = rpcProvider;
      _lobbyManager = lobbyManager;

      _copyCodeButton.AddListener(CopyJoinCode);
      _backButton.AddListener(CloseWindow);
    }

    private void CopyJoinCode()
    {
      GUIUtility.systemCopyBuffer = _lobbyManager.JoinCode;
    }

    protected override UniTask OnVisible()
    {
      _joinCodeText.gameObject.SetActive(false);
      _copyCodeButton.gameObject.SetActive(false);

      _cts = new CancellationTokenSource();
      WaitForJoinCode(_cts.Token).Forget();
      return UniTask.CompletedTask;
    }

    private async UniTask WaitForJoinCode(CancellationToken token = default(CancellationToken))
    {
      if (_lobbyManager.IsLobbyCreating)
      {
        await UniTask.WaitUntil(() => _lobbyManager.IsLobbyCreated, cancellationToken: token)
          .SuppressCancellationThrow();
        if (token.IsCancellationRequested)
          return;

        _joinCodeText.gameObject.SetActive(true);
        _copyCodeButton.gameObject.SetActive(true);

        _joinCodeText.StringReference[NC.JOIN_CODE_NAME] = new StringVariable { Value = _lobbyManager.JoinCode };
        _joinCodeText.RefreshString();
      }
    }

    private void CloseWindow()
    {
      _lobbyManager.CancelSearch();
      _rpcProvider.SendRequest<StopSearchGameRequest>();
      _cts = _cts?.CancelDisposeAndForget();
      _windowManager.CloseWindow<SearchGameWindow>().Forget();
    }

    private void OnDestroy()
    {
      _cts = _cts?.CancelDisposeAndForget();
      _copyCodeButton.RemoveListener(CopyJoinCode);
      _backButton.RemoveListener(CloseWindow);
    }
  }
}
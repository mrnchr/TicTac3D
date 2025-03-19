using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace CollectiveMind.TicTac3D.Runtime.Client.UI
{
  public class MenuWindow : MonoBehaviour
  {
    [SerializeField]
    private Button _hostButton;

    [SerializeField]
    private Button _clientButton;

    private NetworkManager _networkManager;

    [Inject]
    public void Construct(NetworkManager networkManager)
    {
      _networkManager = networkManager;

      _hostButton.onClick.AddListener(SwitchHost);
      _clientButton.onClick.AddListener(SwitchClient);
    }

    private void SwitchHost()
    {
      if (_networkManager.IsListening)
        _networkManager.Shutdown();
      else
        _networkManager.StartHost();
    }

    private void SwitchClient()
    {
      if (_networkManager.IsListening)
        _networkManager.Shutdown();
      else
        _networkManager.StartClient();
    }

    private void Update()
    {
      _hostButton.interactable = _networkManager.IsHost || !_networkManager.IsListening;
      _clientButton.interactable = !_networkManager.IsServer || !_networkManager.IsListening;
    }

    private void OnDestroy()
    {
      _hostButton.onClick.RemoveListener(SwitchHost);
      _clientButton.onClick.RemoveListener(SwitchClient);
    }
  }
}
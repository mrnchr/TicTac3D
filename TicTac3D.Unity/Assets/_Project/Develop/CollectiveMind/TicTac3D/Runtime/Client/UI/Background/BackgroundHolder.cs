using CollectiveMind.TicTac3D.Runtime.Shared.AssetManagement;
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay;
using CollectiveMind.TicTac3D.Runtime.Shared.Network;
using CollectiveMind.TicTac3D.Runtime.Shared.UI;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace CollectiveMind.TicTac3D.Runtime.Client.UI.Background
{
  public class BackgroundHolder : MonoBehaviour
  {
    private INetworkBus _networkBus;
    private IConfigLoader _configLoader;
    private BackgroundConfig _config;
    private Image _image;

    [Inject]
    public void Construct(IConfigLoader configLoader, INetworkBus networkBus)
    {
      _configLoader = configLoader;
      _networkBus = networkBus;
      _config = _configLoader.LoadConfig<BackgroundConfig>();
      _image = GetComponent<Image>();
      
      _networkBus.SubscribeOnRpcWithParameter<ChangeBackgroundResponse>(ChangeBackground);
    }

    private void ChangeBackground(ChangeBackgroundResponse response)
    {
      _image.sprite = _config.Backgrounds[response.Index];
    }

    private void OnDestroy()
    {
      _configLoader.UnloadConfig<BackgroundConfig>();
      _networkBus.UnsubscribeFromRpc<ChangeBackgroundResponse>();
    }
  }
}
using CollectiveMind.TicTac3D.Runtime.Client.Gameplay;
using CollectiveMind.TicTac3D.Runtime.Shared.AssetManagement;
using CollectiveMind.TicTac3D.Runtime.Shared.UI;
using R3;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace CollectiveMind.TicTac3D.Runtime.Client.UI.Background
{
  public class BackgroundHolder : MonoBehaviour
  {
    private IConfigLoader _configLoader;
    private GameInfo _gameInfo;
    private BackgroundConfig _config;
    private Image _image;

    [Inject]
    public void Construct(IConfigLoader configLoader, GameInfo gameInfo)
    {
      _configLoader = configLoader;
      _gameInfo = gameInfo;
      _config = _configLoader.LoadConfig<BackgroundConfig>();
      _image = GetComponent<Image>();

      _gameInfo.BackgroundIndex.Subscribe(ChangeBackground);
    }

    private void ChangeBackground(int index)
    {
      _image.sprite = _config.Backgrounds[index];
    }

    private void OnDestroy()
    {
      _configLoader.UnloadConfig<BackgroundConfig>();
    }
  }
}
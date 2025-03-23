using CollectiveMind.TicTac3D.Runtime.Client.Input;
using CollectiveMind.TicTac3D.Runtime.Client.UI.Settings;
using CollectiveMind.TicTac3D.Runtime.Shared.AssetManagement;
using UnityEngine;
using Zenject;

namespace CollectiveMind.TicTac3D.Runtime.Client.Gameplay.CameraRotation
{
  public class CameraRotator : MonoBehaviour, IGameplayTickable
  {
    private InputProvider _inputProvider;

    private Vector2 _angles;
    private IConfigLoader _configLoader;
    private RotationConfig _rotationConfig;
    private SettingsDataProvider _settingsDataProvider;
    private IGameplayTickableManager _gameplayTickableManager;

    [Inject]
    public void Construct(InputProvider inputProvider,
      IConfigLoader configLoader,
      SettingsDataProvider settingsDataProvider,
      IGameplayTickableManager gameplayTickableManager)
    {
      _inputProvider = inputProvider;
      _configLoader = configLoader;
      _settingsDataProvider = settingsDataProvider;
      _gameplayTickableManager = gameplayTickableManager;
      _rotationConfig = configLoader.LoadConfig<RotationConfig>();

      _gameplayTickableManager.Add(this);
    }

    public void Tick()
    {
      if (_inputProvider.Rotate)
      {
        Vector2 delta = new Vector2(-_inputProvider.Delta.y, _inputProvider.Delta.x);

        Vector2 rawFrameVelocity = delta * _settingsDataProvider.Data.MouseSensitivity.Value;
        Vector2 frameVelocity = Vector2.Lerp(Vector2.zero, rawFrameVelocity, 1 / _rotationConfig.Smoothing);
            
        _angles += frameVelocity;
        _angles.x = Mathf.Clamp(_angles.x, -90, 90);

        ApplyRotation();
      }
    }

    public void ResetRotation()
    {
      _angles = Vector2.zero;
      ApplyRotation();
    }

    private void ApplyRotation()
    {
      transform.rotation = Quaternion.Euler(_angles);
    }

    private void OnDestroy()
    {
      _configLoader.UnloadConfig<RotationConfig>();
      _gameplayTickableManager.Remove(this);
    }
  }
}
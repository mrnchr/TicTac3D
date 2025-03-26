using System;
using System.Linq;
using CollectiveMind.TicTac3D.Runtime.Shared.AssetManagement;
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay.Cell;
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay.Rules;
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay.Shape;
using R3;
using UnityEngine;
using Zenject;

namespace CollectiveMind.TicTac3D.Runtime.Client.Gameplay.Shape
{
  public class ShapeVisual : MonoBehaviour
  {
    private CellModel _cell;
    private GameInfo _gameInfo;
    private MeshRenderer _renderer;
    private IDisposable _disposable;
    private IConfigLoader _configLoader;
    private GameConfig _config;

    [Inject]
    public void Construct(CellModel cell, GameInfo gameInfo, IConfigLoader configLoader)
    {
      _cell = cell;
      _gameInfo = gameInfo;
      _configLoader = configLoader;
      _config = configLoader.LoadConfig<GameConfig>();

      _renderer = GetComponentInChildren<MeshRenderer>(true);

      _disposable = _cell.LifeTime.Subscribe(ChangeTransparency);
    }

    private void ChangeTransparency(int lifeTime)
    {
      if (_gameInfo.Rules.Data.ShapeFading <= ShapeFadingType.Off)
        return;

      Color color = _renderer.material.color;
      color.a = (float)lifeTime / (_cell.Shape.Value == ShapeType.XO
        ? _gameInfo.Rules.Data.FadingMoveCount
        : _config.PlayerShapesLifeTime);

      _renderer.material.color = color;
    }

    private void OnDestroy()
    {
      _disposable?.Dispose();
      _configLoader.UnloadConfig<GameConfig>();
    }
  }
}
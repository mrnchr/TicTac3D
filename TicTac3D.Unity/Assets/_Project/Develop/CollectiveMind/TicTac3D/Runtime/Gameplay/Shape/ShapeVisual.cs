using System;
using R3;
using UnityEngine;
using Zenject;

namespace CollectiveMind.TicTac3D.Runtime.Gameplay
{
  public class ShapeVisual : MonoBehaviour
  {
    private CellModel _cell;
    private GameInfo _gameInfo;
    private MeshRenderer _renderer;
    private IDisposable _disposable;

    [Inject]
    public void Construct(CellModel cell, GameInfo gameInfo)
    {
      _cell = cell;
      _gameInfo = gameInfo;

      _renderer = GetComponentInChildren<MeshRenderer>(true);

      _disposable = _cell.LifeTime.Subscribe(ChangeTransparency);
    }

    private void ChangeTransparency(int lifeTime)
    {
      ShapeFadingType fading = _gameInfo.Rules.Data.ShapeFading;
      if (fading <= ShapeFadingType.Off || _cell.Shape.Value == ShapeType.None)
        return;

      Color color = _renderer.material.color;
      var divider = 1;
      if(fading.IsBot() && _cell.Shape.Value == ShapeType.XO)
        divider = _gameInfo.Rules.Data.BotFadingMoveCount;
      
      if(fading.IsPlayers() && _cell.Shape.Value is ShapeType.X or ShapeType.O)
        divider = _gameInfo.Rules.Data.PlayerFadingMoveCount;

      color.a = (float)lifeTime / divider;

      _renderer.material.color = color;
    }

    private void OnDestroy()
    {
      _disposable?.Dispose();
    }
  }
}
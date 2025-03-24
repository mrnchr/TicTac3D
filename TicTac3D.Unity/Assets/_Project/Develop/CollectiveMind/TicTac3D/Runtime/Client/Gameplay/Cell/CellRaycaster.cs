using System;
using System.Collections.Generic;
using System.Linq;
using CollectiveMind.TicTac3D.Runtime.Client.Input;
using CollectiveMind.TicTac3D.Runtime.Shared.AssetManagement;
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay.Cell;
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay.Shape;
using UnityEngine;

namespace CollectiveMind.TicTac3D.Runtime.Client.Gameplay.Cell
{
  public class CellRaycaster : IGameplayTickable, IDisposable
  {
    private readonly InputProvider _inputProvider;
    private readonly List<CellModel> _cells;
    private readonly IConfigLoader _configLoader;
    private readonly GameInfo _gameInfo;
    private readonly Camera _camera;
    private readonly CellConfig _config;

    public CellRaycaster(InputProvider inputProvider,
      List<CellModel> cells,
      IConfigLoader configLoader,
      GameInfo gameInfo)
    {
      _inputProvider = inputProvider;
      _cells = cells;
      _configLoader = configLoader;
      _gameInfo = gameInfo;
      _camera = Camera.main;
      _config = _configLoader.LoadConfig<CellConfig>();
    }

    public void Tick()
    {
      ClearHovering();
      
      if (!_gameInfo.IsMoving || _gameInfo.CurrentMove.Value == ShapeType.None)
        return;
      
      Ray ray = _camera.ScreenPointToRay(_inputProvider.MousePosition);
      var minDistance = float.MaxValue;
      CellModel hoveredCell = null;
      foreach (CellModel cell in _cells.Where(x => !x.HasShape()))
      {
        float distance = FindDistance(cell.Position, ray);
        if (minDistance > distance)
        {
          hoveredCell = cell;
          minDistance = distance;
        }
      }
      
      if (minDistance <= _config.MaxRaycastDistance && hoveredCell != null && !hoveredCell.HasShape())
        hoveredCell.IsHovered.Value = true;
    }

    private void ClearHovering()
    {
      foreach (CellModel cell in _cells.Where(x => x.IsHovered.Value))
      {
        cell.IsHovered.Value = false;
      }
    }

    private static float FindDistance(Vector3 point, Ray ray)
    {
      Vector3 pointToRayOrigin = point - ray.origin;
      Vector3 crossProduct = Vector3.Cross(pointToRayOrigin, ray.direction);
      float distance = crossProduct.magnitude / ray.direction.magnitude;
      return distance;
    }

    public void Dispose()
    {
      _configLoader.UnloadConfig<CellConfig>();
    }
  }
}
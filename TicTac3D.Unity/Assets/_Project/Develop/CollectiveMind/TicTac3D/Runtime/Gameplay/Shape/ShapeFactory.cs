using System;
using System.Collections.Generic;
using CollectiveMind.TicTac3D.Runtime.AssetManagement;
using UnityEngine;
using Zenject;

namespace CollectiveMind.TicTac3D.Runtime.Gameplay
{
  public class ShapeFactory : IShapeFactory, IDisposable
  {
    private readonly Dictionary<ShapeType, ShapeVisual> _shapePrefabs = new Dictionary<ShapeType, ShapeVisual>();
    private readonly IInstantiator _instantiator;
    private readonly IPrefabLoader _prefabLoader;

    public ShapeFactory(IInstantiator instantiator, IPrefabLoader prefabLoader)
    {
      _instantiator = instantiator;
      _prefabLoader = prefabLoader;

      for (var i = EntityType.XShape; i <= EntityType.XOShape; i++)
      {
        _shapePrefabs.Add((ShapeType)(i - EntityType.XShape + 1), _prefabLoader.LoadPrefab<ShapeVisual>(i));
      }
    }

    public ShapeVisual Create(ShapeType id, Vector3 position, Transform parent, CellModel model)
    {
      return _instantiator.InstantiatePrefabForComponent<ShapeVisual>(_shapePrefabs[id], position, Quaternion.identity,
        parent, new[] { model });
    }

    public void Dispose()
    {
      for (var i = EntityType.XShape; i <= EntityType.XOShape; i++)
      {
        _prefabLoader.UnloadPrefab(i);
      }
    }
  }
}
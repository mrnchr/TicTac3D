using System;
using CollectiveMind.TicTac3D.Runtime.AssetManagement;
using UnityEngine;
using Zenject;

namespace CollectiveMind.TicTac3D.Runtime.Gameplay
{
  public class CellVisualFactory : ICellVisualFactory, IDisposable
  {
    private readonly IInstantiator _instantiator;
    private readonly IPrefabLoader _prefabLoader;
    private readonly CellVisual _prefab;
    private readonly Transform _parent;

    public CellVisualFactory(IInstantiator instantiator, IPrefabLoader prefabLoader)
    {
      _instantiator = instantiator;
      _prefabLoader = prefabLoader;

      _prefab = _prefabLoader.LoadPrefab<CellVisual>(EntityType.Cell);
      _parent = new GameObject("Cells").transform;
    }

    public CellVisual Create(CellModel model)
    {
      return _instantiator.InstantiatePrefabForComponent<CellVisual>(_prefab, model.Position, Quaternion.identity,
        _parent, new[] { model });
    }

    public void Dispose()
    {
      _prefabLoader.UnloadPrefab(EntityType.Cell);
    }
  }
}
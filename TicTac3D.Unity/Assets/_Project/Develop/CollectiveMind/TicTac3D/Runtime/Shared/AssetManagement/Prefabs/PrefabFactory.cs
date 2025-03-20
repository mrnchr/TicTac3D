using UnityEngine;
using Zenject;

namespace CollectiveMind.TicTac3D.Runtime.Shared.AssetManagement
{
  public class PrefabFactory : IPrefabFactory
  {
    private readonly IInstantiator _instantiator;
    private readonly IPrefabLoader _prefabLoader;

    public PrefabFactory(IInstantiator instantiator, IPrefabLoader prefabLoader)
    {
      _instantiator = instantiator;
      _prefabLoader = prefabLoader;
    }

    public TObject Create<TObject>(EntityType id)
    {
      var prefab = _prefabLoader.LoadPrefab<GameObject>(id);
      var instance = _instantiator.InstantiatePrefabForComponent<TObject>(prefab);
      _prefabLoader.UnloadPrefab(id);
      return instance;
    }
  }
}
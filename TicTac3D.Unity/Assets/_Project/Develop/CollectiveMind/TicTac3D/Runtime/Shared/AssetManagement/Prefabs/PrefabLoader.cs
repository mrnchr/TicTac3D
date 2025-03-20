using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CollectiveMind.TicTac3D.Runtime.Shared.AssetManagement
{
  public class PrefabLoader : IDisposable, IPrefabLoader
  {
    private readonly IConfigLoader _configLoader;

    private readonly Dictionary<EntityType, PrefabReference> _loadedPrefabs =
      new Dictionary<EntityType, PrefabReference>();

    private readonly PrefabProvider _provider;

    public PrefabLoader(IConfigLoader configLoader)
    {
      _configLoader = configLoader;
      _provider = configLoader.LoadConfig<PrefabProvider>();
    }

    public TObject LoadPrefab<TObject>(EntityType prefabId) where TObject : Object
    {
      if (!_loadedPrefabs.TryGetValue(prefabId, out PrefabReference reference))
      {
        string path = _provider.GetPrefabPath(prefabId);
        var prefab = Resources.Load<GameObject>(path);
        reference = new PrefabReference { Prefab = prefab, ReferenceCount = 0 };
        _loadedPrefabs[prefabId] = reference;
      }

      reference.ReferenceCount++;
      return reference.Prefab as TObject ?? reference.Prefab.GetComponent<TObject>();
    }

    public void UnloadPrefab(EntityType prefabId)
    {
      bool unloaded = false;
      if (_loadedPrefabs.TryGetValue(prefabId, out PrefabReference reference))
      {
        reference!.ReferenceCount--;
        if (reference.ReferenceCount <= 0)
        {
          _loadedPrefabs.Remove(prefabId);
          unloaded = true;
        }
      }

      if (unloaded)
        Resources.UnloadUnusedAssets();
    }

    public void Dispose()
    {
      _configLoader.UnloadConfig<PrefabProvider>();
    }
  }

  public class PrefabReference
  {
    public GameObject Prefab;
    public int ReferenceCount;
  }
}
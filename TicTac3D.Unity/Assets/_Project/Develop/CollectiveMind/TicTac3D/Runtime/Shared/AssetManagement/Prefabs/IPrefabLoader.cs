using UnityEngine;

namespace CollectiveMind.TicTac3D.Runtime.Shared.AssetManagement
{
  public interface IPrefabLoader
  {
    TObject LoadPrefab<TObject>(EntityType prefabId) where TObject : Object;
    void UnloadPrefab(EntityType prefabId);
  }
}
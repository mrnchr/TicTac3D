using System;
using System.Collections.Generic;
using CollectiveMind.TicTac3D.Runtime.Utils;
using TriInspector;
using UnityEngine;

namespace CollectiveMind.TicTac3D.Runtime.AssetManagement
{
  [CreateAssetMenu(menuName = CAC.Names.PREFAB_PROVIDER_MENU, fileName = "PrefabProvider")]
  public class PrefabProvider : ScriptableObject
  {
    [SerializeField]
    private List<PrefabTuple> _prefabs;

    public string GetPrefabPath(EntityType prefabId)
    {
      return _prefabs.Find(x => x.Id == prefabId).Path;
    }
    
    [Serializable]
    [DeclareHorizontalGroup(nameof(PrefabTuple))]
    private class PrefabTuple
    {
      [GroupNext(nameof(PrefabTuple))]
      [HideLabel]
      public EntityType Id;

      [HideLabel]
      [ResourcePath(ResourceType = typeof(GameObject))]
      public string Path;
    }
  }
}
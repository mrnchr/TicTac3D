using System;
using System.Collections.Generic;
using System.Linq;
using CollectiveMind.TicTac3D.Runtime.Utils;
using TriInspector;
using UnityEngine;

namespace CollectiveMind.TicTac3D.Runtime.AssetManagement
{
  [CreateAssetMenu(menuName = CAC.Names.CONFIG_PROVIDER_MENU, fileName = "ConfigProvider")]
  public class ConfigProvider : ScriptableObject
  {
    [SerializeField]
    private List<ConfigPathPair> _configs;

    public string GetConfigPath<TConfig>() where TConfig : ScriptableObject
    {
      return _configs.Find(x => x.TypeName == typeof(TConfig).Name).Path;
    }
    
    [Serializable]
    [DeclareHorizontalGroup(nameof(ConfigPathPair))]
    private class ConfigPathPair
    {
      [Dropdown("GetConfigNames")]
      [GroupNext(nameof(ConfigPathPair))]
      [HideLabel]
      public string TypeName;

      [HideLabel]
      [ResourcePath(ResourceType = typeof(ScriptableObject))]
      public string Path;

#if UNITY_EDITOR
      private string[] GetConfigNames()
      {
        return Resources.LoadAll<ScriptableObject>("")
          .Select(x => x.GetType().Name)
          .Distinct()
          .OrderBy(x => x)
          .ToArray();
      }
#endif
    }
  }
}
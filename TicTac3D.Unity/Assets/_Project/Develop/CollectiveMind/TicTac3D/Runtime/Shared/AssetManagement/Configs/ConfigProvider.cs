using System;
using System.Collections.Generic;
using System.Linq;
using CollectiveMind.TicTac3D.Runtime.Shared.Utils;
using TriInspector;
using UnityEngine;

namespace CollectiveMind.TicTac3D.Runtime.Shared.AssetManagement
{
  [CreateAssetMenu(menuName = CAC.Names.CONFIG_PROVIDER_MENU, fileName = CAC.Names.CONFIG_PROVIDER_FILE)]
  public class ConfigProvider : ScriptableObject
  {
    [SerializeField]
    private List<ConfigTuple> _configs;

    public string GetConfigPath<TConfig>() where TConfig : ScriptableObject
    {
      return _configs.Find(x => x.TypeName == typeof(TConfig).Name).Path;
    }
  }

  [Serializable]
  [DeclareHorizontalGroup(nameof(ConfigTuple))]
  public class ConfigTuple
  {
    [Dropdown("GetConfigNames")]
    [GroupNext(nameof(ConfigTuple))]
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
        .ToArray();
    }
#endif
  }
}
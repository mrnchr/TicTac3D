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
      return _configs.Find(x => x.ConfigType == typeof(TConfig).Name).ConfigPath;
    }
  }

  [Serializable]
  [DeclareHorizontalGroup(nameof(ConfigTuple))]
  public class ConfigTuple
  {
    [Dropdown("GetConfigNames")]
    [GroupNext(nameof(ConfigType))]
    [HideLabel]
    public string ConfigType;

    [HideLabel]
    [ResourcePath]
    public string ConfigPath;

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
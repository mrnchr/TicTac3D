using System;
using System.Collections.Generic;
using UnityEngine;

namespace CollectiveMind.TicTac3D.Runtime.Shared.AssetManagement
{
  public class ConfigLoader : IConfigLoader, IDisposable
  {
    private readonly Dictionary<Type, ConfigReference> _loadedConfigs = new Dictionary<Type, ConfigReference>();
    private readonly ConfigProvider _provider;

    public ConfigLoader()
    {
      _provider = LoadConfig<ConfigProvider>(RC.CONFIG_PROVIDER);
    }

    public TConfig LoadConfig<TConfig>() where TConfig : ScriptableObject
    {
      string path = _provider.GetConfigPath<TConfig>();
      if (_loadedConfigs.TryGetValue(typeof(TConfig), out ConfigReference reference))
      {
        reference.ReferenceCount++;
        return (TConfig)reference.Config;
      }
      
      var config = Resources.Load<TConfig>(path);
      _loadedConfigs.Add(typeof(TConfig), new ConfigReference{ Config = config, ReferenceCount = 1 });
      return config;
    }
    
    public TConfig LoadConfig<TConfig>(string path) where TConfig : ScriptableObject
    {
      if (_loadedConfigs.TryGetValue(typeof(TConfig), out ConfigReference reference))
      {
        reference.ReferenceCount++;
        return (TConfig)reference.Config;
      }
      
      var config = Resources.Load<TConfig>(path);
      _loadedConfigs.Add(typeof(TConfig), new ConfigReference{ Config = config, ReferenceCount = 1 });
      return config;
    }

    public void UnloadConfig<TConfig>() where TConfig : ScriptableObject
    {
      if(_loadedConfigs.TryGetValue(typeof(TConfig), out ConfigReference reference))
      {
        reference!.ReferenceCount--;
        if (reference.ReferenceCount <= 0)
        {
          Resources.UnloadAsset(reference.Config);
          _loadedConfigs.Remove(typeof(TConfig));
        }
      }
    }

    public void Dispose()
    {
      UnloadConfig<ConfigProvider>();
    }
  }

  public class ConfigReference
  {
    public ScriptableObject Config;
    public int ReferenceCount;
  }
}
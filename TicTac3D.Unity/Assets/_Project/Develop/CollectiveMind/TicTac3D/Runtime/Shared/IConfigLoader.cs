using UnityEngine;

namespace CollectiveMind.TicTac3D.Runtime.Shared
{
  public interface IConfigLoader
  {
    TConfig LoadConfig<TConfig>() where TConfig : ScriptableObject;
    TConfig LoadConfig<TConfig>(string path) where TConfig : ScriptableObject;
    void UnloadConfig<TConfig>() where TConfig : ScriptableObject;
  }
}
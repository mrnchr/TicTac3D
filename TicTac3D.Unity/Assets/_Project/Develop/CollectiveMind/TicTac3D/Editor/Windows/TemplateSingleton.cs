using UnityEditor;
using UnityEngine;

namespace CollectiveMind.TicTac3D.Editor.Windows
{
  public class TemplateSingleton<TSingleton> : ScriptableSingleton<TSingleton>
    where TSingleton : TemplateSingleton<TSingleton>
  {
    private void OnEnable()
    {
      hideFlags = HideFlags.DontSave | HideFlags.HideInHierarchy | HideFlags.HideInInspector;
    }

    public void Save()
    {
      Save(false);
    }
  }
}
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace GitIntegration
{
  [InitializeOnLoad]
  public class SmartMergeRegistrar
  {
    private const string SMART_MERGE_REGISTRAR_EDITOR_PREFS_KEY = "smart_merge_installed";
    private const int VERSION = 1;
    private static readonly string _versionKey = $"{VERSION}_{Application.unityVersion}";

    static SmartMergeRegistrar()
    {
      string installedVersionKey = EditorPrefs.GetString(SMART_MERGE_REGISTRAR_EDITOR_PREFS_KEY);
      if (installedVersionKey != _versionKey)
        SmartMergeRegister();
    }

    [MenuItem("Tools/Git/SmartMerge registration")]
    private static void SmartMergeRegister()
    {
      try
      {
        string unityYamlMergePath = EditorApplication.applicationContentsPath + "/Tools" + "/UnityYAMLMerge.exe";
        ExecuteGitWithParams("config --local merge.unityyamlmerge.name \"Unity SmartMerge (UnityYamlMerge)\"");
        ExecuteGitWithParams(
          $"config --local merge.unityyamlmerge.driver \"\\\"{unityYamlMergePath}\\\" merge -h -p --force --fallback none %O %B %A %A\"");
        ExecuteGitWithParams("config --local merge.unityyamlmerge.recursive binary");
        EditorPrefs.SetString(SMART_MERGE_REGISTRAR_EDITOR_PREFS_KEY, _versionKey);
        Debug.Log($"Successfully registered UnityYAMLMerge with path {unityYamlMergePath}");
      }
      catch (Exception e)
      {
        Debug.Log($"Fail to register UnityYAMLMerge with error: {e}");
      }
    }

    [MenuItem("Tools/Git/SmartMerge unregistration")]
    private static void SmartMergeUnregister()
    {
      ExecuteGitWithParams("config --local --remove-section merge.unityyamlmerge");
      Debug.Log("Successfully unregistered UnityYAMLMerge");
    }

    private static void ExecuteGitWithParams(string param)
    {
      var processInfo = new System.Diagnostics.ProcessStartInfo("git")
      {
        UseShellExecute = false,
        WorkingDirectory = Environment.CurrentDirectory,
        RedirectStandardOutput = true,
        RedirectStandardError = true,
        CreateNoWindow = true
      };

      var process = new System.Diagnostics.Process();
      process.StartInfo = processInfo;
      process.StartInfo.FileName = "git";
      process.StartInfo.Arguments = param;
      process.Start();
      process.WaitForExit();

      if (process.ExitCode != 0)
        throw new Exception(process.StandardError.ReadLine());
    }
  }
}
#endif
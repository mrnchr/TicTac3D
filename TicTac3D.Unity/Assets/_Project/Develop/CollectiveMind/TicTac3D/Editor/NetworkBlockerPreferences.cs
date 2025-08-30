using System;
using System.Diagnostics;
using CollectiveMind.TicTac3D.Editor.Windows;
using CollectiveMind.TicTac3D.Runtime;
using Cysharp.Threading.Tasks;
using TriInspector;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace CollectiveMind.TicTac3D.Editor
{
  [FilePath(FPC.PROJECT_PATH + nameof(NetworkBlockerPreferences) + FPC.FILE_EXTENSION,
    FilePathAttribute.Location.ProjectFolder)]
  public class NetworkBlockerPreferences : TemplateSingleton<NetworkBlockerPreferences>
  {
    private const string NETWORK_BLOCKER_MENU = MIC.TOOLS_MENU + "Network Blocker/";

    [SerializeField]
    [HideInInspector]
    private bool _isBlocked;

    [Title("$" + nameof(BlockString))]
    [ShowInInspector]
    [DisplayAsString]
    [HideLabel]
    private string EmptyString
    {
      get => null;
      set { }
    }

    private string BlockString
    {
      get => _isBlocked ? "Network is blocked" : "Network is unblocked";
      set { }
    }

    [MenuItem(NETWORK_BLOCKER_MENU + "Block")]
    public static void BlockEditor()
    {
      instance.Block();
    }

    [MenuItem(NETWORK_BLOCKER_MENU + "Unblock")]
    [Button(ButtonSizes.Large)]
    public static void UnblockEditor()
    {
      instance.Unblock();
    }

    [Button(ButtonSizes.Medium)]
    private void Block()
    {
      BlockNetwork(() =>
      {
        _isBlocked = true;
        Debug.Log(BlockString);
      });
    }

    [Button(ButtonSizes.Medium)]
    private void Unblock()
    {
      UnblockAll(() =>
      {
        _isBlocked = false;
        Debug.Log(BlockString);
      });
    }

    [Button(ButtonSizes.Medium)]
    public void AddRule()
    {
      AddRuleForBlockNetwork(EditorApplication.applicationPath);
    }

    [Button(ButtonSizes.Medium)]
    public void RemoveRule()
    {
      RemoveRuleForBlockNetwork();
    }

    private static void AddRuleForBlockNetwork(string programPath, Action onExecuted = null)
    {
      programPath = programPath.Replace("/", "\\");

      string ps = $"$ErrorActionPreference='Stop'; "
        + $"New-NetFirewallRule -DisplayName 'UGS Block In' -Direction Inbound -Program \'{programPath}\' -Action Block -Enabled False;"
        + $"New-NetFirewallRule -DisplayName 'UGS Block Out' -Direction Outbound -Program \'{programPath}\' -Action Block -Enabled False;";

      RunElevatedPowerShell(ps, onExecuted);
    }

    private static void RemoveRuleForBlockNetwork(Action onExecuted = null)
    {
      var ps = "Get-NetFirewallRule -DisplayName 'UGS Block *' | Remove-NetFirewallRule;";
      RunElevatedPowerShell(ps, onExecuted);
    }

    private static void BlockNetwork(Action onExecuted = null)
    {
      string ps = $"$ErrorActionPreference='Stop'; "
        + $"Set-NetFirewallRule -DisplayName 'UGS Block *'  -Enabled True;";

      RunElevatedPowerShell(ps, onExecuted);
    }

    private static void UnblockAll(Action onExecuted = null)
    {
      var ps = "Set-NetFirewallRule -DisplayName 'UGS Block *' -Enabled False;";
      RunElevatedPowerShell(ps, onExecuted);
    }

    private static void RunElevatedPowerShell(string psCommand, Action onExecuted = null)
    {
      var psi = new ProcessStartInfo
      {
        FileName = "powershell",
        Arguments = $"-NoProfile -Command \"{psCommand}\"",
        Verb = "runas",
        UseShellExecute = true,
        CreateNoWindow = true
      };

      var proc = new Process();

      proc.StartInfo = psi;
      proc.Start();

      UniTask.WaitUntil(() =>
        {
          if (proc.HasExited)
          {
            proc.Dispose();
            return true;
          }

          return proc.HasExited;
        })
        .ContinueWith(onExecuted);
    }
  }
}
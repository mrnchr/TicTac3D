using CollectiveMind.TicTac3D.Editor.Windows;
using CollectiveMind.TicTac3D.Runtime;
using UnityEditor;

namespace CollectiveMind.TicTac3D.Editor
{
  public class NetworkBlockerWindow : TemplateWindow<NetworkBlockerPreferences>
  {
    [MenuItem(MIC.WINDOW_MENU + "Network Blocker")]
    public static void GetOrCreateWindow()
    {
      GetOrCreateTemplatedWindow<NetworkBlockerWindow>();
    }
    
    protected override NetworkBlockerPreferences Preferences => NetworkBlockerPreferences.instance;
  }
}
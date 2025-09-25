using UnityEngine;
#if UNITY_EDITOR
using UnityEditor.DeviceSimulation;
#endif

namespace CollectiveMind.TicTac3D.Runtime.Input
{
  public class OverridenPlatformSettings
  {
    public static readonly bool IsMobilePlatform =
#if UNITY_EDITOR
      CustomDeviceSimulatorPlugin.Instance?.IsAlive ??
#endif
      Application.isMobilePlatform;

#if UNITY_EDITOR
    private class CustomDeviceSimulatorPlugin : DeviceSimulatorPlugin
    {
      public static CustomDeviceSimulatorPlugin Instance { get; private set; }

      public bool IsAlive { get; private set; }

      public override string title => nameof(CustomDeviceSimulatorPlugin);

      public CustomDeviceSimulatorPlugin()
      {
        Instance = this;
      }

      public override void OnCreate()
      {
        IsAlive = true;
      }

      public override void OnDestroy()
      {
        IsAlive = false;
      }
    }
#endif
  }
}
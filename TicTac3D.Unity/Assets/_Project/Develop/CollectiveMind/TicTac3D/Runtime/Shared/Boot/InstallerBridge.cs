using System;
using System.Collections.Generic;
using Zenject;

namespace CollectiveMind.TicTac3D.Runtime.Shared.Boot
{
  public class InstallerBridge
  {
    private static readonly Dictionary<Type, Action<DiContainer>> _installerActions =
      new Dictionary<Type, Action<DiContainer>>();

    public static void Subscribe<TInstaller>(Action<DiContainer> onInstall)
      where TInstaller : IInstaller
    {
      if (!_installerActions.TryAdd(typeof(TInstaller), onInstall))
        _installerActions[typeof(TInstaller)] += onInstall;
    }

    public static void Unsubscribe<TInstaller>(Action<DiContainer> onInstall)
    {
      Type type = typeof(TInstaller);
      if (_installerActions.ContainsKey(type))
        _installerActions[type] -= onInstall;
    }

    public static void Install<TInstaller>(DiContainer container)
      where TInstaller : IInstaller
    {
      Install(typeof(TInstaller), container);
    }

    private static void Install(Type installerType, DiContainer container)
    {
      if (_installerActions.TryGetValue(installerType, out Action<DiContainer> onInstall))
        onInstall?.Invoke(container);
    }
  }
}
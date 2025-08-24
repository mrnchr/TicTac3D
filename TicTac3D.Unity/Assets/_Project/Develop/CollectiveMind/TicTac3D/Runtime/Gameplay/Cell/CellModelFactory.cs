using System;
using CollectiveMind.TicTac3D.Runtime.AssetManagement;
using UnityEngine;
using Zenject;

namespace CollectiveMind.TicTac3D.Runtime.Gameplay
{
  public class CellModelFactory : ICellModelFactory, IDisposable
  {
    private readonly IInstantiator _instantiator;
    private readonly IConfigLoader _configLoader;
    private readonly CellConfig _config;

    public CellModelFactory(IInstantiator instantiator, IConfigLoader configLoader)
    {
      _instantiator = instantiator;
      _configLoader = configLoader;

      _config = _configLoader.LoadConfig<CellConfig>();
    }

    public CellModel Create(Vector3 index)
    {
      var instance = _instantiator.Instantiate<CellModel>();
      instance.Index = index;
      instance.Position = index * _config.CellSize;
      return instance;
    }

    public void Dispose()
    {
      _configLoader.UnloadConfig<CellConfig>();
    }
  }
}
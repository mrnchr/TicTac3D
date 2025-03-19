using UnityEngine;
using Zenject;

namespace CollectiveMind.TicTac3D.Runtime.Shared.Gameplay.Cell
{
  public class CellModelFactory
  {
    private readonly IInstantiator _instantiator;

    public CellModelFactory(IInstantiator instantiator)
    {
      _instantiator = instantiator;
    }

    public CellModel Create(Vector3Int position)
    {
      var instance = _instantiator.Instantiate<CellModel>();
      instance.Position = position;
      return instance;
    }
  }
}
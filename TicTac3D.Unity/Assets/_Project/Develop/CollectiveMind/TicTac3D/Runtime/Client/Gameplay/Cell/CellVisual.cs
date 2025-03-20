using CollectiveMind.TicTac3D.Runtime.Shared.AssetManagement;
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay.Cell;
using UnityEngine;
using Zenject;

namespace CollectiveMind.TicTac3D.Runtime.Client.Gameplay.Cell
{
  public class CellVisual : MonoBehaviour
  {
    private CellModel _model;
    private IConfigLoader _configLoader;
    private CellConfig _config;

    [Inject]
    public void Construct(CellModel model, IConfigLoader configLoader)
    {
      _configLoader = configLoader;
      _model = model;
      _config = configLoader.LoadConfig<CellConfig>();
    }

    private void OnDestroy()
    {
      _configLoader.UnloadConfig<CellConfig>();
    }
  }
}
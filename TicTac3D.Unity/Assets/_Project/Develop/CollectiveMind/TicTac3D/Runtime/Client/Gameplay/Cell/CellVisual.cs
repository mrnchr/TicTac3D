using CollectiveMind.TicTac3D.Runtime.Shared.AssetManagement;
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay.Cell;
using UnityEngine;
using Zenject;
using R3;

namespace CollectiveMind.TicTac3D.Runtime.Client.Gameplay.Cell
{
  public class CellVisual : MonoBehaviour
  {
    private CellModel _model;
    private IConfigLoader _configLoader;
    private CellConfig _config;
    private Material _material;

    [Inject]
    public void Construct(CellModel model, IConfigLoader configLoader)
    {
      _configLoader = configLoader;
      _model = model;
      _config = configLoader.LoadConfig<CellConfig>();

      _material = GetComponent<Renderer>().material;
      _model.IsHovered.Subscribe(OnHoverChanged);
    }

    private void OnHoverChanged(bool isHovered)
    {
      _material.color = isHovered ? _config.HoveredColor : _config.DefaultColor;
    }

    private void OnDestroy()
    {
      _configLoader.UnloadConfig<CellConfig>();
    }
  }
}
using CollectiveMind.TicTac3D.Runtime.Client.Gameplay.Shape;
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
    private IShapeFactory _shapeFactory;
    private ShapeVisual _shape;

    [Inject]
    public void Construct(CellModel model, IConfigLoader configLoader, IShapeFactory shapeFactory)
    {
      _model = model;
      _configLoader = configLoader;
      _shapeFactory = shapeFactory;
      _config = configLoader.LoadConfig<CellConfig>();

      _material = GetComponent<Renderer>().material;
      _model.IsHovered.Subscribe(OnHoverChanged);
      _model.Shape.Subscribe(ChangeShape);
    }

    private void OnHoverChanged(bool isHovered)
    {
      _material.color = isHovered ? _config.HoveredColor : _config.DefaultColor;
    }

    private void ChangeShape(ShapeType id)
    {
      if (_shape)
        Destroy(_shape.gameObject);

      if (id != ShapeType.None)
        _shape = _shapeFactory.Create(id, transform.position, transform);
    }

    private void OnDestroy()
    {
      _configLoader.UnloadConfig<CellConfig>();
    }
  }
}
using CollectiveMind.TicTac3D.Runtime.Client.Gameplay.Shape;
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay.Cell;
using CollectiveMind.TicTac3D.Runtime.Shared.Gameplay.Shape;
using UnityEngine;
using Zenject;
using R3;

namespace CollectiveMind.TicTac3D.Runtime.Client.Gameplay.Cell
{
  public class CellVisual : MonoBehaviour
  {
    private CellModel _model;
    private IShapeFactory _shapeFactory;
    private ShapeVisual _shape;
    private Renderer _renderer;

    [Inject]
    public void Construct(CellModel model, IShapeFactory shapeFactory)
    {
      _model = model;
      _shapeFactory = shapeFactory;
      _renderer = GetComponent<MeshRenderer>();
    }

    private void Start()
    {
      _model.IsHovered.Subscribe(OnHoverChanged);
      _model.Shape.Subscribe(ChangeShape);
    }

    private void OnHoverChanged(bool isHovered)
    {
      _renderer.enabled = isHovered;
    }

    private void ChangeShape(ShapeType id)
    {
      if (_shape)
        Destroy(_shape.gameObject);

      if (id != ShapeType.None)
        _shape = _shapeFactory.Create(id, transform.position, transform);
    }
  }
}
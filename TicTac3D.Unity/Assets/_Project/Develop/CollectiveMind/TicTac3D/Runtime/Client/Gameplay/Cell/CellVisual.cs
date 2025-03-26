using CollectiveMind.TicTac3D.Runtime.Client.Gameplay.Shape;
using CollectiveMind.TicTac3D.Runtime.Client.SFX;
using CollectiveMind.TicTac3D.Runtime.Shared.AssetManagement;
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
    private IConfigLoader _configLoader;
    private ISoundAudioPlayer _soundAudioPlayer;
    private GameInfo _gameInfo;
    private SoundConfig _soundConfig;
    private Renderer _renderer;
    private ShapeVisual _shape;

    [Inject]
    public void Construct(CellModel model,
      IShapeFactory shapeFactory,
      IConfigLoader configLoader,
      ISoundAudioPlayer soundAudioPlayer,
      GameInfo gameInfo)
    {
      _model = model;
      _shapeFactory = shapeFactory;
      _configLoader = configLoader;
      _soundAudioPlayer = soundAudioPlayer;
      _gameInfo = gameInfo;
      _soundConfig = configLoader.LoadConfig<SoundConfig>();
      _renderer = GetComponent<MeshRenderer>();
      
      gameObject.name = $"Cell {model.Index}";
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
      {
        _shape = _shapeFactory.Create(id, transform.position, transform, _model);
        ShapeType soundShape = id switch
        {
          ShapeType.XO => ShapeType.XO,
          _ => id == _gameInfo.Shape ? ShapeType.X : ShapeType.O
        };
        _soundAudioPlayer.PlaySound(_soundConfig.GetShapeSound(soundShape));
      }
    }

    private void OnDestroy()
    {
      _configLoader.UnloadConfig<SoundConfig>();
    }
  }
}
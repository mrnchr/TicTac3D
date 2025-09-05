using CollectiveMind.TicTac3D.Runtime.Gameplay;
using R3;
using UnityEngine;
using Zenject;

namespace CollectiveMind.TicTac3D.Runtime.UI
{
  public class FadingCountHolder : MonoBehaviour
  {
    public ShapeFadingType ShapeFadingType;
    
    private ReactiveProperty<bool> IsActive { get; } = new ReactiveProperty<bool>();

    [Inject]
    public void Construct()
    {
      IsActive.Subscribe(ChangeVisibility).AddTo(this);
    }
    
    public void OnUpdateRule(GameRulesData rules)
    {
      IsActive.Value = (rules.ShapeFading & ShapeFadingType) > 0 || rules.ShapeFading == ShapeFadingType.None;
    }

    private void ChangeVisibility(bool isActive)
    {
      gameObject.SetActive(isActive);
    }
  }
}
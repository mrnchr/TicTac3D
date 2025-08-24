using System;
using R3;
using UnityEngine;
using Zenject;

namespace CollectiveMind.TicTac3D.Runtime.UI
{
  public class FadingCountHolder : MonoBehaviour
  {
    private FadingCountController _controller;
    private IDisposable _disposable;

    [Inject]
    public void Construct(FadingCountController controller)
    {
      _controller = controller;
      _disposable = _controller.IsActive.Subscribe(ChangeVisibility);
    }

    private void ChangeVisibility(bool isActive)
    {
      gameObject.SetActive(isActive);
    }

    private void OnDestroy()
    {
      _disposable?.Dispose();
    }
  }
}
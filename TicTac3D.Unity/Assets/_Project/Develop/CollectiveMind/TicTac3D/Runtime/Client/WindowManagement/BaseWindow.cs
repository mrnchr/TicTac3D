using Cysharp.Threading.Tasks;
using TriInspector;
using UnityEngine;

namespace CollectiveMind.TicTac3D.Runtime.Client.WindowManagement
{
  public class BaseWindow : MonoBehaviour
  {
    [ShowInInspector]
    [ReadOnly]
    [PropertyOrder(-1)]
    public bool IsOpen { get; private set; }

    [ShowInInspector]
    [ReadOnly]
    [PropertyOrder(-1)]
    public bool IsShown { get; private set; }

    public async UniTask Open()
    {
      IsOpen = true;
      ShowInternal();
      await OnOpened();
      await OnVisible();
    }

    public async UniTask Close()
    {
      IsOpen = false;
      HideInternal();
      await OnInvisible();
      await OnClosed();
    }

    public async UniTask Show()
    {
      ShowInternal();
      await OnShowed();
      await OnVisible();
    }

    public async UniTask Hide()
    {
      HideInternal();
      await OnInvisible();
      await OnHid();
    }

    private void ShowInternal()
    {
      IsShown = true;
      gameObject.SetActive(true);
      AddListeners();
    }

    private void HideInternal()
    {
      IsShown = false;
      gameObject.SetActive(false);
      RemoveListeners();
    }

    protected virtual async UniTask OnOpened()
    {
      await UniTask.CompletedTask;
    }

    protected virtual async UniTask OnClosed()
    {
      await UniTask.CompletedTask;
    }

    protected virtual async UniTask OnShowed()
    {
      await UniTask.CompletedTask;
    }

    protected virtual async UniTask OnHid()
    {
      await UniTask.CompletedTask;
    }

    protected virtual async UniTask OnVisible()
    {
      await UniTask.CompletedTask;
    }

    protected virtual async UniTask OnInvisible()
    {
      await UniTask.CompletedTask;
    }

    protected virtual void AddListeners()
    {
    }

    protected virtual void RemoveListeners()
    {
    }
  }
}
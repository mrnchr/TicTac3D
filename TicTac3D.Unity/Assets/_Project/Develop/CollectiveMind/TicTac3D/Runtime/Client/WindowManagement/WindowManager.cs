using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;

namespace CollectiveMind.TicTac3D.Runtime.Client.WindowManagement
{
  public class WindowManager : IWindowManager
  {
    private readonly List<BaseWindow> _windows = new List<BaseWindow>();
    private readonly Stack<BaseWindow> _history = new Stack<BaseWindow>();

    public void AddWindow(BaseWindow window)
    {
      _windows.Add(window);
    }

    public TWindow GetWindow<TWindow>() where TWindow : BaseWindow
    {
      return _windows.Find(x => x is TWindow) as TWindow;
    }

    public void RemoveWindow(BaseWindow window)
    {
      _windows.Remove(window);
    }

    public async UniTask<TWindow> OpenWindow<TWindow>() where TWindow : BaseWindow
    {
      if(_history.TryPeek(out BaseWindow lastWindow))
        await lastWindow.Hide();
      
      var window = GetWindow<TWindow>();
      if (window)
      {
        _history.Push(window);
        await window.Open();
      }

      return window;
    }

    public async UniTask<TWindow> CloseWindow<TWindow>() where TWindow : BaseWindow
    {
      if (_history.Peek() is not TWindow)
        return null;

      await CloseLastWindow();
      
      return await ShowLastWindow<TWindow>();
    }

    public async UniTask<TWindow> CloseWindowsBy<TWindow>() where TWindow : BaseWindow
    {
      if (!_history.Any(x => x is TWindow))
        return null;

      while (_history.Peek() is not TWindow)
      {
        await CloseLastWindow();
      }

      await CloseLastWindow();
      
      return await ShowLastWindow<TWindow>();
    }

    private async UniTask<TWindow> ShowLastWindow<TWindow>() where TWindow : BaseWindow
    {
      if (_history.TryPeek(out BaseWindow nextWindow))
        await nextWindow.Show();
      
      return nextWindow as TWindow;
    }

    private async UniTask CloseLastWindow()
    {
      await _history.Pop().Close();
    }
  }
}
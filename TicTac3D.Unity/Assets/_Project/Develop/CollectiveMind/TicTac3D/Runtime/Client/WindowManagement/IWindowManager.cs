using Cysharp.Threading.Tasks;

namespace CollectiveMind.TicTac3D.Runtime.Client.WindowManagement
{
  public interface IWindowManager
  {
    void AddWindow(BaseWindow window);
    TWindow GetWindow<TWindow>() where TWindow : BaseWindow;
    void RemoveWindow(BaseWindow window);
    UniTask<TWindow> OpenWindow<TWindow>() where TWindow : BaseWindow;
    UniTask<TWindow> CloseWindow<TWindow>() where TWindow : BaseWindow;
    UniTask<TWindow> CloseWindowsBy<TWindow>() where TWindow : BaseWindow;
  }
}
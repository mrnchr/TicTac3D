using System;
using Cysharp.Threading.Tasks;

namespace CollectiveMind.TicTac3D.Runtime.Client.UI.SetShape
{
  [Serializable]
  public class ConfirmationContext
  {
    private bool _isWaiting;
    private bool _answer;

    public event Action OnAsked;

    public async UniTask<bool> Ask()
    {
      if (!_isWaiting)
      {
        _isWaiting = true;
        OnAsked?.Invoke();
      }

      await UniTask.WaitWhile(() => _isWaiting);
      return _answer;
    }

    public void Answer(bool value)
    {
      _answer = value;
      _isWaiting = false;
    }
  }
}
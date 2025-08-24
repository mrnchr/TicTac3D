using System;
using CollectiveMind.TicTac3D.Runtime.GameStateComponents;
using R3;
using UnityEngine;
using Zenject;

namespace CollectiveMind.TicTac3D.Runtime.SFX
{
  public class MusicSwitcher : MonoBehaviour, IDisposable
  {
    [SerializeField]
    private AudioClip _menuMusic;
    
    [SerializeField]
    private AudioClip _gameplayMusic;
    
    private IGameStateMachine _gameStateMachine;
    private DisposableManager _disposableManager;
    private IDisposable _disposable;
    private AudioSource _audioSource;

    [Inject]
    public void Construct(IGameStateMachine gameStateMachine, DisposableManager disposableManager)
    {
      _gameStateMachine = gameStateMachine;
      _disposableManager = disposableManager;
      
      _audioSource = GetComponent<AudioSource>();

      _disposableManager.Add(this);
      _disposable = _gameStateMachine.CurrentState.Subscribe(ChangeMusic);
    }

    private void ChangeMusic(IExitableState gameState)
    {
      AudioClip clip = gameState switch
      {
        MenuGameState => _menuMusic,
        GameplayGameState => _gameplayMusic,
        _ => null
      };

      _audioSource.clip = clip;
      if(clip != null)
        _audioSource.Play();
    }

    public void Dispose()
    {
      _disposable?.Dispose();
    }
  }
}
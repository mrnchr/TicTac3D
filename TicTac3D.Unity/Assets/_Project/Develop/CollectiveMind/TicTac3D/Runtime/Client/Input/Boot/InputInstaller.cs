using UnityEngine.InputSystem;
using Zenject;

namespace CollectiveMind.TicTac3D.Runtime.Client.Input
{
  public class InputInstaller : Installer<InputInstaller>
  {
    public override void InstallBindings()
    {
      BindInputProvider();
      BindPlayerInputActions();
      BindPlayerInput();
      BindInputHandler();
    }

    private void BindInputProvider()
    {
      Container
        .Bind<InputProvider>()
        .AsSingle();
    }

    private void BindPlayerInputActions()
    {
      Container
        .BindInterfacesAndSelfTo<PlayerInputActions>()
        .AsSingle();
    }

    private void BindPlayerInput()
    {
      Container
        .Bind<PlayerInput>()
        .FromNewComponentOnNewGameObject()
        .WithGameObjectName(nameof(PlayerInput))
        .AsSingle();
    }

    private void BindInputHandler()
    {
      Container
        .BindInterfacesTo<InputHandler>()
        .AsSingle();
    }
  }
}
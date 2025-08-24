using System.Linq;
using UnityEngine;
using Zenject;

namespace CollectiveMind.TicTac3D.Runtime.Boot
{
  [DisallowMultipleComponent]
  public class MonoInjector : MonoBehaviour
  {
#if UNITY_EDITOR
    private bool _injected;

    [Inject]
    public void Construct()
    {
      _injected = true;
    }

    private void Awake()
    {
      if (!_injected)
      {
        Context ctx = GetComponentInParent<GameObjectContext>(true);
        if (!transform.parent)
        {
          Inject(ctx);
          return;
        }

        var parentInjector = transform.parent.GetComponentInParent<MonoInjector>(true);

        switch ((bool)ctx)
        {
          case false when parentInjector._injected:
          case true when parentInjector.transform.IsChildOf(ctx.transform) && parentInjector._injected:
          case true when ctx.transform.IsChildOf(parentInjector.transform):
          case true when ctx.gameObject == parentInjector.gameObject && parentInjector._injected:
            Inject(ctx);
            break;
        }
      }
    }

    private void Inject(Context ctx)
    {
      if (!ctx)
      {
        ctx = FindObjectsByType<Context>(FindObjectsInactive.Include, FindObjectsSortMode.None)
          .First(x => x.gameObject.scene == gameObject.scene && x is ProjectContext or SceneContext);
      }

      ctx.Container.InjectGameObject(gameObject);
    }
#endif
  }
}
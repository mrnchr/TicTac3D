using TriInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;

namespace CollectiveMind.TicTac3D.Runtime.Input
{
  public class OnScreenHoldButton : OnScreenControl, IPointerUpHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
  {
    [ShowInInspector]
    [ReadOnly]
    private bool _isPointerDown;
    
    [InputControl(layout = "Button")]
    [SerializeField]
    private string _controlPath;

    protected override string controlPathInternal
    {
      get => _controlPath;
      set => _controlPath = value;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
    }

    public void OnPointerExit(PointerEventData eventData)
    {
      OnPointerUp();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
      OnPointerDown();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
      OnPointerUp();
    }

    private void Update()
    {
      if(_isPointerDown)
        SendValueToControl(1.0f);
    }

    private void OnPointerDown()
    {
      _isPointerDown = true;
      SendValueToControl(1.0f);
    }

    private void OnPointerUp()
    {
      _isPointerDown = false;
      SendValueToControl(0.0f);
    }

#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(OnScreenHoldButton))]
    internal class OnScreenHoldButtonEditor : UnityEditor.Editor
    {
      private UnityEditor.SerializedProperty _controlPathInternal;

      public void OnEnable()
      {
        _controlPathInternal = serializedObject.FindProperty(nameof(_controlPath));
      }

      public override void OnInspectorGUI()
      {
        // Current implementation has UGUI dependencies (ISXB-915, ISXB-916)
        var instance = (OnScreenHoldButton)target;
        Transform parentTransform = instance.transform.parent;
        if (!parentTransform || !parentTransform.GetComponentInParent<RectTransform>())
          UnityEditor.EditorGUILayout.HelpBox(
            $"{instance.GetType()} needs to be attached as a child to a UI Canvas and have a RectTransform component to function properly.",
            UnityEditor.MessageType.Warning);

        UnityEditor.EditorGUILayout.PropertyField(_controlPathInternal);

        serializedObject.ApplyModifiedProperties();
      }
    }
#endif
  }
}
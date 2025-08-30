using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace CollectiveMind.TicTac3D.Editor.Windows
{
  public class TemplateWindow<TPreferences> : EditorWindow
    where TPreferences : TemplateSingleton<TPreferences>
  {
    private UnityEditor.Editor _editor;

    protected virtual TPreferences Preferences => throw new NotImplementedException();

    private void Update()
    {
      if (EditorUtility.IsDirty(Preferences))
        Preferences.Save();
    }

    private void OnEnable()
    {
      Subscribe();
      RecreateGUI();
    }

    private void OnDisable()
    {
      DisposeInternalData();
      Unsubscribe();
    }

    private void CreateGUI()
    {
      rootVisualElement.Clear();
      rootVisualElement.hierarchy.Add(new ScrollView(ScrollViewMode.Vertical));
      var scrollView = rootVisualElement.Q<ScrollView>();
      scrollView.Add(new IMGUIContainer(() => _editor.DrawHeader()));
      scrollView.Add(new InspectorElement(_editor));
    }

    protected static void GetOrCreateTemplatedWindow<TWindow>() where TWindow : TemplateWindow<TPreferences>
    {
      var window = GetWindow<TWindow>();
      window.SetName();
      window.Show();
    }

    protected virtual void SetName()
    {
      titleContent = new GUIContent(ObjectNames.NicifyVariableName(GetType().Name.Replace("Window", "")));
    }

    private void Subscribe()
    {
      SceneManager.activeSceneChanged += OnActiveSceneChanged;
      EditorSceneManager.activeSceneChangedInEditMode += OnActiveSceneChanged;
    }

    private void OnActiveSceneChanged(Scene oldScene, Scene newScene)
    {
      RecreateGUI();
    }

    private void Unsubscribe()
    {
      SceneManager.activeSceneChanged -= OnActiveSceneChanged;
      EditorSceneManager.activeSceneChangedInEditMode -= OnActiveSceneChanged;
    }

    private void RecreateGUI()
    {
      if (_editor)
        DestroyImmediate(_editor);

      _editor = UnityEditor.Editor.CreateEditor(Preferences);
      CreateGUI();
    }

    private void DisposeInternalData()
    {
      if (_editor)
        DestroyImmediate(_editor);
    }
  }
}
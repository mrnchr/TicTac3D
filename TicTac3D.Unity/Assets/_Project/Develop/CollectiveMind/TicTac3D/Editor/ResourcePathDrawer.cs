using System;
using System.IO;
using CollectiveMind.TicTac3D.Editor;
using CollectiveMind.TicTac3D.Runtime.Shared.Utils;
using TriInspector;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

[assembly: RegisterTriAttributeDrawer(typeof(ResourcePathDrawer), TriDrawerOrder.Decorator, ApplyOnArrayElement = true)]

namespace CollectiveMind.TicTac3D.Editor
{
  public class ResourcePathDrawer : TriAttributeDrawer<ResourcePathAttribute>
  {
    public override TriExtensionInitializationResult Initialize(TriPropertyDefinition propertyDefinition)
    {
      var type = propertyDefinition.FieldType;
      if (type != typeof(string))
      {
        return "File path attribute can only be used on field with string type";
      }

      return base.Initialize(propertyDefinition);
    }

    public override TriElement CreateElement(TriProperty property, TriElement next)
    {
      return new ResourcePathElement(property, Attribute);
    }

    private class ResourcePathElement : TriElement
    {
      private const string RESOURCES_FOLDER = "Resources/";

      private readonly TriProperty _property;
      private readonly ResourcePathAttribute _attribute;

      private Object _asset;

      public ResourcePathElement(TriProperty property, ResourcePathAttribute attribute)
      {
        _property = property;
        _attribute = attribute;
      }

      protected override void OnAttachToPanel()
      {
        base.OnAttachToPanel();

        _property.ValueChanged += OnValueChanged;

        RefreshAsset();
      }

      protected override void OnDetachFromPanel()
      {
        _property.ValueChanged -= OnValueChanged;

        base.OnDetachFromPanel();
      }

      public override float GetHeight(float width)
      {
        return EditorGUIUtility.singleLineHeight;
      }

      public override bool Update()
      {
        bool isDirty = false;
        if (ConvertPathToResourcePath(AssetDatabase.GetAssetPath(_asset)) != _property.Value as string)
        {
          isDirty = true;
          RefreshAsset();
        }
        
        return isDirty | base.Update();
      }

      public override void OnGUI(Rect position)
      {
        EditorGUI.BeginChangeCheck();

        Object asset = EditorGUI.ObjectField(position, _property.DisplayName, _asset, _attribute.ResourceType, false);

        if (EditorGUI.EndChangeCheck())
        {
          string path = AssetDatabase.GetAssetPath(asset);
          _property.SetValue(ConvertPathToResourcePath(path));
        }
      }

      private string ConvertPathToResourcePath(string filePath)
      {
        string withoutExtension = Path.ChangeExtension(filePath, null);

        int resourcesIndex = withoutExtension.IndexOf(RESOURCES_FOLDER, StringComparison.OrdinalIgnoreCase);
        return resourcesIndex == -1 ? withoutExtension : withoutExtension[(resourcesIndex + RESOURCES_FOLDER.Length)..];
      }

      private void OnValueChanged(TriProperty property)
      {
        RefreshAsset();
      }

      private void RefreshAsset()
      {
        _asset = Resources.Load<Object>(_property.Value as string);
      }
    }
  }
}
using System;
using System.Collections;
using System.Linq;
using CollectiveMind.TicTac3D.Runtime.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CollectiveMind.TicTac3D.Runtime.UI
{
  [RequireComponent(typeof(CustomDropdownData))]
  public class CustomDropdown : TMP_Dropdown
  {
    private RectTransform _dropdownList;
    private RectTransform _item;

    private CustomDropdownData _dropdownData;

    private float _height;
    private bool _wasForceRebuild;
    private Canvas MenuCanvas => DropdownData.MenuCanvas;
    private CanvasGroup MenuCanvasGroup => DropdownData.MenuMenuCanvasGroup;

    public event Action<RectTransform> OnDropdownShowed;

    private CustomDropdownData DropdownData
    {
      get
      {
        if (!_dropdownData)
          TryGetComponent(out _dropdownData);

        return _dropdownData;
      }
    }

    public Toggle GetDropdownItemToggle(string optionName)
    {
      return _dropdownList.OrNull()?.GetComponentsInChildren<DropdownItem>()
        .Where(x => x.text.text == optionName)
        .Select(x => x.toggle)
        .FirstOrDefault();
    }

    protected override GameObject CreateDropdownList(GameObject template)
    {
      if (!_wasForceRebuild)
      {
        _wasForceRebuild = true;
        template.gameObject.SetActive(true);
        var group = template.GetComponentInChildren<DropdownItem>().transform.parent
          .GetComponent<VerticalLayoutGroup>();
        LayoutRebuilder.ForceRebuildLayoutImmediate(group.transform as RectTransform);
        template.gameObject.SetActive(false);
      }

      GameObject list = base.CreateDropdownList(template);
      if (list)
      {
        _dropdownList = list.transform as RectTransform;
      }

      if (MenuCanvas)
        MenuCanvas.overrideSorting = true;

      if (MenuCanvasGroup)
        MenuCanvasGroup.blocksRaycasts = false;
      
      StartCoroutine(AdjustDropdownList());

      return list;
    }

    private IEnumerator AdjustDropdownList()
    {
      yield return null;
      if (_dropdownList)
      {
        var item = _dropdownList.GetComponentInChildren<DropdownItem>().transform as RectTransform;
        var content = item.parent as RectTransform;
        var group = content.GetComponent<VerticalLayoutGroup>();
        if (transform.position.y - ((RectTransform)transform).rect.yMin < content.position.y)
        {
          group.padding.bottom = group.padding.top;
          group.padding.top = 0;
        }
        
        LayoutRebuilder.ForceRebuildLayoutImmediate(content);
        OnDropdownShowed?.Invoke(_dropdownList);
      }
    }

    protected override void DestroyDropdownList(GameObject dropdownList)
    {
      if (MenuCanvas)
        MenuCanvas.overrideSorting = false;

      if (MenuCanvasGroup)
        MenuCanvasGroup.blocksRaycasts = true;

      base.DestroyDropdownList(dropdownList);
    }
  }
}
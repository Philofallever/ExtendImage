using System.Collections;
using System.Collections.Generic;
using ExtendUI.SuperScrollView;
using UnityEngine;
using UnityEngine.UI;

public class Item1Data
{
    public Color HeadColor;
    public string Desc;
}

public class Item1Script : MonoBehaviour
{
    public Image Head;
    public Text Desc;
    public InputField InputField;
    public Button SetButton;

    private LoopListViewItem2 _listItem;
    public void Init()
    {
        _listItem = GetComponent<LoopListViewItem2>();
        SetButton.onClick.AddListener(SetSize);
    }

    public void SetSize()
    {
        if (!double.TryParse(InputField.text, out var height))
            return;

        var rect = (RectTransform)transform;
        rect.sizeDelta += new Vector2(0, (float)height);
        _listItem.ParentListView.OnItemSizeChanged(_listItem.ItemIndex);
    }

    public void SetData(Item1Data data)
    {
        Head.color = data.HeadColor;
        Desc.text = data.Desc;
    }
}

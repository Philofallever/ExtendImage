using System.Collections;
using System.Collections.Generic;
using ExtendUI;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

public class Test : MonoBehaviour,ISelectHandler,IDeselectHandler
{

    [RuntimeInitializeOnLoadMethod]
    static void Initialize()
    {
        var obj = new GameObject();
        obj.AddComponent<Test>();
        DontDestroyOnLoad(obj);
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 100, 50), "切换语言", GUI.skin.button))
        {
        }

    }

    [Button]
    void SetId(int colorId)
    {
        //GetComponentInChildren<ExtendText>().colorId = colorId;
    }

    [Button]
    void SetHeight(bool value)
    {
        //selectb
        //GetComponentInChildren<ExtendText>().highlight = value;
    }

    public void OnSelect(BaseEventData eventData)
    {
        print("onselsect");
    }

    public void OnDeselect(BaseEventData eventData)
    {
        print("ondesct");
    }
}

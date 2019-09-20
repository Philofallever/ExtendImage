using System.Collections;
using System.Collections.Generic;
using ExtendUI;
using Sirenix.OdinInspector;
using UnityEngine;

public class Test : MonoBehaviour
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
        GetComponentInChildren<ExtendText>().colorId = colorId;
    }

    [Button]
    void SetHeight(bool value)
    {
        GetComponentInChildren<ExtendText>().highlight = value;
    }
}

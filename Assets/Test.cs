using System.Collections;
using System.Collections.Generic;
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
}

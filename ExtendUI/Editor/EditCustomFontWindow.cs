using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class EditCustomFontWindow : EditorWindow
{
    private Font       font;
    private Texture2D  tex;
    private string     fontPath;
    private int        num = 10;
    private Vector2Int size;
    private string     fntFilePath;

    private void OnGUI()
    {
        GUILayout.BeginVertical();

        GUILayout.BeginHorizontal();
        GUILayout.Label("字体：");
        font = (Font) EditorGUILayout.ObjectField(font, typeof(Font), true);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("字体图片：");
        tex = (Texture2D) EditorGUILayout.ObjectField(tex, typeof(Texture2D), true);
        if (tex)
        {
            size.x = tex.width / num;
            size.y = tex.height;
        }

        GUILayout.EndHorizontal();
        num = EditorGUILayout.IntField("数目", num);

        GUILayout.BeginHorizontal();
        size = EditorGUILayout.Vector2IntField("size", size);

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("更新文本")) UpdateFont();

        GUILayout.EndHorizontal();

        GUILayout.EndVertical();
    }

    private void UpdateFont()
    {
        var path    = AssetDatabase.GetAssetPath(font);
        var dir     = Path.GetDirectoryName(path);
        var matPath = Path.Combine(dir,Path.GetFileNameWithoutExtension(path) +".mat");

        var mat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
        if (!mat)
        {
            mat = new Material(Shader.Find("GUI/Text Shader"));
            AssetDatabase.CreateAsset(mat, matPath);
        }

        mat.SetTexture("_MainTex", tex);
        EditorUtility.SetDirty(mat);

        var list      = new List<CharacterInfo>();
        var eachWidth = (float) tex.width / num;
        for (var i = 0; i < num; ++i)
        {
            var charInfo = new CharacterInfo();
            charInfo.index     = i;
            charInfo.uv.x      = eachWidth * i / tex.width;
            charInfo.uv.y      = 0;
            charInfo.uv.width  = eachWidth / tex.width;
            charInfo.uv.height = 1;

            charInfo.vert.x      = 0;
            charInfo.vert.y      = tex.height/2;
            charInfo.vert.width  = eachWidth;
            charInfo.vert.height = -tex.height;
            charInfo.advance = 1;
            list.Add(charInfo);
        }

        font.material      = mat;
        font.characterInfo = list.ToArray();
        EditorUtility.SetDirty(font);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("更新成功");
    }
}
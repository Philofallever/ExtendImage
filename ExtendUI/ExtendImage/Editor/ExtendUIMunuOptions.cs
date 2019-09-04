using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using WXB;
using Object = UnityEngine.Object;

namespace UnityEditor.UI
{
    internal static class ExtendUIMunuOptions
    {
        private static Type _menuOptions;

        private static Type MenuOptions
        {
            get
            {
                if (_menuOptions == null)
                {
                    var assembly = Assembly.Load("UnityEditor.UI");
                    _menuOptions = assembly.GetType("UnityEditor.UI.MenuOptions", true, true);
                }

                return _menuOptions;
            }
        }

        [MenuItem("GameObject/UI/ExtendImage", false, 2003)]
        private static void AddExtendImage(MenuCommand menuCommand)
        {
            var addImage = MenuOptions?.GetMethod("AddImage", BindingFlags.Public | BindingFlags.Static);
            if (addImage != null)
            {
                addImage.Invoke(null, new object[] {menuCommand});
                // 创建后会选中
                var obj = Selection.activeGameObject;
                obj.name = nameof(ExtendImage);
                Object.DestroyImmediate(obj.GetComponent<Image>());
                var extendImg = obj.AddComponent<ExtendImage>();
                extendImg.raycastTarget = false;
            }
        }

        [MenuItem("GameObject/UI/SymbolText",false,2001)]
        private static void AddSymbolText(MenuCommand menuCommand)
        {
            var addText = MenuOptions?.GetMethod("AddText", BindingFlags.Public | BindingFlags.Static);
            if (addText != null)
            {
                addText.Invoke(null, new object[] {menuCommand});
                var obj = Selection.activeGameObject;
                obj.name = nameof(SymbolText);
                Object.DestroyImmediate(obj.GetComponent<Text>());
                var symbolText = obj.AddComponent<SymbolText>();
                symbolText.fontSize = 36;
                symbolText.supportRichText = false;
            }
        }

        [MenuItem("ExtendUI/LanguageCfg",false)]
        private static void LoadLangugageCfg()
        {
            var type = typeof(LanguageCfgs);
            var path = $"Assets/ExtendText/Resources/{type.Name}.asset";
            if (!File.Exists(path))
            {
                var obj = ScriptableObject.CreateInstance<LanguageCfgs>();
                AssetDatabase.CreateAsset(obj,path);
                AssetDatabase.Refresh();
            }

            var asset = AssetDatabase.LoadAssetAtPath<LanguageCfgs>(path);
            AssetDatabase.OpenAsset(asset);
        }

        [MenuItem("ExtendUI/SymbolTextCfg", false)]
        private static void LoadSymbolTextCfg()
        {
            var type = typeof(SymbolTextCfg);
            var path = $"Assets/ExtendText/Resources/{type.Name}.asset";
            if (!File.Exists(path))
            {
                var obj = ScriptableObject.CreateInstance<SymbolTextCfg>();
                AssetDatabase.CreateAsset(obj, path);
                AssetDatabase.Refresh();
            }

            var asset = AssetDatabase.LoadAssetAtPath<SymbolTextCfg>(path);
            AssetDatabase.OpenAsset(asset);
        }

    }
}
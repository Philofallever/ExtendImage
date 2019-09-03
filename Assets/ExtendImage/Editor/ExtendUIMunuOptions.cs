using System;
using System.Reflection;
using UnityEngine.UI;
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
    }
}
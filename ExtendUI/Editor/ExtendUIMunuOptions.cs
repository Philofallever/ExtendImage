using System;
using System.IO;
using System.Reflection;
using ExtendUI;
using ExtendUI.SymbolText;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace UnityEditor.UI
{
    internal static class ExtendUIMunuOptions
    {
        private static          Type   _menuOptions;
        private static readonly string _packageRoot = @"Packages/com.blz.extend-ui";

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

        [MenuItem("GameObject/UI/ExtendText", false, 2001)]
        private static void AddExtendText(MenuCommand menuCommand)
        {
            var addText = MenuOptions?.GetMethod("AddText", BindingFlags.Public | BindingFlags.Static);
            if (addText != null)
            {
                addText.Invoke(null, new object[] {menuCommand});
                var obj = Selection.activeGameObject;
                obj.name = nameof(ExtendText);
                Object.DestroyImmediate(obj.GetComponent<Text>());
                var extendText = obj.AddComponent<ExtendText>();
                extendText.fontSize        = 36;
                extendText.supportRichText = false;
                extendText.raycastTarget   = false;
            }
        }

        [MenuItem("GameObject/UI/SymbolText", false, 2001)]
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
                symbolText.fontSize        = 36;
                symbolText.supportRichText = false;
            }
        }

        private static void Load<T>() where T : ScriptableObject
        {
            var type = typeof(T);
            var path = $"{_packageRoot}/Resources/{type.Name}.asset";
            if (!File.Exists(path))
            {
                var obj = ScriptableObject.CreateInstance<T>();
                AssetDatabase.CreateAsset(obj, path);
                AssetDatabase.Refresh();
            }

            var asset = AssetDatabase.LoadAssetAtPath<T>(path);
            AssetDatabase.OpenAsset(asset);
        }

        [MenuItem("ExtendUI/富文本表情配置", false)]
        private static void LoadSymbolTextCfg()
        {
            Load<SymbolTextCfg>();
        }

        [MenuItem("ExtendUI/数字字体",false)]
        private static void DoIt()
        {
            EditorWindow.GetWindow<EditCustomFontWindow>("创建字体");
        }

        /* 富文本表情
        [MenuItem("ExtendUI/CheckFileName",false)]
        static void CheckFileName()
        {
            string assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
            HashSet<string> guilds = new HashSet<string>(AssetDatabase.FindAssets("", new string[] { assetPath }));
            foreach (string guid in guilds)
            {
                string ap = AssetDatabase.GUIDToAssetPath(guid);
                //if (ap.EndsWith("_.png"))
                //{
                //    Debug.LogFormat(ap);
                //}

                if (string.IsNullOrEmpty(ap) || !ap.EndsWith(".png", true, null))
                    continue;

                int pos = ap.LastIndexOf('/');
                if (!ap.Substring(pos + 1).StartsWith("anim_"))
                    continue;

                int pos_ = ap.LastIndexOf('_');
                if (pos_ == -1)
                    continue;

                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append(ap.Substring(0, pos_ + 1));
                int m = pos_ + 1;
                int e = ap.LastIndexOf('.') - 1;
                for (; m < e; ++m)
                {
                    if (ap[m] != '0')
                        break;
                }

                sb.Append(ap.Substring(m, ap.Length - m));
                //Debug.LogFormat("{0}->{1}", ap.Substring(7), sb.ToString().Substring(7));
                AssetDatabase.CopyAsset(ap, sb.ToString());
                AssetDatabase.DeleteAsset(ap);
            }
        }


        [MenuItem("Assets/更新动画数据",false)]
        static void UpdateAnim()
        {
            string assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);

            HashSet<string> guids = new HashSet<string>(AssetDatabase.FindAssets("", new string[] { assetPath }));

            Dictionary<string, Sprite> sprites = new Dictionary<string, Sprite>();
            foreach (string guid in guids)
            {
                Object[] objs = AssetDatabase.LoadAllAssetRepresentationsAtPath(AssetDatabase.GUIDToAssetPath(guid));
                foreach (Object o in objs)
                {
                    if (o is Sprite)
                        sprites.Add(o.name, o as Sprite);
                }
            }

            Dictionary<string, Cartoon> Cartoons = new Dictionary<string, Cartoon>();
            List<Sprite> tempss = new List<Sprite>();
            for (int i = 0; i < 1000; ++i)
            {
                string animName = string.Format("anim_{0}_", i);
                for (int j = 0; j < 100; ++j)
                {
                    string frameName = animName + j;
                    Sprite s = null;
                    if (sprites.TryGetValue(frameName, out s))
                        tempss.Add(s);
                 }

                if (tempss.Count != 0)
                {
                    Cartoon c = new Cartoon();
                    c.name = i.ToString();
                    c.fps = 5f;
                    c.sprites = tempss.ToArray();
                    c.width = (int)c.sprites[0].rect.width;
                    c.height = (int)c.sprites[0].rect.height;

                    Cartoons.Add(i.ToString(), c);

                    tempss.Clear();
                }
            }

            SymbolTextInit sti = Resources.Load<SymbolTextInit>("SymbolTextInit");
            FieldInfo info = typeof(SymbolTextInit).GetField("cartoons", BindingFlags.Instance | BindingFlags.NonPublic);
            List<Cartoon> cartoons = new List<Cartoon>(Cartoons.Values);
            cartoons.Sort((Cartoon x, Cartoon y) => 
            {
                return int.Parse(x.name).CompareTo(int.Parse(y.name));
            });

            info.SetValue(sti, cartoons.ToArray());
            EditorUtility.SetDirty(sti);
        }
         */
    }
}
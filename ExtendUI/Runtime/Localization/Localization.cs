using System;
using System.Collections.Generic;
using UnityEngine;

#if !UNITY_EDITOR && EXTEND_UI_DEBUG
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Assembly-CSharp")]
#endif

namespace ExtendUI
{
    // 多语言对应图片和文本两部分,对于TEXT组件来说,只需要替换字体即可,对于Image组件需要替换精灵
    public interface ILocalizable
    {
        void Localize();
    }

    /// <summary>UI组件本地化类</summary>
    // 需要语言系统和资源系统支持
    public static class Localization
    {
        private static          Dictionary<SystemLanguage, Font>     _localizeFonts;
        private static          Func<SystemLanguage, string, Sprite> _localSpriteFunc;
        internal static         SystemLanguage                       CurrLang { get; private set; }
        internal static         Font                                 CurrFont { get; private set; }
        private static readonly HashSet<ILocalizable>                localizables;

        static Localization()
        {
            _localizeFonts = new Dictionary<SystemLanguage, Font>();
            localizables   = new HashSet<ILocalizable>();
        }

        public static void AddLocalFont(SystemLanguage lang, Font font)
        {
            if (font)
                _localizeFonts[lang] = font;
        }

        // 语言系统，切换语言后调用此API
        public static void OnLanguageChange(SystemLanguage lang)
        {
            _localizeFonts.TryGetValue(lang, out var font);

            if (font != null)
            {
                CurrLang = lang;
                CurrFont = font;
                // Text组件无论如何都是需要本地化的，替换字体，而Image大多时候是不需要的
                foreach (var localizable in localizables)
                    localizable.Localize();
            }
#if UNITY_EDITOR
            else
                Debug.LogError($"系统语言{lang}所对应的字体不存在,请使用`AddLocalFont`添加");
#endif
        }

        internal static void Register(ILocalizable localizable)
        {
            localizables.Add(localizable);
        }

        internal static void UnRegister(ILocalizable localizable)
        {
            localizables.Remove(localizable);
        }

        internal static Sprite LocalizeSprite(Sprite originSprite)
        {
            // TODO 最好是初始化时绑定一个API   Func<SystemLanguage,Sprite,Sprite>
            return originSprite;
        }
    }
}
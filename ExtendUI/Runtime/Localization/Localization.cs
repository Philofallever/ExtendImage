using System.Collections.Generic;
using ExtendUI;
using ExtendUI.SymbolText;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

// 测试多语言对应图片和文本两部分,对于TEXT组件来说,只需要替换字体即可,对于Image组件需要替换精灵
public interface ILocalizable
{
}

public interface ILocalizableText : ILocalizable
{
    void Localize(Font localFont);
}

// TODO 
public interface ILocalizableImage : ILocalizable
{
    void Localize(Sprite localSprite);
}

public static class Localization
{
    public static  LanguageCfgs.LanguageCfg CurrLangCfg;
    private static LanguageCfgs.LanguageCfg _defaultLangCfg;
    private static LanguageCfgs             _languageCfgs;
    private static HashSet<ILocalizable>    localizables;

#if UNITY_EDITOR
    [InitializeOnLoadMethod]
#endif
    public static void Initialize( /*SystemLanguage defaultLang*/)
    {
        if (_languageCfgs != null)
            return;

        localizables    = new HashSet<ILocalizable>();
        _languageCfgs   = Resources.Load<LanguageCfgs>(nameof(LanguageCfgs));
        _defaultLangCfg = _languageCfgs[1];
        CurrLangCfg     = _defaultLangCfg;
    }

    public static void ChangeLanguage(SystemLanguage language)
    {
        var target = _languageCfgs[language];
        if (target == null) return;

        CurrLangCfg = target;
        foreach (var localizable in localizables)
        {
            switch (localizable)
            {
                case ExtendText extendText:
                    extendText.Localize(CurrLangCfg.Font);
                    break;
                case SymbolText symbolText:
                    symbolText.Localize(CurrLangCfg.Font);
                    break;
                case ExtendImage extendImage:
                    var resPath = $"{CurrLangCfg.RootPath}/{extendImage.sprite.name}";
                    var sprite  = Resources.Load<Sprite>(resPath);
                    // extendImage.Localize(sprite)
                    break;
            }
        }
    }

    public static void Register(ILocalizable localizable)
    {
        localizables.Add(localizable);
    }

    public static void Ungister(ILocalizable localizable)
    {
        localizables.Remove(localizable);
    }
}
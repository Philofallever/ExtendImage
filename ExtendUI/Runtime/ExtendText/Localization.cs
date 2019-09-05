using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public interface ILocalizable
{
    void Localize();
}

// 多语言对应图片和文本两部分
public static class Localization
{
    public static LanguageCfgs LanguageCfgs;
    public static LanguageCfgs.LanguageCfg CurrLangCfg;

    public static List<ILocalizable> Ilocalizables;

    public static void Initialize()
    {
        LanguageCfgs = AssetBundle.LoadFromFile("Asset/AssetBundles/LanguageCfgs").LoadAsset<LanguageCfgs>(nameof(LanguageCfgs));
    }

    public static void ChangeLanguage(SystemLanguage language)
    {
        var target = LanguageCfgs.Languages.FirstOrDefault(lang => lang.LangId == language);
        if (target == null) return;

        CurrLangCfg = target;
    }
}

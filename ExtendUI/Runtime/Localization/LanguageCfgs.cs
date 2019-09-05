using System;
using UnityEngine;

public class LanguageCfgs : ScriptableObject
{
    [Serializable]
    public class LanguageCfg
    {
        public int            Id;
        public SystemLanguage Lang;
        public string         Name;
        public Font           Font;
        public string         RootPath;
    }

    [SerializeField]
    private LanguageCfg[] _languages;

    public LanguageCfg this[int id]
    {
        get
        {
            var cfg = Array.Find(_languages, langCfg => langCfg.Id == id);
#if UNITY_EDITOR && EXTEND_UI_DEBUG
            if (cfg == null)
                Debug.LogError($"配置Id为{id}的语言配置不存在", this);
#endif
            return cfg;
        }
    }

    public LanguageCfg this[SystemLanguage lang]
    {
        get
        {
            var cfg = Array.Find(_languages, langCfg => langCfg.Lang == lang);
#if UNITY_EDITOR && EXTEND_UI_DEBUG
            if (cfg == null)
                Debug.LogError($"系统语言为{lang}的语言配置不存在", this);
#endif
            return cfg;
        }
    }

    // TODO:更新数据?? 
    public LanguageCfgs UpdateData()
    { 
        return this;
    }
}
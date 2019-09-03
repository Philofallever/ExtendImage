using Sirenix.OdinInspector;
using UnityEngine;

public class LanguageCfgs : SerializedScriptableObject
{
    public class LanguageCfg
    {
        public int            Id;
        public SystemLanguage LangId;
        public string         Name;
        public string         RootPath;
        public Font           Font;
    }

    public LanguageCfg[] Languages;
}
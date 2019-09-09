using UnityEngine;
using UnityEngine.UI;

namespace ExtendUI
{
    /// <summary>
    /// 支持本地化的文本
    /// </summary>
    public class ExtendText : Text,ILocalizable
    {
        protected override void Awake()
        {
            base.Awake();
            Localize();
            Localization.Register(this);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Localization.UnRegister(this);
        }

        public void Localize()
        {
            if (Localization.CurrFont != null && font != Localization.CurrFont)
                font = Localization.CurrFont;
        }
    }
}
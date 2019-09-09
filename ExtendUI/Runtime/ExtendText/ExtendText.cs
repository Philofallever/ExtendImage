﻿using UnityEngine;
using UnityEngine.UI;

namespace ExtendUI
{
    public class ExtendText : Text,ILocalizableText
    {
        protected override void Awake()
        {
            base.Awake();
            if (Localization.CurrLangCfg.Font)
                font = Localization.CurrLangCfg.Font;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Localization.Register(this);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            Localization.Ungister(this);
        }

        public void Localize(Font localFont)
        {
            if(localFont)
                font = localFont;
        }
    }
}
using System;
using UnityEngine;

namespace ExtendUI
{
    public static class ExtendUIUtility
    {
        internal static Func<int, bool, (Color, Color)> textColorFunc;
        /// <summary>
        /// 设置ExtendText获取颜色的API
        /// </summary>
        /// <param name="apiFunc">Func(int ColorId,bool hightLight,(Color sourceColor,Color gradientColor))</param>
        public static void SetTextColorAPI(Func<int, bool, (Color, Color)> apiFunc)
        {
            textColorFunc = apiFunc;
        }
    }
}
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ExtendUI
{
    /// <summary>
    /// 支持本地化的文本
    /// </summary>
    public class ExtendText : Text, ILocalizable
    {
        public enum GradientStyle
        {
            Local
          , Global
        }

        public enum GradientDirection
        {
            Vertical
          , Horizontal
        }

        //private int m_ColorId;
        //private bool m_Highlight = false;
        [SerializeField]
        private bool m_Shrink;

        [SerializeField]
        private bool m_Gradient;

        [SerializeField]
        private Color m_GradientColor = Color.black;

        [SerializeField]
        private GradientStyle m_GradientStyle = GradientStyle.Local;

        [SerializeField]
        private GradientDirection m_GradientDirection = GradientDirection.Vertical;

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

        /// <summary>
        /// 渐变色从Color线下渐变至GradientColor
        /// </summary>
        public Color GradientColor
        {
            get => m_GradientColor;
            set
            {
                if (m_GradientColor == value)
                    return;

                m_GradientColor = value;
                if (m_Gradient)
                    SetVerticesDirty();
            }
        }

        /// <summary>
        /// 渐变
        /// </summary>
        public bool Gradient
        {
            get => m_Gradient;
            set
            {
                if (m_Gradient == value)
                    return;

                m_Gradient = value;
                if (!supportRichText)
                    SetVerticesDirty();
            }
        }

        // 
        //public override string text
        //{
        //    set
        //    {
        //        // 这儿要处理富文本的ColorId,再往下处理不了
        //        if (supportRichText && !string.IsNullOrEmpty(value))
        //        {
        //            const string colorIdTag = @"<colorid=(\d+)>(.*)</colorid>";
        //            Regex.Replace(value, colorIdTag, ReplaceColorIdTag, RegexOptions.IgnoreCase, TimeSpan.FromSeconds(1));
        //        }

            //        // 源码
            //        if (string.IsNullOrEmpty(value))
            //        {
            //            if (string.IsNullOrEmpty(m_Text))
            //                return;

            //            m_Text = "";
            //            SetVerticesDirty();
            //        }
            //        else if (m_Text != value)
            //        {
            //            m_Text = value;
            //            SetVerticesDirty();
            //            SetLayoutDirty();
            //        }
            //    }
            //}

            //// 富文本标签替换
            //private string ReplaceColorIdTag(Match match)
            //{
            //    var colorIdStr   = match.Groups[1].Value;
            //    if (int.TryParse(colorIdStr, out var id))
            //    {
            //        var cfgColors = GetCfgColors(id, false);
            //        return string.Intern($"<color=#{ColorUtility.ToHtmlStringRGB(cfgColors.Source)}>{match.Groups[2].Value}</color>");
            //    }

            //    return match.Value;
            //}

            //public override Color color
            //{
            //    set
            //    {
            //        base.color = value;
            //        m_ColorId = 0;
            //    }
            //}

            //public int colorId
            //{
            //    get => m_ColorId;
            //    set
            //    {
            //        if (m_ColorId == value)
            //            return;

            //        var cfgColors = GetCfgColors(value, m_Highlight);
            //        m_GradientColor = cfgColors.Gradient;
            //        color           = cfgColors.Source;
            //        m_ColorId = value;
            //        SetVerticesDirty();
            //    }
            //}

            //public bool highlight
            //{
            //    get => m_Highlight;
            //    set
            //    {
            //        if(m_Highlight == value)
            //            return;

            //        if (m_ColorId == 0)
            //        {
            //            m_Highlight = value;
            //            return;
            //        }

            //        var cfgColors = GetCfgColors(m_ColorId,value);
            //        m_GradientColor = cfgColors.Gradient;
            //        var tempColorId = m_ColorId;
            //        color = cfgColors.Source;
            //        m_ColorId = tempColorId;
            //        m_Highlight = value;
            //        SetVerticesDirty();
            //    }
            //}

            //private (Color Source, Color Gradient) GetCfgColors(int colId,bool isHighLight)
            //{
            //    return ExtendUIUtility.textColorFunc?.Invoke(colId, isHighLight) ?? (Color.white, Color.black);
            //}

        public int VisibleLines { get; private set; }
        private void Shrink()
        {
            TextGenerationSettings settings = GetGenerationSettings(rectTransform.rect.size);
            if (m_Shrink)
            {
                settings.resizeTextForBestFit = false;
                Rect rect = rectTransform.rect;
                var height = cachedTextGenerator.GetPreferredHeight(text, settings);
                if (height > rect.height)
                {
                    var s = settings.fontSize;
                    for (int i = s; i >= 0; --i)
                    {
                        settings.fontSize = i;
                        var h = cachedTextGenerator.GetPreferredHeight(text, settings);
                        if (h <= rect.height) break;
                    }
                }
                cachedTextGenerator.Populate(text, settings);
            }
        }

        private readonly UIVertex[] m_TempVerts = new UIVertex[4];

        // 修改Mesh生成源码，增加渐变功能
        // BUG:多行文字在水平情况下Global渐变有小问题,原因是Gloal计算是取最后一字的顶点,多行最后一个字并不一定是最长的那一行的
        protected override void OnPopulateMesh(VertexHelper toFill)
        {
            if (font == null)
                return;

            m_DisableFontTextureRebuiltCallback = true;

            if (font == null)
                return;

            // We don't care if we the font Texture changes while we are doing our Update.
            // The end result of cachedTextGenerator will be valid for this instance.
            // Otherwise we can get issues like Case 619238.
            m_DisableFontTextureRebuiltCallback = true;

            Vector2 extents = rectTransform.rect.size;

            var settings = GetGenerationSettings(extents);
            cachedTextGenerator.PopulateWithErrors(text, settings, gameObject);
            Shrink();

            // Apply the offset to the vertices
            IList<UIVertex> verts         = cachedTextGenerator.verts;
            float           unitsPerPixel = 1 / pixelsPerUnit;
            //Last 4 verts are always a new line... (\n)
            int vertCount = verts.Count - 4;

            // We have no verts to process just return (case 1037923)
            if (vertCount <= 0)
            {
                toFill.Clear();
                return;
            }

            Vector2 roundingOffset = new Vector2(verts[0].position.x, verts[0].position.y) * unitsPerPixel;
            roundingOffset = PixelAdjustPoint(roundingOffset) - roundingOffset;
            toFill.Clear();
            if (roundingOffset != Vector2.zero)
            {
                #region 渐变功能

                var firstVert = verts[0];
                firstVert.position   *= unitsPerPixel;
                firstVert.position.x += roundingOffset.x;
                firstVert.position.y += roundingOffset.y;

                var lastVert = verts[vertCount - 1];
                lastVert.position   *= unitsPerPixel;
                lastVert.position.x += roundingOffset.x;
                lastVert.position.y += roundingOffset.y;

                #endregion

                for (int i = 0; i < vertCount; ++i)
                {
                    int tempVertsIndex = i & 3;
                    m_TempVerts[tempVertsIndex]            =  verts[i];
                    m_TempVerts[tempVertsIndex].position   *= unitsPerPixel;
                    m_TempVerts[tempVertsIndex].position.x += roundingOffset.x;
                    m_TempVerts[tempVertsIndex].position.y += roundingOffset.y;

                    #region 渐变功能

                    if (!supportRichText && m_Gradient)
                    {
                        switch (m_GradientDirection)
                        {
                            case GradientDirection.Vertical:
                                switch (m_GradientStyle)
                                {
                                    case GradientStyle.Local:
                                        m_TempVerts[tempVertsIndex].color = tempVertsIndex == 0 || tempVertsIndex == 1 ? color : m_GradientColor;
                                        break;
                                    case GradientStyle.Global:
                                        var t = (m_TempVerts[tempVertsIndex].position.y - firstVert.position.y) / (lastVert.position.y - firstVert.position.y);
                                        m_TempVerts[tempVertsIndex].color = Color.Lerp(color, m_GradientColor, t);
                                        break;
                                }

                                break;
                            case GradientDirection.Horizontal:
                                switch (m_GradientStyle)
                                {
                                    case GradientStyle.Local:
                                        m_TempVerts[tempVertsIndex].color = tempVertsIndex == 0 || tempVertsIndex == 3 ? color : m_GradientColor;
                                        break;
                                    case GradientStyle.Global:
                                        var t = (m_TempVerts[tempVertsIndex].position.x - firstVert.position.x) / (lastVert.position.x - firstVert.position.x);
                                        m_TempVerts[tempVertsIndex].color = Color.Lerp(color, m_GradientColor, t);
                                        break;
                                }

                                break;
                        }
                    }

                    #endregion

                    if (tempVertsIndex == 3)
                        toFill.AddUIVertexQuad(m_TempVerts);
                }
            }
            else
            {
                #region 渐变功能

                var firstVert = verts[0];
                firstVert.position *= unitsPerPixel;

                var lastVert = verts[vertCount - 1];
                lastVert.position *= unitsPerPixel;

                #endregion

                for (int i = 0; i < vertCount; ++i)
                {
                    int tempVertsIndex = i & 3;
                    m_TempVerts[tempVertsIndex]          =  verts[i];
                    m_TempVerts[tempVertsIndex].position *= unitsPerPixel;

                    #region 渐变功能

                    if (!supportRichText && m_Gradient)
                    {
                        switch (m_GradientDirection)
                        {
                            case GradientDirection.Vertical:
                                switch (m_GradientStyle)
                                {
                                    case GradientStyle.Local:
                                        m_TempVerts[tempVertsIndex].color = tempVertsIndex == 0 || tempVertsIndex == 1 ? color : m_GradientColor;
                                        break;
                                    case GradientStyle.Global:
                                        var t = (m_TempVerts[tempVertsIndex].position.y - firstVert.position.y) / (lastVert.position.y - firstVert.position.y);
                                        m_TempVerts[tempVertsIndex].color = Color.Lerp(color, m_GradientColor, t);
                                        break;
                                }

                                break;
                            case GradientDirection.Horizontal:
                                switch (m_GradientStyle)
                                {
                                    case GradientStyle.Local:
                                        m_TempVerts[tempVertsIndex].color = tempVertsIndex == 0 || tempVertsIndex == 3 ? color : m_GradientColor;
                                        break;
                                    case GradientStyle.Global:
                                        var t = (m_TempVerts[tempVertsIndex].position.x - firstVert.position.x) / (lastVert.position.x - firstVert.position.x);
                                        m_TempVerts[tempVertsIndex].color = Color.Lerp(color, m_GradientColor, t);
                                        break;
                                }

                                break;
                        }
                    }

                    #endregion

                    if (tempVertsIndex == 3)
                        toFill.AddUIVertexQuad(m_TempVerts);
                }
            }

            m_DisableFontTextureRebuiltCallback = false;

            VisibleLines = cachedTextGenerator.lineCount;
        }
    }
}
using UnityEngine;
using UnityEngine.UI;

namespace ExtendUI
{
    [DisallowMultipleComponent]
    [ExecuteAlways]
    [RequireComponent(typeof(RectTransform))]
    [AddComponentMenu("Layout/Wrap Content Height Fitter", 150)]
    public sealed class WrapContentHeightFitter : LayoutGroup, ILayoutSelfController
    {
        [Tooltip("如果目标有Text|LayoutGroup则控制其高度")]
        [SerializeField, Header("包围目标")]
        private RectTransform m_wrapRect;

        private Text m_Text;

        private Text text
        {
            get
            {
                if (!m_Text)
                    m_Text = m_wrapRect.GetComponent<Text>();

                return m_Text;
            }
        }

        private LayoutGroup m_layoutGroup;

        private LayoutGroup layoutGroup
        {
            get
            {
                if (!m_layoutGroup)
                    m_layoutGroup = m_wrapRect.GetComponent<LayoutGroup>();

                return m_layoutGroup;
            }
        }


        private Vector2 m_cachedSize;


        public override void CalculateLayoutInputHorizontal()
        {
            if (!m_wrapRect)
                return;

            // 缓存来保存原始宽度
            m_cachedSize = m_wrapRect.rect.size;
            var width = m_cachedSize.x;
            if (layoutGroup)
                width = LayoutUtility.GetPreferredWidth(m_wrapRect);

            width += m_Padding.left + m_Padding.right;
            SetLayoutInputForAxis(width, width, 0, 0);
            m_Tracker.Clear();
        }

        public override void CalculateLayoutInputVertical()
        {
            if (!m_wrapRect)
                return;

            var height = m_cachedSize.y;
            if (text || layoutGroup)
                height = LayoutUtility.GetPreferredHeight(m_wrapRect);

            height += m_Padding.top + m_Padding.bottom;
            SetLayoutInputForAxis(height, height, 0, 1);
            m_Tracker.Clear();
        }

        public override void SetLayoutHorizontal()
        {
            if (!m_wrapRect)
                return;

            if (!text && !layoutGroup)
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, preferredWidth);
        }

        public override void SetLayoutVertical()
        {
            if (!m_wrapRect)
                return;

            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, preferredHeight);

            // 子对象设置
            m_wrapRect.anchorMax = Vector2.one;
            m_wrapRect.anchorMin = Vector2.zero;
            m_wrapRect.offsetMax = new Vector2(-m_Padding.right, -m_Padding.top);
            m_wrapRect.offsetMin = new Vector2(m_Padding.left,   m_Padding.bottom);

            var flags = text || layoutGroup ? DrivenTransformProperties.SizeDeltaY : DrivenTransformProperties.SizeDelta;
            m_Tracker.Add(this, rectTransform, flags);

            flags = DrivenTransformProperties.AnchorMin | DrivenTransformProperties.AnchorMax | DrivenTransformProperties.AnchoredPosition;

            if (text || layoutGroup)
                flags |= DrivenTransformProperties.SizeDelta;

            m_Tracker.Add(this, m_wrapRect, flags);
        }

        protected override void OnValidate()
        {
            if (!m_wrapRect && rectTransform.childCount != 0)
                m_wrapRect = rectTransform.GetChild(0) as RectTransform;
            base.OnValidate();
        }

#if UNITY_EDITOR

        [ContextMenu("设置目标宽度")]
        private void SetWrapWidth()
        {
            m_wrapRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _width);
        }

        [ContextMenu("获得目标宽度")]
        private void GetWrapWidth()
        {
            _width = m_wrapRect.rect.width;
        }

        [SerializeField]
        private float _width = 200;
#endif
    }
}
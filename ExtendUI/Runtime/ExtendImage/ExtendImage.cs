using UnityEngine.Sprites;

namespace UnityEngine.UI
{
    [AddComponentMenu("UI/ExtendImage")]
    public class ExtendImage : Image
    {
        [SerializeField]
        private bool m_Grey = false;

        private static Material m_GreyMaterial; // 灰色材质

        private static Material GreyMaterial
        {
            get
            {
                if (m_GreyMaterial == null)
                    m_GreyMaterial = new Material(Shader.Find("UI/ImageGrey"));
                return m_GreyMaterial;
            }
        }

        /// <summary>
        ///  Image的Type为Simple时,镜像可选模式
        /// </summary>
        public enum Mirror
        {
            /// <summary>
            /// 不镜像
            /// </summary>
            None

          , /// <summary>
            /// 水平镜像
            /// </summary>
            Horizontal

          , /// <summary>
            /// 竖直镜像
            /// </summary>
            Vertical
        }

        [SerializeField]
        private Mirror m_Mirror = Mirror.None; // 镜像模式

        #region 重载部分

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            var activeSprite = overrideSprite ?? sprite;
            switch (type)
            {
                // 为了简洁,暂时只有simple方式且不使用精灵网格的才可以镜像
                case Type.Simple when !useSpriteMesh && activeSprite != null && m_Mirror != Mirror.None:
                    GenerateSimpleSprite(vh);
                    break;
                default:
#if UNITY_EDITOR && EXTEND_UI_DEBUG
                    print("Base OnPopulateMesh");
#endif
                    base.OnPopulateMesh(vh);
                    break;
            }
        }

        public override void SetNativeSize()
        {
            var activeSprite = overrideSprite ?? sprite;
            if (activeSprite != null)
            {
                var w = sprite.rect.width / pixelsPerUnit;
                var h = sprite.rect.height / pixelsPerUnit;
                if (m_Mirror == Mirror.Horizontal && !useSpriteMesh)
                    w *= 2;
                else if (m_Mirror == Mirror.Vertical && !useSpriteMesh)
                    h *= 2;

                rectTransform.anchorMax = rectTransform.anchorMin;
                rectTransform.sizeDelta = new Vector2(w, h);
                SetAllDirty();
            }
        }

        private void GenerateSimpleSprite(VertexHelper vh)
        {
            var lPreserveAspect = preserveAspect;
            var activeSprite    = overrideSprite ?? sprite;

            var v       = GetDrawingDimensions(lPreserveAspect);
            var v1      = v;
            var uv      = activeSprite != null ? DataUtility.GetOuterUV(activeSprite) : Vector4.zero;
            var color32 = color;
            vh.Clear();

            // 参考：https://github.com/codingriver/UnityProjectTest/blob/master/MirrorImage/Assets/Script/MirrorImage.cs
            if (m_Mirror == Mirror.Horizontal)
            {
                v.z = (v.x + v.z) / 2;
                vh.AddVert(new Vector3(v.x,  v.y),  color32, new Vector2(uv.x, uv.y));
                vh.AddVert(new Vector3(v.x,  v.w),  color32, new Vector2(uv.x, uv.w));
                vh.AddVert(new Vector3(v.z,  v.w),  color32, new Vector2(uv.z, uv.w));
                vh.AddVert(new Vector3(v.z,  v.y),  color32, new Vector2(uv.z, uv.y));
                vh.AddVert(new Vector3(v1.z, v1.y), color32, new Vector2(uv.x, uv.y));
                vh.AddVert(new Vector3(v1.z, v1.w), color32, new Vector2(uv.x, uv.w));

                vh.AddTriangle(0, 1, 2);
                vh.AddTriangle(2, 3, 0);
                vh.AddTriangle(3, 2, 5);
                vh.AddTriangle(5, 4, 3);
            }
            else if (m_Mirror == Mirror.Vertical)
            {
                v.w = (v.w + v.y) / 2;
                vh.AddVert(new Vector3(v.x,  v.y),  color32, new Vector2(uv.x, uv.y));
                vh.AddVert(new Vector3(v.x,  v.w),  color32, new Vector2(uv.x, uv.w));
                vh.AddVert(new Vector3(v.z,  v.w),  color32, new Vector2(uv.z, uv.w));
                vh.AddVert(new Vector3(v.z,  v.y),  color32, new Vector2(uv.z, uv.y));
                vh.AddVert(new Vector3(v1.x, v1.w), color32, new Vector2(uv.x, uv.y));
                vh.AddVert(new Vector3(v1.z, v1.w), color32, new Vector2(uv.z, uv.y));

                vh.AddTriangle(0, 1, 2);
                vh.AddTriangle(2, 3, 0);
                vh.AddTriangle(1, 4, 5);
                vh.AddTriangle(5, 2, 1);
            }
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            material = m_Grey ? GreyMaterial : null;
            if (type != Type.Simple || useSpriteMesh)
                m_Mirror = Mirror.None;
            base.OnValidate();
        }
#endif

        #endregion

        #region 需要的Image源码

        /// Image's dimensions used for drawing. X = left, Y = bottom, Z = right, W = top.
        private Vector4 GetDrawingDimensions(bool shouldPreserveAspect)
        {
            var activeSprite = overrideSprite ?? sprite;
            var padding      = activeSprite == null ? Vector4.zero : DataUtility.GetPadding(activeSprite);
            var size         = activeSprite == null ? Vector2.zero : new Vector2(activeSprite.rect.width, activeSprite.rect.height);

            var r = GetPixelAdjustedRect();
            // Debug.Log(string.Format("r:{2}, size:{0}, padding:{1}", size, padding, r));

            var spriteW = Mathf.RoundToInt(size.x);
            var spriteH = Mathf.RoundToInt(size.y);

            var v = new Vector4(
                                padding.x / spriteW
                              , padding.y / spriteH
                              , (spriteW - padding.z) / spriteW
                              , (spriteH - padding.w) / spriteH);

            if (shouldPreserveAspect && size.sqrMagnitude > 0.0f) PreserveSpriteAspectRatio(ref r, size);

            v = new Vector4(
                            r.x + r.width * v.x
                          , r.y + r.height * v.y
                          , r.x + r.width * v.z
                          , r.y + r.height * v.w
                           );

            return v;
        }

        private void PreserveSpriteAspectRatio(ref Rect rect, Vector2 spriteSize)
        {
            var spriteRatio = spriteSize.x / spriteSize.y;
            var rectRatio   = rect.width / rect.height;

            if (spriteRatio > rectRatio)
            {
                var oldHeight = rect.height;
                rect.height =  rect.width * (1.0f / spriteRatio);
                rect.y      += (oldHeight - rect.height) * rectTransform.pivot.y;
            }
            else
            {
                var oldWidth = rect.width;
                rect.width =  rect.height * spriteRatio;
                rect.x     += (oldWidth - rect.width) * rectTransform.pivot.x;
            }
        }

        #endregion
    }
}
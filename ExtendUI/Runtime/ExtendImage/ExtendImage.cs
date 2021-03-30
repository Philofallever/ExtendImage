using BlzClient;
using UnityEngine;
using UnityEngine.Sprites;
using UnityEngine.UI;

namespace ExtendUI
{
    [AddComponentMenu("UI/ExtendImage")]
    public class ExtendImage : Image, ILocalizable
    {
        [SerializeField]
        private bool m_Grey;

        [SerializeField]
        private bool m_CircleMask;

        [SerializeField]
        private bool m_Localiable = false;

        [SerializeField]
        private RectTransform m_HandleRect;

        private static Material m_GreyMaterial; // 灰色材质

        private static Material GreyMaterial
        {
            get
            {
                if (m_GreyMaterial == null)
                    m_GreyMaterial = Core.res.LoadGlobal<Material>("System/Mats/UIGray");
                return m_GreyMaterial;
            }
        }

        public bool Grey
        {
            get => m_Grey; 
            set
            {
                if (m_Grey == value)
                    return;

                m_Grey = value;
                material = m_Grey ? GreyMaterial : null;
                SetMaterialDirty();
            }
        }

        public bool CircleMask
        {
            get => m_CircleMask; 
            set
            {
                if (m_CircleMask == value)
                    return;

                m_CircleMask = value;
                SetMaterialDirty();
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

        protected override void Awake()
        {
            base.Awake();
            if (m_Localiable)
            {
                Localize();
                Localization.Register(this);
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (m_Localiable)
                Localization.UnRegister(this);
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            var activeSprite = overrideSprite ?? sprite;
            switch (type)
            {
                // 为了简洁,暂时只有simple方式且不使用精灵网格的才可以镜像
                case Type.Simple when !useSpriteMesh && activeSprite != null && m_Mirror != Mirror.None:
                    GenerateSimpleSprite(vh);
                    break;
                case Type.Simple when !useSpriteMesh && activeSprite != null && m_CircleMask:
                    CircleMaskSprite(vh);
                    break;
                default:
#if UNITY_EDITOR && EXTEND_UI_DEBUG
                    print("Base OnPopulateMesh");
#endif
                    base.OnPopulateMesh(vh);
                    break;
            }
            //UIVertex vert = new UIVertex();
            //for (int i = 0; i < vh.currentVertCount; i++)
            //{
            //    vh.PopulateUIVertex(ref vert, i);
            //    vert.uv1.x = (i >> 1);
            //    vert.uv1.y = ((i >> 1) ^ (i & 1));
            //    vert.uv2.x = m_Grey ? 1 : 0;
            //    vert.uv2.y = m_CircleMask ? 1 : 0;
            //    vh.SetUIVertex(vert, i);
            //}
            //if (m_GreyMaterial && (m_CircleMask || m_Grey))
            //{
            //    var uv = overrideSprite != null ? DataUtility.GetOuterUV(overrideSprite) : Vector4.zero;
            //    m_GreyMaterial.SetVector("_UvRect", uv);
            //    m_GreyMaterial.SetFloat("_ExtandUI", 1);
            //}
            if (type == Type.Filled)
            {
                FillHandle();
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

        private void CircleMaskSprite(VertexHelper vh)
        {
            vh.Clear();

            float tw = rectTransform.rect.width;
            float th = rectTransform.rect.height;
            float outerRadius = 0.5f * tw;
            float vertexCenterX = (0.5f - rectTransform.pivot.x) * tw;
            float vertexCenterY = (0.5f - rectTransform.pivot.y) * th;

            Vector4 uv = overrideSprite != null ? DataUtility.GetOuterUV(overrideSprite) : Vector4.zero;
            float uvCenterX = (uv.x + uv.z) * 0.5f;
            float uvCenterY = (uv.y + uv.w) * 0.5f;
            float uvRadius = (uv.z - uv.x) * 0.5f;

            float degreeDelta = (float)(2 * Mathf.PI / 30);
            int curSegements = 30;

            float curDegree = 0;
            UIVertex uiVertex;
            int verticeCount;
            int triangleCount;
            Vector2 curVertice;
            Vector2 curUV;
            curVertice = new Vector2(vertexCenterX, vertexCenterY);
            verticeCount = curSegements + 1;
            uiVertex = new UIVertex();
            uiVertex.color = color;
            uiVertex.position = curVertice;
            uiVertex.uv0 = new Vector2(uvCenterX, uvCenterY);
            vh.AddVert(uiVertex);

            for (int i = 1; i < verticeCount; i++)
            {
                float cosA = Mathf.Cos(curDegree);
                float sinA = Mathf.Sin(curDegree);
                curVertice = new Vector2(cosA * outerRadius, sinA * outerRadius);
                curUV = new Vector2(cosA * uvRadius, sinA * uvRadius);
                curDegree += degreeDelta;

                uiVertex = new UIVertex();
                uiVertex.color = color;
                uiVertex.position.x = vertexCenterX + curVertice.x;
                uiVertex.position.y = vertexCenterY + curVertice.y;
                uiVertex.uv0 = new Vector2(curUV.x + uvCenterX, curUV.y + uvCenterY);
                vh.AddVert(uiVertex);
            }
            triangleCount = curSegements * 3;
            for (int i = 0, vIdx = 1; i < triangleCount - 3; i += 3, vIdx++)
            {
                vh.AddTriangle(vIdx, 0, vIdx + 1);
            }
            vh.AddTriangle(verticeCount - 1, 0, 1);
        }

        private void FillHandle()
        {
            if (m_HandleRect == null) return;
            if(fillMethod == FillMethod.Horizontal || fillMethod == FillMethod.Vertical)
            {
                m_HandleRect.anchorMin = new Vector2(fillAmount, 0.0f);
                m_HandleRect.anchorMax = new Vector2(fillAmount, 1.0f);        
            }
            else if (fillMethod == FillMethod.Radial90)
            {
                m_HandleRect.localEulerAngles = new Vector3(0, 0, -fillAmount * 90);
            }
            else if (fillMethod == FillMethod.Radial180)
            {
                m_HandleRect.localEulerAngles = new Vector3(0, 0, -fillAmount * 180);
            }
            else if(fillMethod == FillMethod.Radial360)
            {
                m_HandleRect.localEulerAngles = new Vector3(0, 0, -fillAmount * 360);
            }
        }

#if UNITY_EDITOR
        //protected override void OnValidate()
        //{
        //    material = m_Grey ? GreyMaterial : null;
        //    if (type != Type.Simple || useSpriteMesh)
        //        m_Mirror = Mirror.None;
        //    base.OnValidate();
        //}
#endif
        // TODO 
        // public void Localize(Sprite localSprite)
        // {
        //    sprite = localSprite;
        // }

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

        public void Localize()
        {
            var localSprite = Localization.LocalizeSprite(sprite);
            sprite = localSprite;
        }
    }
}
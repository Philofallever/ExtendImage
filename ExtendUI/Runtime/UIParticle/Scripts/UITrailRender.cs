using BlzClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITrailRender : MaskableGraphic
{
    int mLastLayer = -1;
    Material mMaterial;
    List<Material> mMatList = new List<Material>();
    Dictionary<Material, int> mOriQueueDic;
    TrailRenderer mTr;
    Renderer mRender;
    Mesh mSmrM;
    static readonly List<Vector3> s_Vertices = new List<Vector3>();
    public override Texture mainTexture
    {
        get
        {
            Texture tex = null;
            if (!tex && mRender)
            {
                var mat = material;
                if (mat && mat.HasProperty("_MainTex"))
                {
                    tex = mat.mainTexture;
                }
            }
            return tex ?? s_WhiteTexture;
        }
    }

    public override Material material
    {
        get
        {
            if (gameObject.layer != 16 && gameObject.layer != 5 && gameObject.layer != 3)
                return null;
            return mMaterial;
        }

        set
        {
            if (!mRender)
            {
            }
            else if (mRender.sharedMaterial != value)
            {
                mRender.sharedMaterial = value;
                SetMaterialDirty();
            }
        }
    }

    protected override void Start()
    {
        base.Start();
        mSmrM = new Mesh();
        mSmrM.MarkDynamic();
        raycastTarget = false;
        mRender = gameObject.GetComponent<Renderer>();
        mTr = gameObject.GetComponent<TrailRenderer>();
        RefreshRender();
    }

    protected override void OnDestroy() {
        Destroy(mSmrM);

        base.OnDestroy();
    }

    private void LateUpdate()
    {
        if (mRender == null) return;
        if (mLastLayer != gameObject.layer)
        {
            mLastLayer = gameObject.layer;
            ChangeRenderState();
        }
        if (mTr != null && mRender.enabled)
        {
            mSmrM.Clear();
            if (mLastLayer == 16 || mLastLayer == 5 || mLastLayer == 3)
            {
                gameObject.layer = 3;
                Matrix4x4 matrix = default(Matrix4x4);
                Matrix4x4 rot = new Matrix4x4(new Vector4(1, 0, 0, 0), new Vector4(0, -1, 0, 0), new Vector4(0, 0, -1, 0), new Vector4(0, 0, 0, 0));
                matrix = Matrix4x4.Scale(rectTransform.lossyScale).inverse * rectTransform.worldToLocalMatrix;
                mTr.BakeMesh(mSmrM, BlzClient.Core.ui.mainCamera, true);
                mSmrM.GetVertices(s_Vertices);
                var count = s_Vertices.Count;
                for (int i = 0; i < count; i++)
                {
                    s_Vertices[i] = matrix.MultiplyPoint3x4(s_Vertices[i]);
                }
                mSmrM.SetVertices(s_Vertices);
                canvasRenderer.SetMesh(mSmrM);
            }
        }
    }

    [ContextMenu("Refresh")]
    public void RefreshRender()
    {
        if (mRender == null) return;
        var matList = mRender.materials;
        if (mOriQueueDic == null) mOriQueueDic = new Dictionary<Material, int>();
        mMatList.Clear();
        for (int i = 0; i < matList.Length; i++)
        {
            if (matList[i] == null) continue;
            if (!mOriQueueDic.ContainsKey(matList[i]))
                mOriQueueDic.Add(matList[i], matList[i].renderQueue);
            mMatList.Add(matList[i]);
        }
        mMatList.Sort((x, y) => x.renderQueue.CompareTo(y.renderQueue));
        mMaterial = mMatList[0];
        ChangeRenderState();
        SetMaterialDirty();
    }

    void ChangeRenderState()
    {
        if (mOriQueueDic != null)
        {
            foreach (var item in mOriQueueDic)
            {
                var mat = item.Key;
                mat.renderQueue = mLastLayer == 16 || mLastLayer == 5 || mLastLayer == 3 ? 3000 : item.Value;
            }
        }
        SetMaterialDirty();
    }
}

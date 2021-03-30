using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISubBody : MaskableGraphic
{
    Dictionary<Material, int> mOriQueueDic;
    Material mMaterial;
    Renderer mRender;
    MeshRenderer mMr;
    MeshFilter mMf;
    SkinnedMeshRenderer mSmr;
    Mesh mSmrM;
    bool dirty;
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
            if (dirty == false)
                return null;
            else
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
    }

    protected override void OnDestroy() {
        // 释放
        Destroy(mSmrM);

        base.OnDestroy();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        dirty = false;
    }

    private void LateUpdate()
    {
        if (mRender == null) return;
        if (mMr != null && mMf != null)
        {
            canvasRenderer.SetMesh(mMf.sharedMesh);
        }
        if (mSmr != null)
        {
            mSmrM.Clear();
            Matrix4x4 matrix = default(Matrix4x4);
            Matrix4x4 rot = new Matrix4x4(new Vector4(1, 0, 0, 0), new Vector4(0, -1, 0, 0), new Vector4(0, 0, -1, 0), new Vector4(0, 0, 0, 0));
            matrix = Matrix4x4.Scale(rectTransform.lossyScale).inverse;
            mSmr.BakeMesh(mSmrM);
            mSmrM.GetVertices(s_Vertices);
            var count = s_Vertices.Count;
            for (int i = 0; i < count; i++)
            {
                s_Vertices[i] = matrix.MultiplyPoint3x4(s_Vertices[i]);
            }
            mSmrM.SetVertices(s_Vertices);
            canvasRenderer.SetMesh(mSmrM);
        }
        if (dirty == false)
        {
            dirty = true;
            SetMaterialDirty();
        }
    }

    public void Init(Renderer render, MeshRenderer meshRender, MeshFilter meshFilter, SkinnedMeshRenderer skinMesh, Material mat)
    {
        this.mRender = render;
        this.mMr = meshRender;
        this.mMf = meshFilter;
        this.mSmr = skinMesh;
        this.mMaterial = mat;
    }
}

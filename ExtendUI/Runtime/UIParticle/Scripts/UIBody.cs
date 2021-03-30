using BlzClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBody : MaskableGraphic
{
    int mLastLayer = -1;
    Material mMaterial;
    List<Material> mMatList = new List<Material>();
    Dictionary<Material, int> mOriQueueDic;
    List<UISubBody> mSubBodyList;
    Renderer mRender;
    MeshRenderer mMr;
    MeshFilter mMf;
    SkinnedMeshRenderer mSmr;
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
            if (gameObject.layer != 15)
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
        mMr = gameObject.GetComponent<MeshRenderer>();
        if (mMr != null)
            mMf = gameObject.GetComponent<MeshFilter>();
        mSmr = gameObject.GetComponent<SkinnedMeshRenderer>();
        RefreshRender();
    }

    protected override void OnDestroy() {
        // 释放 Mesh
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
        if (mMr != null && mMf != null)
        {
            if (mLastLayer == 15)
            {
                try
                {
                    if (mMf.sharedMesh.isReadable)
                        canvasRenderer.SetMesh(mMf.sharedMesh);
                    else
                    {
#if UNITY_EDITOR
                        Debug.LogErrorFormat("Can`t get sharedMesh : {0}", mMr.name);
#endif
                    }
                    if (mMr.enabled) mMr.enabled = false;
                }
                catch
                {
#if UNITY_EDITOR
                    Debug.LogErrorFormat("Can`t get sharedMesh : {0}", mMr.name);
#endif
                }
            }
            else if (!mMr.enabled)
                mMr.enabled = true;
        }
        if (mSmr != null)
        {
            mSmrM.Clear();
            if (mLastLayer == 15)
            {
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
                if (mSmr.enabled) mSmr.enabled = false;
            }
            else if (!mSmr.enabled)
                mSmr.enabled = true;
        }
    }

    [ContextMenu("Refresh")]
    public void RefreshRender()
    {
        if (mRender == null) return;
//#if UNITY_EDITOR
        var matList = mRender.materials;
//#else
//        var matList = mRender.sharedMaterials;
//#endif
        if (mOriQueueDic == null) mOriQueueDic = new Dictionary<Material, int>();
        if (mSubBodyList == null) mSubBodyList = new List<UISubBody>();
        mMatList.Clear();
        for (int i = 0; i < matList.Length; i++)
        {
            if (matList[i] == null) continue;
            matList[i].SetFloat("_IsUI", 1);
            if (!mOriQueueDic.ContainsKey(matList[i]))
                mOriQueueDic.Add(matList[i], matList[i].renderQueue);
            mMatList.Add(matList[i]);
        }
        mMatList.Sort((x, y) => x.renderQueue.CompareTo(y.renderQueue));
        mMaterial = mMatList[0];
        for (int i = 1; i < mMatList.Count; i++)
        {
            if (mSubBodyList.Count >= i)
            {
                mSubBodyList[i - 1].gameObject.SetActive(true);
                mSubBodyList[i - 1].Init(mRender, mMr, mMf, mSmr, mMatList[i]);
            }
            else
            {
                GameObject go = GoHelper.CreateGameObject(transform, "sub" + i);
                var sub = go.AddComponent<UISubBody>();
                sub.Init(mRender, mMr, mMf, mSmr, mMatList[i]);
                mSubBodyList.Add(sub);
            }
        }
        if (mSubBodyList.Count > mMatList.Count - 1)
        {
            for (int i = mMatList.Count - 1; i < mSubBodyList.Count; i++)
            {
                mSubBodyList[i].gameObject.SetActive(false);
            }
        }
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
                mat.renderQueue = mLastLayer == 15 ? 3000 : item.Value;
            }
        }
        if (mSubBodyList != null)
        {
            for (int i = 0; i < mSubBodyList.Count; i++)
            {
                mSubBodyList[i].enabled = mLastLayer == 15;
            }
        }
        SetMaterialDirty();
    }
}

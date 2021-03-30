using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITrail : MaskableGraphic
{
    List<Vector3> s_Vertices = new List<Vector3>();
    Material mMaterial;

    public override Texture mainTexture
    {
        get
        {
            Texture tex = null;
            if (!tex)
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
            return mMaterial;
        }

        set
        {
            mMaterial = value;
        }
    }

    protected override void Start()
    {
        base.Start();
        color = new Color(0, 0, 0, 0);
        raycastTarget = false;
    }

    public void SetMesh(Mesh mesh, Material mat)
    {
        material = mat;
        mesh.GetVertices(s_Vertices);
        var count = s_Vertices.Count;
        for (int i = 0; i < count; i++)
        {
            s_Vertices[i] = rectTransform.worldToLocalMatrix.MultiplyPoint3x4(s_Vertices[i]);
        }
        mesh.SetVertices(s_Vertices);
       
        canvasRenderer.SetMesh(mesh);
        SetMaterialDirty();
    }
}

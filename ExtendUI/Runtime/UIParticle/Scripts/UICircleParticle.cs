using UnityEngine;
using System.Collections;


public class UICircleParticle : MonoBehaviour
{
    public float radius = 0;
    public float height = 0;
    public float width = 0;
    public int cornerindex = 0;

    public enum CircleType
    {
        round,
        quad
    }
    public CircleType circleType = CircleType.quad;
    public Vector3 rotatespeed;
    public float speed;
    public RectTransform targetRect;

    Transform root;
    Vector3[] corners;
    bool iscircle;
    float delta;
    Vector3 goal, orig;
    int index = 0;
    float offsets, realspeed;
    void Awake()
    {
        Init();
    }

    void PlayCircle()
    {
        iscircle = true;
        if (root == null)
            root = new GameObject().transform;

        root.parent = transform.parent;
        root.localPosition = transform.localPosition;
        root.localRotation = Quaternion.identity;
        root.localScale = Vector3.one;
        transform.parent = root;
        transform.localPosition = new Vector3(0, radius, 0);

        //		rotatespeed = new Vector3 (0, 0, 0.1f);
    }

    void PlayQuad()
    {
        iscircle = false;
        if (corners == null)
            corners = new Vector3[4];

        corners[0].Set(-width * 0.5f, height * 0.5f, 0);
        corners[1].Set(width * 0.5f, height * 0.5f, 0);
        corners[2].Set(width * 0.5f, -height * 0.5f, 0);
        corners[3].Set(-width * 0.5f, -height * 0.5f, 0);

        goal = corners[cornerindex];
        orig = corners[cornerindex];
        index = cornerindex;
        realspeed = speed;
        delta = 0;
        offsets = height / width;
    }

    [ContextMenu("Init")]
    public void Init()
    {
        if (targetRect != null)
        {
            width = targetRect.rect.width;
            height = targetRect.rect.height;
            if (targetRect.rect.width > targetRect.rect.height)
                radius = targetRect.rect.height;
            else
                radius = targetRect.rect.width;
        }

        switch (circleType)
        {
            case CircleType.quad:
                PlayQuad();
                break;
            default:
                iscircle = true;
                transform.localPosition = new Vector3(0, radius, 0);
                break;
        }
    }

    void Update()
    {
        if (iscircle)
            root.Rotate(rotatespeed);
        else
        {
            if (delta < 1)
                delta += realspeed * Time.deltaTime;
            if (Vector3.Distance(transform.localPosition, goal) < 0.1f)
            {
                if (index == 1 || index == 3)
                    realspeed /= offsets;
                else
                    realspeed *= offsets;
                orig = corners[index];
                goal = corners[(index + 1) % 4];
                index = (index + 1) % 4;
                transform.LookAt(goal);
                delta = 0;
            }
            transform.localPosition = Vector3.Lerp(orig, goal, delta);
        }
    }
}

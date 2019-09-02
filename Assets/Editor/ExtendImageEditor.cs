using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEditor.UI;
using UnityEngine.UI;

[CustomEditor(typeof(ExtendImage),true),CanEditMultipleObjects]
public class ExtendImageEditor :ImageEditor
{
    SerializedProperty m_Sprite;
    SerializedProperty m_Type;
    SerializedProperty m_PreserveAspect;
    SerializedProperty m_UseSpriteMesh;

    SerializedProperty m_Mirror;


    AnimBool m_ShowMirror;
    AnimBool m_ShowType;


    protected override void OnEnable()
    {
        m_Sprite = serializedObject.FindProperty("m_Sprite");
        m_Type = serializedObject.FindProperty("m_Type");
        m_PreserveAspect = serializedObject.FindProperty("m_PreserveAspect");
        m_UseSpriteMesh = serializedObject.FindProperty("m_UseSpriteMesh");
        m_Mirror = serializedObject.FindProperty("m_Mirror");

        m_ShowType = new AnimBool(m_Sprite.objectReferenceValue != null);
        var typeEnum = (Image.Type)m_Type.enumValueIndex;
        m_ShowMirror = new AnimBool(!m_Type.hasMultipleDifferentValues && typeEnum == Image.Type.Simple);
        m_ShowMirror.valueChanged.AddListener(Repaint);
        base.OnEnable();
    }

    protected override void OnDisable()
    {
        m_ShowMirror.valueChanged.RemoveListener(Repaint);
        base.OnDisable();
    }


    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        SpriteGUI();
        AppearanceControlsGUI();
        RaycastControlsGUI();

        m_ShowType.target = m_Sprite.objectReferenceValue != null;
        if (EditorGUILayout.BeginFadeGroup(m_ShowType.faded))
            TypeGUI();
        EditorGUILayout.EndFadeGroup();

        SetShowNativeSize(false);
        if (EditorGUILayout.BeginFadeGroup(m_ShowNativeSize.faded))
        {
            EditorGUI.indentLevel++;

            if ((Image.Type)m_Type.enumValueIndex == Image.Type.Simple)
            {
                EditorGUILayout.PropertyField(m_UseSpriteMesh);
                EditorGUILayout.PropertyField(m_Mirror);
            }

            EditorGUILayout.PropertyField(m_PreserveAspect);
            EditorGUI.indentLevel--;
        }
        EditorGUILayout.EndFadeGroup();
        serializedObject.ApplyModifiedProperties();
        
    }

    void SetShowNativeSize(bool instant)
    {
        Image.Type type = (Image.Type)m_Type.enumValueIndex;
        bool showNativeSize = (type == Image.Type.Simple || type == Image.Type.Filled) && m_Sprite.objectReferenceValue != null;
        base.SetShowNativeSize(showNativeSize, instant);
    }
}

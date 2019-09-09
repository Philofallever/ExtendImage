using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEditor.UI;
using UnityEngine.UI;

namespace ExtendUI
{
    [CustomEditor(typeof(ExtendImage), true), CanEditMultipleObjects]
    public class ExtendImageEditor : ImageEditor
    {
        private SerializedProperty m_Sprite;
        private SerializedProperty m_Type;
        private SerializedProperty m_PreserveAspect;
        private SerializedProperty m_UseSpriteMesh;

        private SerializedProperty m_Mirror;
        private SerializedProperty m_Grey;
        private SerializedProperty m_Localiable;
        private AnimBool           m_ShowImgType;

        protected override void OnEnable()
        {
            m_Sprite         = serializedObject.FindProperty("m_Sprite");
            m_Type           = serializedObject.FindProperty("m_Type");
            m_PreserveAspect = serializedObject.FindProperty("m_PreserveAspect");
            m_UseSpriteMesh  = serializedObject.FindProperty("m_UseSpriteMesh");
            m_Mirror         = serializedObject.FindProperty("m_Mirror");
            m_Grey           = serializedObject.FindProperty("m_Grey");
            m_Localiable     = serializedObject.FindProperty("m_Localiable");

            m_ShowImgType = new AnimBool(m_Sprite.objectReferenceValue != null);
            base.OnEnable();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.HelpBox("图片置灰、本地化及simple模式图片镜像", MessageType.Info, true);
            SpriteGUI();
            AppearanceControlsGUI();
            EditorGUILayout.PropertyField(m_Localiable);
            EditorGUILayout.PropertyField(m_Grey);
            RaycastControlsGUI();

            m_ShowImgType.target = m_Sprite.objectReferenceValue != null;
            if (EditorGUILayout.BeginFadeGroup(m_ShowImgType.faded))
                TypeGUI();
            EditorGUILayout.EndFadeGroup();

            SetShowNativeSize(false);
            if (EditorGUILayout.BeginFadeGroup(m_ShowNativeSize.faded))
            {
                EditorGUI.indentLevel++;

                if ((Image.Type) m_Type.enumValueIndex == Image.Type.Simple)
                {
                    EditorGUILayout.PropertyField(m_UseSpriteMesh);
                    EditorGUILayout.PropertyField(m_Mirror);
                }

                EditorGUILayout.PropertyField(m_PreserveAspect);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.EndFadeGroup();
            NativeSizeButtonGUI();

            serializedObject.ApplyModifiedProperties();
        }

        private void SetShowNativeSize(bool instant)
        {
            var type           = (Image.Type) m_Type.enumValueIndex;
            var showNativeSize = (type == Image.Type.Simple || type == Image.Type.Filled) && m_Sprite.objectReferenceValue != null;
            base.SetShowNativeSize(showNativeSize, instant);
        }
    }
}
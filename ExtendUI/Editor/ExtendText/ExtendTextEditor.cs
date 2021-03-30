using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEditor.UI;

namespace ExtendUI
{
    [CustomEditor(typeof(ExtendText), true), CanEditMultipleObjects]
    public class ExtendTextEditor : TextEditor
    {
        private SerializedProperty m_Text;
        private SerializedProperty m_FontData;
        private SerializedProperty m_Gradient;
        private SerializedProperty m_GradientColor;
        private SerializedProperty m_GradientStyle;
        private SerializedProperty m_GradientDirection;
        private SerializedProperty m_Shrink;

        private AnimBool m_ShowGradient;

        protected override void OnEnable()
        {
            m_Text     = serializedObject.FindProperty("m_Text");
            m_FontData = serializedObject.FindProperty("m_FontData");
            m_Gradient = serializedObject.FindProperty("m_Gradient");
            m_GradientColor = serializedObject.FindProperty(nameof(m_GradientColor));
            m_GradientStyle = serializedObject.FindProperty(nameof(m_GradientStyle));
            m_GradientDirection = serializedObject.FindProperty(nameof(m_GradientDirection));
            m_Shrink = serializedObject.FindProperty("m_Shrink");

            m_ShowGradient = new AnimBool(m_Gradient.boolValue);
            m_ShowGradient.valueChanged.AddListener(Repaint);
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            m_ShowGradient.valueChanged.RemoveListener(Repaint);
            base.OnDisable();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(m_Text);
            EditorGUILayout.PropertyField(m_FontData);
            RaycastControlsGUI();
            //AppearanceControlsGUI();

            m_ShowGradient.target = m_Gradient.boolValue;
            EditorGUILayout.PropertyField(m_Gradient);
            if (EditorGUILayout.BeginFadeGroup(m_ShowGradient.faded))
                GradientGUI();
            EditorGUILayout.EndFadeGroup();
            EditorGUILayout.PropertyField(m_Shrink);

            serializedObject.ApplyModifiedProperties();
        }

        protected void GradientGUI()
        {
            EditorGUI.indentLevel++;
            //EditorGUILayout.PropertyField(m_GradientColor);
            EditorGUILayout.PropertyField(m_GradientDirection);
            EditorGUILayout.PropertyField(m_GradientStyle);
            EditorGUI.indentLevel--;
        }
    }
}
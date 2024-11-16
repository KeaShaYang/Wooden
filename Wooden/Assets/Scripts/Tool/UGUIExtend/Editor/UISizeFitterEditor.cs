using UnityEngine;
using UnityEngine.UI;

namespace UnityEditor.UI
{
    [CustomEditor(typeof(UISizeFitter), true)]
    [CanEditMultipleObjects]
    public class UISizeFitterEditor : SelfControllerEditor
    {
        SerializedProperty m_HorizontalFit;
        SerializedProperty m_VerticalFit;
        SerializedProperty m_MaxHorizontal;
        SerializedProperty m_MaxVertical;
        SerializedProperty m_SyncUIHeight;
        SerializedProperty m_SyncUI;
        SerializedProperty m_SyncMaxHeight;
        SerializedProperty m_DisableScrollRect;
        SerializedProperty m_ScrollRect;
        SerializedProperty m_MyScrollRect;

        protected virtual void OnEnable()
        {
            m_HorizontalFit = serializedObject.FindProperty("m_HorizontalFit");
            m_VerticalFit = serializedObject.FindProperty("m_VerticalFit");
            m_MaxHorizontal = serializedObject.FindProperty("m_MaxHorizontal");
            m_MaxVertical = serializedObject.FindProperty("m_MaxVertical");
            m_SyncUIHeight = serializedObject.FindProperty("m_SyncUIHeight");
            m_SyncUI = serializedObject.FindProperty("m_SyncUI");
            m_SyncMaxHeight = serializedObject.FindProperty("m_SyncMaxHeight");
            m_DisableScrollRect = serializedObject.FindProperty("m_DisableScrollRect");
            m_ScrollRect = serializedObject.FindProperty("m_ScrollRect");
            m_MyScrollRect = serializedObject.FindProperty("m_MyScrollRect");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(m_HorizontalFit, true);
            if(m_HorizontalFit.intValue == 3) // CustomLimit
            {
                EditorGUILayout.PropertyField(m_MaxHorizontal);
            }
            EditorGUILayout.PropertyField(m_VerticalFit, true);
            if (m_VerticalFit.intValue == 3) // CustomLimit
            {
                EditorGUILayout.PropertyField(m_MaxVertical);
            }
            EditorGUILayout.PropertyField(m_SyncUIHeight);
            if (m_SyncUIHeight.boolValue)
            {
                EditorGUILayout.PropertyField(m_SyncUI);
                EditorGUILayout.PropertyField(m_SyncMaxHeight);
                EditorGUILayout.PropertyField(m_DisableScrollRect);
                if (m_DisableScrollRect.boolValue)
                {
                    EditorGUILayout.PropertyField(m_ScrollRect);
                    EditorGUILayout.PropertyField(m_MyScrollRect);
                }
            }

            serializedObject.ApplyModifiedProperties();

            base.OnInspectorGUI();
        }
    }
}

using UnityEditor;
using UnityEditor.UI;

[CustomEditor(typeof(TextGamma), true)]
[CanEditMultipleObjects]
public class TextGammaInspector : GraphicEditor
{
    SerializedProperty m_Text;
    SerializedProperty m_FontData;
    SerializedProperty m_IsNeedDealHeadChar;
    SerializedProperty m_UseShrinkBestFit;
    SerializedProperty m_BestFit;

    protected override void OnEnable()
    {
        base.OnEnable();
        m_Text = serializedObject.FindProperty("m_Text");
        m_FontData = serializedObject.FindProperty("m_FontData");
        m_BestFit = m_FontData.FindPropertyRelative("m_BestFit");
        m_UseShrinkBestFit = serializedObject.FindProperty("m_UseShrinkBestFit");
        m_IsNeedDealHeadChar = serializedObject.FindProperty("m_IsNeedDealHeadChar");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(m_Text);
        EditorGUILayout.PropertyField(m_FontData);
        if (m_BestFit.boolValue)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(m_UseShrinkBestFit);
            EditorGUI.indentLevel--;
        }

        AppearanceControlsGUI();
        RaycastControlsGUI();
        EditorGUILayout.PropertyField(m_IsNeedDealHeadChar);
        serializedObject.ApplyModifiedProperties();
    }
}

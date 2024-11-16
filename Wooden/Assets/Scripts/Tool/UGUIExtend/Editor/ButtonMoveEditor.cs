using UnityEngine;
using System.Collections;
using UnityEditor;
[CustomEditor(typeof(ButtonMove))]
public class ButtonMoveEditor : UnityEditor.Editor {

    private SerializedObject m_sprite;
    private SerializedProperty m_parent1,
        m_parent2,
        m_parent3,
        m_parentChildren1,
        m_parentChildren2,
        m_parentChildren3;

    void OnEnable()
    {
        m_sprite = new SerializedObject(target);
        m_parent1 = m_sprite.FindProperty("m_parent1");
        m_parent2 = m_sprite.FindProperty("m_parent2");
        m_parent3 = m_sprite.FindProperty("m_parent3");
        m_parentChildren1 = m_sprite.FindProperty("m_parentChildren1");
        m_parentChildren2 = m_sprite.FindProperty("m_parentChildren2");
        m_parentChildren3 = m_sprite.FindProperty("m_parentChildren3");
    }
    public override void OnInspectorGUI()
    {
        m_sprite.Update();
        EditorGUILayout.PropertyField(m_parent1);
        EditorGUILayout.PropertyField(m_parent2);
        EditorGUILayout.PropertyField(m_parent3);
        EditorGUILayout.PropertyField(m_parentChildren1);
        EditorGUILayout.PropertyField(m_parentChildren2);
        EditorGUILayout.PropertyField(m_parentChildren3);
        if (m_sprite.targetObject != null)
        {
            ButtonMove go = (ButtonMove)m_sprite.targetObject;
            if (go != null)
            {
                if (go.m_parent1 != null)
                {
                    for (int i = 0; i < go.m_parentChildren1.Count; ++i)
                    {
                        if (go.m_parentChildren1[i] == null)
                        {
                            continue;
                        }
                        go.m_parentChildren1[i].transform.position = go.m_parent1.transform.position;
                    }
                }
                if (go.m_parent2 != null)
                {
                    
                    for (int i = 0; i < go.m_parentChildren2.Count; ++i)
                    {
                        if (go.m_parentChildren2[i] == null)
                        {
                            continue;
                        }
                        go.m_parentChildren2[i].transform.position = go.m_parent2.transform.position;
                    }
                }
                if (go.m_parent3 != null)
                {
                    for (int i = 0; i < go.m_parentChildren3.Count; ++i)
                    {
                        if (go.m_parentChildren3[i] == null)
                        {
                            continue;
                        }
                        go.m_parentChildren3[i].transform.position = go.m_parent3.transform.position;
                    }
                }
            }
        }

        m_sprite.ApplyModifiedProperties();
        Repaint();
    }
}

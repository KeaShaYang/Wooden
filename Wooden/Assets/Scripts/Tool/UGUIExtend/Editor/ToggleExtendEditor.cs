using System.Collections;
using System.Collections.Generic;
using Tool.UGUIExtend.Scripts;
using UnityEditor;
using UnityEngine;

namespace Tool.UGUIExtend.Editor
{
    [CustomEditor(typeof(ToggleExtend), true)]
    public class ToggleExtendEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            ToggleExtend te = target as ToggleExtend;
            serializedObject.Update();
            SerializedProperty ExtendClickArea = serializedObject.FindProperty("m_ExtendClickArea");
            EditorGUILayout.PropertyField(ExtendClickArea);
            if (ExtendClickArea.boolValue)
            {
                EmptyImage ei = te.GetComponentInChildren<EmptyImage>();
                if (!ei)
                {
                    GameObject clickArea = new GameObject("Click Area", typeof(RectTransform), typeof(EmptyImage));
                    clickArea.transform.SetParent(te.transform, false);
                }
            }
            else
            {
                Transform m_ClickArea = te.transform.Find("Click Area");
                if(m_ClickArea!=null && m_ClickArea.GetComponent<EmptyImage>()!=null)
                {
                    DestroyImmediate(m_ClickArea.gameObject);
                }
            }
            
            SerializedProperty ToggleGameObject = serializedObject.FindProperty("m_ToggleGameObject");
            EditorGUILayout.PropertyField(ToggleGameObject);
            if (ToggleGameObject.boolValue)
            {
                SerializedProperty ObjOn = serializedObject.FindProperty("m_ObjOn");
                SerializedProperty ObjOff = serializedObject.FindProperty("m_ObjOff");
                EditorGUILayout.PropertyField(ObjOn);
                EditorGUILayout.PropertyField(ObjOff);
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}
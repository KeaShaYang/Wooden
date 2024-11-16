/*
作者：张阳
说明：UI序列帧动画
日期：2020-08-06
*/

using System;
using UnityEngine;

namespace UnityEditor
{
    public class UIFrameGUI : ShaderGUI
    {
        public static readonly string[] blendNames = Enum.GetNames(typeof(UIBlendMode));
        public static GUIContent rowNumText = EditorGUIUtility.TrTextContent("行数");
        public static GUIContent colNumText = EditorGUIUtility.TrTextContent("列数");
        public static GUIContent speedText = EditorGUIUtility.TrTextContent("播放速度");
        public static GUIContent lerpFrameText = EditorGUIUtility.TrTextContent("差值平滑");
        MaterialProperty rowNum = null;
        MaterialProperty colNum = null;
        MaterialProperty speed = null;
        MaterialProperty lerpFrame = null;
        MaterialProperty blendMode = null;
        MaterialEditor m_MaterialEditor = null;
        bool m_FirstTimeApply = true;

        /// <summary>
        /// 混合模式
        /// </summary>
        public enum UIBlendMode
        {
            Opaque,
            Transparent
        }


        /// <summary>
        /// 查找属性
        /// </summary>
        public void FindProperties(MaterialProperty[] props)
        {
            rowNum = FindProperty("_RowNum", props); ;
            colNum = FindProperty("_ColNum", props); ;
            speed = FindProperty("_Speed", props); ;
            lerpFrame = FindProperty("_LerpFrame", props); ;
            blendMode = FindProperty("_Mode", props);
        }

        /// <summary>
        /// 显示
        /// </summary>
        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props)
        {
            FindProperties(props);
            m_MaterialEditor = materialEditor;
            Material material = materialEditor.target as Material;
            EditorGUIUtility.labelWidth = 60f;
            if (m_FirstTimeApply)
            {
                SetUIMaterialBlendMode(material, (UIBlendMode)material.GetFloat("_Mode"));
                m_FirstTimeApply = false;
            }
            EditorGUI.BeginChangeCheck();
            {
                UIBlendModePopup(m_MaterialEditor, blendMode);
                m_MaterialEditor.ShaderProperty(rowNum, rowNumText.text);
                m_MaterialEditor.ShaderProperty(colNum, colNumText.text);
                m_MaterialEditor.ShaderProperty(speed, speedText.text);
                m_MaterialEditor.ShaderProperty(lerpFrame, lerpFrameText.text);
            }

            if (EditorGUI.EndChangeCheck())
            {
                foreach (var obj in blendMode.targets)
                {
                    Material mat = (Material)obj;
                    SetUIMaterialBlendMode(mat, (UIBlendMode)mat.GetFloat("_Mode"));
                }
            }
            material.renderQueue = EditorGUILayout.IntField(BaseShaderGUI.renderQueueText, material.renderQueue);
        }


        /// <summary>
        /// 渲染模式
        /// </summary>
        public static void UIBlendModePopup(MaterialEditor md, MaterialProperty blendMode)
        {
            EditorGUI.showMixedValue = blendMode.hasMixedValue;
            var mode = (UIBlendMode)blendMode.floatValue;
            EditorGUI.BeginChangeCheck();
            mode = (UIBlendMode)EditorGUILayout.Popup(BaseShaderGUI.renderingMode, (int)mode, blendNames);
            if (EditorGUI.EndChangeCheck())
            {
                md.RegisterPropertyChangeUndo("Rendering Mode");
                blendMode.floatValue = (float)mode;
            }
            EditorGUI.showMixedValue = false;
        }

        /// <summary>
        /// 设置材质混合模式
        /// </summary>
        public static void SetUIMaterialBlendMode(Material material, UIBlendMode blendMode)
        {
            switch (blendMode)
            {
                case UIBlendMode.Opaque:
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    break;
                case UIBlendMode.Transparent:
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    break;
            }
        }
    }
}
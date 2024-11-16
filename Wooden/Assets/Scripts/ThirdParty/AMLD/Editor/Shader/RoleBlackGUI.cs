/*
作者：张阳
说明：角色黑色边缘光
日期：2020-11-10
*/

using UnityEngine;

namespace UnityEditor
{
    public class RoleBlackGUI : ShaderGUI
    {
        public static GUIContent lightFactorText = EditorGUIUtility.TrTextContent("灯光影响程度");

        public static GUIContent centerAlphaText = EditorGUIUtility.TrTextContent("中心透明度");
        public static GUIContent centerAddColorText = EditorGUIUtility.TrTextContent("中心叠加颜色");
        public static GUIContent centerHueText = EditorGUIUtility.TrTextContent("中心色相");
        public static GUIContent centerSaturationText = EditorGUIUtility.TrTextContent("中心饱和度");
        public static GUIContent centerLightnessText = EditorGUIUtility.TrTextContent("中心明度");

        public static GUIContent edgeWidthText = EditorGUIUtility.TrTextContent("边缘宽度");
        public static GUIContent edgeAddColorText = EditorGUIUtility.TrTextContent("边缘叠加颜色");
        public static GUIContent edgeHueText = EditorGUIUtility.TrTextContent("边缘色相");
        public static GUIContent edgeSaturationText = EditorGUIUtility.TrTextContent("边缘饱和度");
        public static GUIContent edgeLightnessText = EditorGUIUtility.TrTextContent("边缘明度");

        MaterialProperty blendMode = null;
        MaterialProperty albedoMap = null;
        MaterialProperty albedoColor = null;
        MaterialProperty alphaCutoff = null;
        MaterialProperty alpha = null;
        MaterialProperty lightFactor = null;

        MaterialProperty centerAlpha = null;
        MaterialProperty centerAddColor = null;
        MaterialProperty centerHue = null;
        MaterialProperty centerSaturation = null;
        MaterialProperty centerLightness = null;

        MaterialProperty edgeWidth = null;
        MaterialProperty edgeAddColor = null;
        MaterialProperty edgeHue = null;
        MaterialProperty edgeSaturation = null;
        MaterialProperty edgeLightness = null;

        MaterialEditor m_MaterialEditor = null;
        bool m_FirstTimeApply = true;

        /// <summary>
        /// 查找属性
        /// </summary>
        public void FindProperties(MaterialProperty[] props)
        {
            blendMode = FindProperty("_Mode", props);
            albedoMap = FindProperty("_MainTex", props);
            albedoColor = FindProperty("_BlendColor", props);
            alphaCutoff = FindProperty("_Cutoff", props);
            alpha = FindProperty("_Alpha", props);
            lightFactor = FindProperty("_LightFactor", props);
            centerAlpha = FindProperty("_CenterAlpha", props);
            centerAddColor = FindProperty("_CenterAddColor", props);
            centerHue = FindProperty("_CenterHue", props);
            centerSaturation = FindProperty("_CenterSaturation", props);
            centerLightness = FindProperty("_CenterLightness", props);
            edgeWidth = FindProperty("_EdgeWidth", props);
            edgeAddColor = FindProperty("_EdgeAddColor", props);
            edgeHue = FindProperty("_EdgeHue", props);
            edgeSaturation = FindProperty("_EdgeSaturation", props);
            edgeLightness = FindProperty("_EdgeLightness", props);
        }

        /// <summary>
        /// 显示
        /// </summary>
        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props)
        {
            FindProperties(props);
            m_MaterialEditor = materialEditor;
            Material material = materialEditor.target as Material;
            if (m_FirstTimeApply)
            {
                BaseShaderGUI.MaterialChanged(material,1, false,false);
                m_FirstTimeApply = false;
            }

            bool bModeUpdate = false;
            EditorGUI.BeginChangeCheck();
            {
                bModeUpdate = BaseShaderGUI.BlendModePopup(m_MaterialEditor, blendMode);
                BaseShaderGUI.DoAlbedoArea(m_MaterialEditor, albedoColor, albedoMap, material, alphaCutoff, alpha);
                m_MaterialEditor.ShaderProperty(lightFactor, lightFactorText.text);
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("中心区域", EditorStyles.boldLabel);
                m_MaterialEditor.ShaderProperty(centerAddColor, centerAddColorText.text);
                m_MaterialEditor.ShaderProperty(centerAlpha, centerAlphaText.text);
                m_MaterialEditor.ShaderProperty(centerHue, centerHueText.text);
                m_MaterialEditor.ShaderProperty(centerSaturation, centerSaturationText.text);
                m_MaterialEditor.ShaderProperty(centerLightness, centerLightnessText.text);
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("边缘区域", EditorStyles.boldLabel);
                m_MaterialEditor.ShaderProperty(edgeWidth, edgeWidthText.text);
                m_MaterialEditor.ShaderProperty(edgeAddColor, edgeAddColorText.text);
                m_MaterialEditor.ShaderProperty(edgeHue, edgeHueText.text);
                m_MaterialEditor.ShaderProperty(edgeSaturation, edgeSaturationText.text);
                m_MaterialEditor.ShaderProperty(edgeLightness, edgeLightnessText.text);
            }

            if (EditorGUI.EndChangeCheck()|| bModeUpdate)
            {
                foreach (var obj in blendMode.targets)
                {
                    BaseShaderGUI.MaterialChanged((Material)obj,1, bModeUpdate, false);
                }
            }

            BaseShaderGUI.DoEnd(m_MaterialEditor,true);
        }

        /// <summary>
        /// 更换新shader
        /// </summary>
        public override void AssignNewShaderToMaterial(Material material, Shader oldShader, Shader newShader)
        {
            base.AssignNewShaderToMaterial(material, oldShader, newShader);
            BaseShaderGUI.AssignNewShaderToMaterial(material, oldShader, newShader);
            if(newShader.name == "AMLD/role/role_bloom_black")
            {
                BaseShaderGUI.MaterialChanged(material,1,false,false);
            }
            else
            {
                BaseShaderGUI.MaterialChanged(material);
            }
        }
    }
}
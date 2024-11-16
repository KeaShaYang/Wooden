/*
作者：张阳
说明：水晶玉石Shader界面
日期：2021-01-06
*/

using UnityEngine;

namespace UnityEditor
{
    public class BaseCrystalGUI : ShaderGUI
    {
        public static GUIContent mainOffsetXText = EditorGUIUtility.TrTextContent("颜色贴图流动速度X");
        public static GUIContent mainOffsetYText = EditorGUIUtility.TrTextContent("颜色贴图流动速度Y");
        public static GUIContent parallaxScaleText = EditorGUIUtility.TrTextContent("视差缩放");
        public static GUIContent causticColorText = EditorGUIUtility.TrTextContent("焦散颜色");
        public static GUIContent causticMapText = EditorGUIUtility.TrTextContent("焦散贴图");
        public static GUIContent causticOffsetXText = EditorGUIUtility.TrTextContent("焦散贴图流动速度X");
        public static GUIContent causticOffsetYText = EditorGUIUtility.TrTextContent("焦散贴图流动速度Y");
        public static GUIContent fresnelShininessText = EditorGUIUtility.TrTextContent("菲涅尔强度");
        public static GUIContent fresnelBrightnessText = EditorGUIUtility.TrTextContent("菲涅尔亮度");
        public static GUIContent specularShininessText = EditorGUIUtility.TrTextContent("光泽度");
        public static GUIContent specularBrightnessText = EditorGUIUtility.TrTextContent("高光强度");
        public static GUIContent lightFactorText = EditorGUIUtility.TrTextContent(" 光照影响程度");
        
        public static GUIContent FogFactorText = EditorGUIUtility.TrTextContent("雾效强度");
        public static GUIContent FogPixelText = EditorGUIUtility.TrTextContent("开启像素雾");

        MaterialProperty blendMode = null;
        MaterialProperty albedoMap = null;
        MaterialProperty bumpMap = null;
        MaterialProperty albedoColor = null;
        MaterialProperty alphaCutoff = null;
        MaterialProperty alpha = null;

        MaterialProperty mainOffsetX = null;
        MaterialProperty mainOffsetY = null;
        MaterialProperty parallaxScale = null;
        MaterialProperty causticColor = null;
        MaterialProperty causticMap = null;
        MaterialProperty causticOffsetX = null;
        MaterialProperty causticOffsetY = null;
        MaterialProperty fresnelShininess = null;
        MaterialProperty fresnelBrightness = null;
        MaterialProperty specularShininess = null;
        MaterialProperty specularBrightness = null;
        MaterialProperty lightFactor = null;

        MaterialProperty fogFactor = null;
        MaterialProperty emissionColor = null;
        MaterialProperty emissionMap = null;

        BaseShaderGUI.WorkflowMode m_WorkflowMode = BaseShaderGUI.WorkflowMode.Metallic;
        MaterialEditor m_MaterialEditor = null;
        bool m_FirstTimeApply = true;

        /// <summary>
        /// 查找属性
        /// </summary>
        public void FindProperties(MaterialProperty[] props)
        {
            blendMode = FindProperty("_Mode", props);
            albedoMap = FindProperty("_MainTex", props);
            bumpMap = FindProperty("_BumpMap", props);
            albedoColor = FindProperty("_Color", props);
            alphaCutoff = FindProperty("_Cutoff", props);
            alpha = FindProperty("_Alpha", props);

            mainOffsetX = FindProperty("_MainOffsetX", props);
            mainOffsetY = FindProperty("_MainOffsetY", props);
            parallaxScale = FindProperty("_ParallaxScale", props);
            causticColor = FindProperty("_CausticColor", props);
            causticMap = FindProperty("_CausticMap", props);
            causticOffsetX = FindProperty("_CausticOffsetX", props);
            causticOffsetY = FindProperty("_CausticOffsetY", props);
            fresnelShininess = FindProperty("_FresnelShininess", props);
            fresnelBrightness = FindProperty("_FresnelBrightness", props);
            specularShininess = FindProperty("_SpecularShininess", props);
            specularBrightness = FindProperty("_SpecularBrightness", props);
            lightFactor = FindProperty("_LightFactor", props);

            fogFactor = FindProperty("_FogFactor", props);
            emissionColor = FindProperty("_EmissionColor", props);
            emissionMap = FindProperty("_EmissionMap", props);
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
                BaseShaderGUI.MaterialChanged(material,0,false);
                m_FirstTimeApply = false;
            }
            bool bModeUpdate = false;
            EditorGUI.BeginChangeCheck();
            {
                bModeUpdate = BaseShaderGUI.BlendModePopup(m_MaterialEditor, blendMode);
                if (alphaCutoff != null)
                {
                    if (((BaseShaderGUI.BlendMode)material.GetFloat("_Mode") == BaseShaderGUI.BlendMode.Cutout))
                    {
                        m_MaterialEditor.ShaderProperty(alphaCutoff, BaseShaderGUI.CutoffText.text);
                    }
                }
                if (alpha != null)
                {
                    if ((BaseShaderGUI.BlendMode)material.GetFloat("_Mode") == BaseShaderGUI.BlendMode.Transparent)
                    {
                        m_MaterialEditor.ShaderProperty(alpha, BaseShaderGUI.AlphaText.text);
                    }
                }
                m_MaterialEditor.SetDefaultGUIWidths();

                m_MaterialEditor.ShaderProperty(albedoColor, BaseShaderGUI.ColorText);
                m_MaterialEditor.TextureProperty(albedoMap, BaseShaderGUI.MainTexText.text);
                m_MaterialEditor.TextureProperty(bumpMap, BaseShaderGUI.BumpMapText.text);

                m_MaterialEditor.ShaderProperty(mainOffsetX, mainOffsetXText.text);
                m_MaterialEditor.ShaderProperty(mainOffsetY, mainOffsetYText.text);
                m_MaterialEditor.ShaderProperty(parallaxScale, parallaxScaleText.text);

                m_MaterialEditor.ShaderProperty(causticColor, causticColorText);
                m_MaterialEditor.TextureProperty(causticMap, causticMapText.text);
                m_MaterialEditor.ShaderProperty(causticOffsetX, causticOffsetXText.text);
                m_MaterialEditor.ShaderProperty(causticOffsetY, causticOffsetYText.text);

                m_MaterialEditor.ShaderProperty(fresnelShininess, fresnelShininessText.text);
                m_MaterialEditor.ShaderProperty(fresnelBrightness, fresnelBrightnessText.text);
                m_MaterialEditor.ShaderProperty(specularShininess, specularShininessText.text);
                m_MaterialEditor.ShaderProperty(specularBrightness, specularBrightnessText.text);
                m_MaterialEditor.ShaderProperty(lightFactor, lightFactorText.text);

                m_MaterialEditor.ShaderProperty(fogFactor, FogFactorText.text);

                BaseShaderGUI.DoEmissionArea(m_MaterialEditor, emissionColor, emissionMap);
            }

            if (EditorGUI.EndChangeCheck() || bModeUpdate)
            {
                foreach (var obj in blendMode.targets)
                {
                    BaseShaderGUI.MaterialChanged((Material)obj,0, bModeUpdate);
                }
            }

            BaseShaderGUI.DoEnd(m_MaterialEditor, true);
        }

        /// <summary>
        /// 更换新shader
        /// </summary>
        public override void AssignNewShaderToMaterial(Material material, Shader oldShader, Shader newShader)
        {
            base.AssignNewShaderToMaterial(material, oldShader, newShader);
            BaseShaderGUI.AssignNewShaderToMaterial(material, oldShader, newShader);
            BaseShaderGUI.MaterialChanged(material,0);
        }
    }
}
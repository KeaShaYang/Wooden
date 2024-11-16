/*
作者：张阳
说明：角色身体
日期：2019-11-18
*/

using UnityEngine;

namespace UnityEditor
{
    public class RoleBodyGUI : ShaderGUI
    {
        public static GUIContent RoleMaskMapText = EditorGUIUtility.TrTextContent("混合贴图(R 粗糙度 G 金属度 B AO遮罩 A 皮肤遮罩)");
        public static GUIContent sssColorText = EditorGUIUtility.TrTextContent("皮肤混合颜色");
        public static GUIContent sssSkinMapText = EditorGUIUtility.TrTextContent("皮肤过渡贴图");
        public static GUIContent sssRangeText = EditorGUIUtility.TrTextContent("皮肤散色强度");
        public static GUIContent sssTranslucencyColorText = EditorGUIUtility.TrTextContent("皮肤通透颜色");
        public static GUIContent sssTranslucencyText = EditorGUIUtility.TrTextContent("皮肤通透强度(混合贴图G通道)");
        public static GUIContent ssssScalerText = EditorGUIUtility.TrTextContent("磨皮强度(需挂SSSSSkin脚本才生效)");
        

        MaterialProperty blendMode = null;
        MaterialProperty albedoMap = null;
        MaterialProperty albedoColor = null;
        MaterialProperty alphaCutoff = null;
        MaterialProperty alpha = null;
        MaterialProperty bumpMap = null;
        MaterialProperty maskMap = null;
        MaterialProperty roughness = null;
        MaterialProperty metallic = null;
        MaterialProperty reflection = null;
        MaterialProperty occlusion = null;
        MaterialProperty emissionColor = null;

        MaterialProperty sssColor = null;
        MaterialProperty sssSkinMap = null;
        MaterialProperty sssRange = null;
        MaterialProperty sssTranslucencyColor = null;
        MaterialProperty sssTranslucency = null;
        MaterialProperty ssssScaler = null;

        MaterialProperty dissolveTexScale = null;
        MaterialProperty dissolveValue = null;
        //MaterialProperty dissolveNoiseType = null;
        MaterialProperty dissolveRampType = null;
        MaterialProperty flashValue = null;
        MaterialProperty flashColor = null;
        MaterialProperty flashWidth = null;
        MaterialProperty effectStream = null;
        MaterialProperty streamFactor = null;
        MaterialProperty streamColorFactor = null;
        MaterialProperty streamTexFactor = null;
        MaterialProperty streamOffsetX = null;
        MaterialProperty streamOffsetY = null;
        MaterialProperty streamTex = null;
        MaterialProperty lodQualityLow = null;

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
            albedoColor = FindProperty("_Color", props);
            alphaCutoff = FindProperty("_Cutoff", props);
            alpha = FindProperty("_Alpha", props);
            bumpMap = FindProperty("_BumpMap", props);
            maskMap = FindProperty("_MaskMap", props);
            roughness = FindProperty("_Roughness", props);
            metallic = FindProperty("_Metallic", props);
            reflection = FindProperty("_Reflection", props);
            occlusion = FindProperty("_Occlusion", props);
            emissionColor = FindProperty("_EmissionColor", props);
            sssColor = FindProperty("_SSSColor", props);
            sssSkinMap = FindProperty("_SSSSkinMap", props);
            //sssRange = FindProperty("_SSSRange", props);
            sssTranslucencyColor = FindProperty("_SSSTranslucencyColor", props);
            //sssTranslucency = FindProperty("_SSSTranslucency", props);
            ssssScaler = FindProperty("_SSSSScaler", props);
            dissolveTexScale = FindProperty("_DissolveNoiseTexScale", props);
            dissolveValue = FindProperty("_DissolveValue", props);
            dissolveRampType = FindProperty("_DissolveRampType", props);
            //dissolveNoiseType = FindProperty("_DissolveNoiseType", props);
            flashValue = FindProperty("_FlashValue", props);
            flashColor = FindProperty("_FlashColor", props);
            flashWidth = FindProperty("_FlashWidth", props);
            effectStream = FindProperty("_EffectStream", props);
            streamFactor = FindProperty("_StreamFactor", props);
            streamColorFactor = FindProperty("_StreamColorFactor", props);
            streamTexFactor = FindProperty("_StreamTexFactor", props);
            streamOffsetX = FindProperty("_StreamOffsetX", props);
            streamOffsetY = FindProperty("_StreamOffsetY", props);
            streamTex = FindProperty("_StreamTex", props);
            lodQualityLow = FindProperty("_LodQualityLow", props);
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
                BaseShaderGUI.MaterialChanged(material,1,false);
                m_FirstTimeApply = false;
            }

            bool bModeUpdate = false;
            EditorGUI.BeginChangeCheck();
            {
                bModeUpdate = BaseShaderGUI.BlendModePopup(m_MaterialEditor, blendMode);
                BaseShaderGUI.DoAlbedoArea(m_MaterialEditor, albedoColor, albedoMap, material, alphaCutoff, alpha);
                BaseShaderGUI.DoNormalArea(m_MaterialEditor, bumpMap);
                BaseShaderGUI.DoMaskArea(m_MaterialEditor, m_WorkflowMode, maskMap,null, roughness, metallic, reflection, occlusion, RoleMaskMapText);

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("皮肤", EditorStyles.boldLabel);
                m_MaterialEditor.TexturePropertySingleLine(sssSkinMapText, sssSkinMap);

                BaseShaderGUI.ColorField(m_MaterialEditor, sssColorText.text, sssColor, false);
                //m_MaterialEditor.ShaderProperty(sssRange, sssRangeText.text);
                BaseShaderGUI.ColorField(m_MaterialEditor, sssTranslucencyColorText.text, sssTranslucencyColor, false);
                //m_MaterialEditor.ShaderProperty(sssTranslucency, sssTranslucencyText.text);
                m_MaterialEditor.ShaderProperty(ssssScaler, ssssScalerText.text);

                BaseShaderGUI.DoDissolveArea(m_MaterialEditor, dissolveValue, dissolveTexScale, dissolveRampType/*, dissolveNoiseType*/);
                BaseShaderGUI.DoFlashArea(m_MaterialEditor, flashValue, flashColor, flashWidth);
                BaseShaderGUI.DoStreamArea(m_MaterialEditor, effectStream, streamFactor,streamColorFactor, streamTexFactor, streamOffsetX, streamOffsetY, streamTex);
                BaseShaderGUI.DoEmissionArea(m_MaterialEditor, emissionColor);

                m_MaterialEditor.ShaderProperty(lodQualityLow, BaseShaderGUI.LodQualityLowText.text);
            }

            if (EditorGUI.EndChangeCheck()|| bModeUpdate)
            {
                foreach (var obj in blendMode.targets)
                {
                    BaseShaderGUI.MaterialChanged((Material)obj,1, bModeUpdate);
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
            BaseShaderGUI.MaterialChanged(material,1);
        }
    }
}
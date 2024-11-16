/*
作者：黄云龙
说明：角色头发
日期：2019-11-21
*/

using UnityEngine;

namespace UnityEditor
{
    public class RoleHairGUI : ShaderGUI
    {
        public static GUIContent AnisoMapText = EditorGUIUtility.TrTextContent("遮罩贴图(R 高光1遮罩 G 高光2遮罩 B 高光偏移 A AO遮罩)");
        public static GUIContent TangentDirText = EditorGUIUtility.TrTextContent("高光 流动方向");
        public static GUIContent AnisoSpecColor1Text = EditorGUIUtility.TrTextContent("高光1 颜色");
        public static GUIContent AnisoGloss1Text = EditorGUIUtility.TrTextContent("高光1 Gloss");
        public static GUIContent AnisoSpec1Text = EditorGUIUtility.TrTextContent("高光1 Specular");
        public static GUIContent TangentShift1Text = EditorGUIUtility.TrTextContent("高光1 流动偏移");
        public static GUIContent AnisoSpecColor2Text = EditorGUIUtility.TrTextContent("高光2 颜色");
        public static GUIContent AnisoGloss2Text = EditorGUIUtility.TrTextContent("高光2 Gloss");
        public static GUIContent AnisoSpec2Text = EditorGUIUtility.TrTextContent("高光2 Specular");
        public static GUIContent TangentShift2Text = EditorGUIUtility.TrTextContent("高光2 流动偏移");
        public static GUIContent ShowTangentText = EditorGUIUtility.TrTextContent("显示世界空间切线");

        MaterialProperty blendMode = null;
        MaterialProperty albedoMap = null;
        MaterialProperty albedoColor = null;
        MaterialProperty alphaCutoff = null;
        MaterialProperty alpha = null;
        MaterialProperty bumpMap = null;
        MaterialProperty roughness = null;
        MaterialProperty metallic = null;
        MaterialProperty reflection = null;
        MaterialProperty occlusion = null;
        MaterialProperty emissionColor = null;
        MaterialProperty AnisoMap = null;
        MaterialProperty TangentDir = null;
        MaterialProperty AnisoSpecColor1 = null;
        MaterialProperty AnisoGloss1 = null;
        MaterialProperty AnisoSpec1 = null;
        MaterialProperty TangentShift1 = null;
        MaterialProperty AnisoSpecColor2 = null;
        MaterialProperty AnisoGloss2 = null;
        MaterialProperty AnisoSpec2 = null;
        MaterialProperty TangentShift2 = null;
        MaterialProperty dissolveTexScale = null;
        MaterialProperty dissolveValue = null;
        MaterialProperty dissolveRampType = null;
        //MaterialProperty dissolveNoiseType = null;
        MaterialProperty showTangent = null;
        MaterialProperty lodQualityLow = null;

        BaseShaderGUI.WorkflowMode m_WorkflowMode = BaseShaderGUI.WorkflowMode.Metallic;
        MaterialEditor m_MaterialEditor = null;
        bool m_FirstTimeApply = true;

        private bool bShowTangent;

        private bool init = false;
        private void Init()
        {
            bShowTangent = showTangent.floatValue > 0.5f;
        }

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
            roughness = FindProperty("_Roughness", props);
            metallic = FindProperty("_Metallic", props);
            reflection = FindProperty("_Reflection", props);
            occlusion = FindProperty("_Occlusion", props);
            emissionColor = FindProperty("_EmissionColor", props);
            AnisoMap = FindProperty("_AnisoMap", props);
            TangentDir = FindProperty("_TangentDir", props);
            AnisoSpecColor1 = FindProperty("_AnisoSpecColor1", props);
            AnisoGloss1 = FindProperty("_AnisoGloss1", props);
            AnisoSpec1 = FindProperty("_AnisoSpec1", props);
            TangentShift1 = FindProperty("_TangentShift1", props);
            AnisoSpecColor2 = FindProperty("_AnisoSpecColor2", props);
            AnisoGloss2 = FindProperty("_AnisoGloss2", props);
            AnisoSpec2 = FindProperty("_AnisoSpec2", props);
            TangentShift2 = FindProperty("_TangentShift2", props);
            dissolveTexScale = FindProperty("_DissolveNoiseTexScale", props);
            dissolveValue = FindProperty("_DissolveValue", props);
            dissolveRampType = FindProperty("_DissolveRampType", props);
            //dissolveNoiseType = FindProperty("_DissolveNoiseType", props);
            showTangent = FindProperty("_ShowTangent", props);
            lodQualityLow = FindProperty("_LodQualityLow", props);
        }

        /// <summary>
        /// 显示
        /// </summary>
        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props)
        {
            FindProperties(props);

            if (!init)
            {
                Init();
                init = true;
            }

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
                EditorGUI.BeginChangeCheck();
                BaseShaderGUI.DoCommonValueArea(m_MaterialEditor,m_WorkflowMode, roughness, metallic, reflection, occlusion);
                m_MaterialEditor.ShaderProperty(TangentDir, TangentDirText.text);
                m_MaterialEditor.TexturePropertySingleLine(AnisoMapText, AnisoMap);
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("各项异性1", EditorStyles.boldLabel);
                m_MaterialEditor.ShaderProperty(AnisoSpecColor1, AnisoSpecColor1Text.text);
                m_MaterialEditor.ShaderProperty(AnisoGloss1, AnisoGloss1Text.text);
                m_MaterialEditor.ShaderProperty(AnisoSpec1, AnisoSpec1Text.text);
                m_MaterialEditor.ShaderProperty(TangentShift1, TangentShift1Text.text);
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("各项异性2", EditorStyles.boldLabel);
                m_MaterialEditor.ShaderProperty(AnisoSpecColor2, AnisoSpecColor2Text.text);
                m_MaterialEditor.ShaderProperty(AnisoGloss2, AnisoGloss2Text.text);
                m_MaterialEditor.ShaderProperty(AnisoSpec2, AnisoSpec2Text.text);
                m_MaterialEditor.ShaderProperty(TangentShift2, TangentShift2Text.text);
                EditorGUILayout.Space();
                BaseShaderGUI.DoDissolveArea(m_MaterialEditor, dissolveValue, dissolveTexScale, dissolveRampType/*, dissolveNoiseType*/);
                EditorGUILayout.Space();
                BaseShaderGUI.DoEmissionArea(m_MaterialEditor, emissionColor);

                EditorGUILayout.Space();
                bShowTangent = EditorGUILayout.Toggle(ShowTangentText.text, bShowTangent);

                m_MaterialEditor.ShaderProperty(lodQualityLow, BaseShaderGUI.LodQualityLowText.text);
            }

            if (EditorGUI.EndChangeCheck() || bModeUpdate)
            {
                if (bShowTangent)
                {
                    showTangent.floatValue = 1;
                    material.EnableKeyword("_SHOW_WORLD_TANGENT");
                }
                else
                {
                    showTangent.floatValue = 0;
                    material.DisableKeyword("_SHOW_WORLD_TANGENT");
                }

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
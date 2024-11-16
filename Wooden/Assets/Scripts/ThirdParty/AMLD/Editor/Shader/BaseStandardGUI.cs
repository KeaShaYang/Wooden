/*
作者：张阳
说明：标准光照Shader界面
日期：2019-11-18
*/

using UnityEngine;

namespace UnityEditor
{
    public class BaseStandardGUI : ShaderGUI
    {
        public static GUIContent SpecStrengthText = EditorGUIUtility.TrTextContent("直接高光强度");
        public static GUIContent IndirectSpecStrengthText = EditorGUIUtility.TrTextContent("环境高光强度");
        public static GUIContent ReflectionGlossText = EditorGUIUtility.TrTextContent("反射清晰度");
        public static GUIContent FogFactorText = EditorGUIUtility.TrTextContent("雾效强度");
        public static GUIContent EmissionUseAlphaText = EditorGUIUtility.TrTextContent("(烘焙专用)颜色贴图A通道作为自发光遮罩");
        public static GUIContent BaseMaskMapText = EditorGUIUtility.TrTextContent("混合贴图(R 粗糙度 G 金属度 B AO遮罩)");

        MaterialProperty blendMode = null;
        MaterialProperty albedoMap = null;
        MaterialProperty albedoColor = null;
        MaterialProperty alphaCutoff = null;
        MaterialProperty alpha = null;
        MaterialProperty bumpMap = null;
        MaterialProperty maskMap = null;
        MaterialProperty normalDetail = null;
        MaterialProperty detailNormalTiling = null;
        MaterialProperty detailNormalMap = null;
        MaterialProperty roughness = null;
        MaterialProperty metallic = null;
        MaterialProperty reflection = null;
        MaterialProperty reflectionGloss = null;
        MaterialProperty occlusion = null;
        MaterialProperty specStrength = null;
        MaterialProperty indirectSpecStrength = null;
        MaterialProperty effectStream = null;
        MaterialProperty streamFactor = null;
        MaterialProperty fogFactor = null;

        MaterialProperty streamColorFactor = null;
        MaterialProperty streamTexFactor = null;
        MaterialProperty streamOffsetX = null;
        MaterialProperty streamOffsetY = null;
        MaterialProperty streamTex = null;

        MaterialProperty effectIce = null;
        MaterialProperty iceOpacity = null;
        MaterialProperty iceFresnelWidth = null;
        MaterialProperty iceFresnelStrength = null;
        MaterialProperty iceReflectColor = null;
        MaterialProperty iceReflectionStrength = null;
        MaterialProperty iceRefractionTex = null;
        MaterialProperty iceMaskTexUvScale = null;
        MaterialProperty iceMaskTex = null;

        MaterialProperty emissionColor = null;
        MaterialProperty emissionMap = null;
        MaterialProperty cullMode = null;
        MaterialProperty emissionUseAlpha = null;
        MaterialProperty lodQualityLow = null;
        MaterialProperty enablefogPixel = null;

        BaseShaderGUI.WorkflowMode m_WorkflowMode = BaseShaderGUI.WorkflowMode.Metallic;
        MaterialEditor m_MaterialEditor = null;
        bool m_FirstTimeApply = true;

        private bool bEmissionUseAlpha = false;
        private bool bEmissionEnable = true;



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
            //normalDetail = FindProperty("_NormalDetail", props);
            //detailNormalMap = FindProperty("_DetailNormalMap", props);
            //detailNormalTiling = FindProperty("_DetailNormalTiling", props);
            roughness = FindProperty("_Roughness", props);
            metallic = FindProperty("_Metallic", props);
            reflection = FindProperty("_Reflection", props);
            reflectionGloss = FindProperty("_ReflectionGloss", props);
            occlusion = FindProperty("_Occlusion", props);
            specStrength = FindProperty("_SpecStrength", props);
            indirectSpecStrength = FindProperty("_IndirectSpecStrength", props);
            effectStream = FindProperty("_EffectStream", props);
            streamFactor = FindProperty("_StreamFactor", props);
            streamColorFactor = FindProperty("_StreamColorFactor", props);
            streamTexFactor = FindProperty("_StreamTexFactor", props);
            streamOffsetX = FindProperty("_StreamOffsetX", props);
            streamOffsetY = FindProperty("_StreamOffsetY", props);
            streamTex = FindProperty("_StreamTex", props);
            effectIce = FindProperty("_EffectIce", props);
            iceOpacity = FindProperty("_IceOpacity", props);
            fogFactor = FindProperty("_FogFactor", props);

            iceFresnelWidth = FindProperty("_IceFresnelWidth", props);
            iceFresnelStrength = FindProperty("_IceFresnelStrength", props);
            iceReflectColor = FindProperty("_IceReflectColor", props);
            iceReflectionStrength = FindProperty("_IceReflectionStrength", props);
            iceRefractionTex = FindProperty("_IceRefractionTex", props);
            iceMaskTexUvScale = FindProperty("_IceMaskTexUvScale", props);
            iceMaskTex = FindProperty("_IceMaskTex", props);
            emissionColor = FindProperty("_EmissionColor", props);
            emissionMap = FindProperty("_EmissionMap", props);
            cullMode = FindProperty("_Cull", props);
            emissionUseAlpha = FindProperty("_EmissionUseAlpha", props);

            lodQualityLow = FindProperty("_LodQualityLow", props);
            enablefogPixel = FindProperty("_FogPixel", props);
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
                bEmissionUseAlpha = emissionUseAlpha.floatValue > 0.5f;
                m_FirstTimeApply = false;
            }

            bool bModeUpdate = false;
            EditorGUI.BeginChangeCheck();
            {
                bModeUpdate = BaseShaderGUI.BlendModePopup(m_MaterialEditor, blendMode);
                BaseShaderGUI.DoAlbedoArea(m_MaterialEditor, albedoColor, albedoMap, material, alphaCutoff, alpha);
                BaseShaderGUI.DoNormalArea(m_MaterialEditor, bumpMap);
                BaseShaderGUI.DoMaskArea(m_MaterialEditor, m_WorkflowMode, maskMap,null, roughness, metallic, reflection, occlusion, BaseMaskMapText);
                m_MaterialEditor.ShaderProperty(reflectionGloss, ReflectionGlossText.text);
                m_MaterialEditor.ShaderProperty(specStrength, SpecStrengthText.text);
                m_MaterialEditor.ShaderProperty(indirectSpecStrength, IndirectSpecStrengthText.text);
                m_MaterialEditor.ShaderProperty(fogFactor, FogFactorText.text);
                //BaseShaderGUI.DoNormalDetailArea(m_MaterialEditor, normalDetail, detailNormalTiling, detailNormalMap);
                BaseShaderGUI.DoStreamArea(m_MaterialEditor, effectStream, streamFactor,streamColorFactor, streamTexFactor, streamOffsetX, streamOffsetY, streamTex);
                BaseShaderGUI.DoIceArea(m_MaterialEditor, effectIce, iceOpacity, iceFresnelWidth, iceFresnelStrength, iceReflectColor, 
                    iceReflectionStrength, iceRefractionTex, iceMaskTexUvScale, iceMaskTex);
                bool emEnable = BaseShaderGUI.DoEmissionArea(m_MaterialEditor, emissionColor, emissionMap);
                if(emEnable)
                {
                    if(bEmissionEnable!= emEnable)
                    {
                        bEmissionEnable = emEnable;
                        if (material != null)
                        {
                            bEmissionUseAlpha = true;
                            material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.BakedEmissive;
                        }
                    }
                    else
                    {
                        bEmissionUseAlpha = EditorGUILayout.Toggle(EmissionUseAlphaText.text, bEmissionUseAlpha);
                    }
                    EditorGUILayout.HelpBox("勾选该选项, 将颜色图A通道作为自发光遮罩, 可以用自发光来模拟流光烘焙， 需注意开启后 流光颜色 = 原流光颜色+自发光颜色, 若不需要显示自发光颜色, 可在烘焙后关闭自发光选项", MessageType.Info);
                }
                else
                {
                    bEmissionUseAlpha = false;
                    bEmissionEnable = false;
                }
                EditorGUI.BeginChangeCheck();
                m_MaterialEditor.TextureScaleOffsetProperty(albedoMap);
                if (EditorGUI.EndChangeCheck())
                {
                    bumpMap.textureScaleAndOffset = albedoMap.textureScaleAndOffset;
                    maskMap.textureScaleAndOffset = albedoMap.textureScaleAndOffset; 
                }
                BaseShaderGUI.CullModeToggle(m_MaterialEditor, cullMode);

                m_MaterialEditor.ShaderProperty(lodQualityLow, BaseShaderGUI.LodQualityLowText.text);

                EditorGUILayout.Space();
                m_MaterialEditor.ShaderProperty(enablefogPixel, BaseShaderGUI.enablefogPixelText.text);
                if(enablefogPixel.floatValue > 0.5f)
                {
                    EditorGUILayout.HelpBox("像素雾消耗极高，如非必要，请谨慎开启。可考虑能否减低雾强度代替开启像素雾。", MessageType.Warning);
                }
            }

            if (EditorGUI.EndChangeCheck() || bModeUpdate)
            {
                if (bEmissionUseAlpha)
                {
                    emissionUseAlpha.floatValue = 1;
                }
                else
                {
                    emissionUseAlpha.floatValue = 0;
                }

                foreach (var obj in blendMode.targets)
                {
                    BaseShaderGUI.MaterialChanged((Material)obj, 0,bModeUpdate);
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
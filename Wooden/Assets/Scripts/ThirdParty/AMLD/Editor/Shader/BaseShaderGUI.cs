/*
作者：张阳
说明：Shader界面通用函数
日期：2019-11-18
*/

using System;
using UnityEngine;

namespace UnityEditor
{
    public class BaseShaderGUI
    {
        /// <summary>
        /// 工作流类型
        /// </summary>
        public enum WorkflowMode
        {
            /// <summary>
            /// 高光
            /// </summary>
            Specular,
            /// <summary>
            /// 金属
            /// </summary>
            Metallic,
        }

        /// <summary>
        /// 混合模式
        /// </summary>
        public enum BlendMode
        {
            Opaque,
            Cutout,
            Transparent
        }


        public static GUIContent ColorText = EditorGUIUtility.TrTextContent("混合颜色");
        public static GUIContent MainTexText = EditorGUIUtility.TrTextContent("颜色贴图");
        public static GUIContent BumpMapText = EditorGUIUtility.TrTextContent("法线贴图");
        public static GUIContent SpecularColorText = EditorGUIUtility.TrTextContent("高光颜色");
        public static GUIContent RoughnessText = EditorGUIUtility.TrTextContent("粗糙强度");
        public static GUIContent MetallicText = EditorGUIUtility.TrTextContent("金属强度");
        public static GUIContent ReflectionText = EditorGUIUtility.TrTextContent("反光强度");
        public static GUIContent OcclusionText = EditorGUIUtility.TrTextContent("AO强度");
        public static GUIContent GlossinessText = EditorGUIUtility.TrTextContent("粗糙强度"); //字面是光泽度，但和粗糙度一样的处理，美术可以把它当成粗糙度看
        public static GUIContent MetallicMaskMapText = EditorGUIUtility.TrTextContent("混合贴图(R 粗糙度 G 金属度 B AO遮罩)");
        public static GUIContent SpecularMaskMapText = EditorGUIUtility.TrTextContent("混合贴图(RGB高光颜色 A粗糙度)");  //字面是光泽度，但和粗糙度一样的处理，美术可以把它当成粗糙度看
        public static GUIContent CutoffText = EditorGUIUtility.TrTextContent("透明裁剪");
        public static GUIContent AlphaText = EditorGUIUtility.TrTextContent("透明度");
        public static GUIContent EmissionColorText = EditorGUIUtility.TrTextContent("自发光颜色");
        public static GUIContent dissolveTexScaleText = EditorGUIUtility.TrTextContent("消融贴图缩放");
        public static GUIContent dissolveValueText = EditorGUIUtility.TrTextContent("消融值");
        public static GUIContent dissolveRampTypeText = EditorGUIUtility.TrTextContent("消融颜色类型(0为默认的第1种)");
        //public static GUIContent dissolveNoiseTypeText = EditorGUIUtility.TrTextContent("消融效果类型");
        public static GUIContent flashValueText = EditorGUIUtility.TrTextContent("闪白值");
        public static GUIContent flashColorText = EditorGUIUtility.TrTextContent("闪白颜色");
        public static GUIContent flashWidthText = EditorGUIUtility.TrTextContent("闪白宽度");
        public static GUIContent useStreamText = EditorGUIUtility.TrTextContent("开启流光");
        public static GUIContent streamFactorText = EditorGUIUtility.TrTextContent("流光强度");

        public static GUIContent streamColorFactorText = EditorGUIUtility.TrTextContent("流光颜色");
        public static GUIContent streamTexFactorText = EditorGUIUtility.TrTextContent("流动饱和度");
        public static GUIContent streamOffsetXText = EditorGUIUtility.TrTextContent("流光速度U");
        public static GUIContent streamOffsetYText = EditorGUIUtility.TrTextContent("流光速度V");
        public static GUIContent streamTexText = EditorGUIUtility.TrTextContent("流光贴图");
        public static GUIContent effectIceText = EditorGUIUtility.TrTextContent("开启冰冻");
        public static GUIContent iceOpacityText = EditorGUIUtility.TrTextContent("冰不透明度");
        public static GUIContent iceFresnelWidthText = EditorGUIUtility.TrTextContent("冰菲涅尔宽度");
        public static GUIContent iceFresnelStrengthText = EditorGUIUtility.TrTextContent("冰菲涅尔强度");
        public static GUIContent iceReflectColorText = EditorGUIUtility.TrTextContent("冰反射颜色");
        public static GUIContent iceReflectionStrengthText = EditorGUIUtility.TrTextContent("冰反射强度");
        public static GUIContent iceRefractionTexText = EditorGUIUtility.TrTextContent("冰反射贴图");
        public static GUIContent iceMaskTexUvScaleText = EditorGUIUtility.TrTextContent("冰遮罩贴图Uv缩放");
        public static GUIContent iceMaskTexText = EditorGUIUtility.TrTextContent("冰遮罩贴图(R 反射遮罩 G 透明度遮罩)");

        public static GUIContent normalDetailText = EditorGUIUtility.TrTextContent("开启细节法线");
        public static GUIContent detailNormalTilingText = EditorGUIUtility.TrTextContent("细节法线平铺");
        public static GUIContent detailNormalMapText = EditorGUIUtility.TrTextContent("细节法线");

        public static GUIContent renderQueueText = EditorGUIUtility.TrTextContent("渲染队列");
        public static GUIContent LodQualityLowText = EditorGUIUtility.TrTextContent("LOD低画质");
        public static GUIContent enablefogPixelText = EditorGUIUtility.TrTextContent("开启像素雾");

        public static string primaryText = "主属性";
        public static string OtherText = "其他";
        public static string renderingMode = "混合模式";
        public static string cullModeText = "双面绘制";

        public static readonly string[] blendNames = Enum.GetNames(typeof(BlendMode));

        public static void CullModeToggle(MaterialEditor md, MaterialProperty cullMode)
        {
            EditorGUI.showMixedValue = cullMode.hasMixedValue;
            var mode = cullMode.floatValue;
            bool cull = (mode < 1);
            EditorGUI.BeginChangeCheck();
            cull = EditorGUILayout.Toggle(cullModeText, cull);
            if (EditorGUI.EndChangeCheck())
            {
                cullMode.floatValue = cull?0:2;
            }
            EditorGUI.showMixedValue = false;
        }

        /// <summary>
        /// 渲染模式
        /// </summary>
        public static bool BlendModePopup(MaterialEditor md,MaterialProperty blendMode, string[] showNames = null)
        {
            bool bUpdate = false;
            EditorGUI.showMixedValue = blendMode.hasMixedValue;
            var mode = (BlendMode)blendMode.floatValue;
            EditorGUI.BeginChangeCheck();
            if(showNames != null)
            {
                mode = (BlendMode)EditorGUILayout.Popup(renderingMode, (int)mode, showNames);
            }
            else
            {
                mode = (BlendMode)EditorGUILayout.Popup(renderingMode, (int)mode,blendNames);
            }
            if (EditorGUI.EndChangeCheck())
            {
                md.RegisterPropertyChangeUndo("Rendering Mode");
                blendMode.floatValue = (float)mode;
                bUpdate = true;
            }
            EditorGUI.showMixedValue = false;
            return bUpdate;
        }

        /// <summary>
        /// 颜色
        /// </summary>
        public static void DoAlbedoArea(MaterialEditor md, MaterialProperty albedoColor, MaterialProperty albedoMap, Material material, MaterialProperty alphaCutoff, MaterialProperty alpha,GUIContent MainTitle = null)
        {
            if(MainTitle!=null)
            {
                md.TexturePropertySingleLine(MainTitle, albedoMap, albedoColor);
            }
            else
            {
                md.TexturePropertySingleLine(MainTexText, albedoMap, albedoColor);
            }
            DoBlendArea(md, material, alphaCutoff, alpha);
        }

        /// <summary>
        /// 透明
        /// </summary>
        public static void DoBlendArea(MaterialEditor md, Material material, MaterialProperty alphaCutoff, MaterialProperty alpha)
        {
            if (material.HasProperty("_Mode"))
            {
                if (((BlendMode)material.GetFloat("_Mode") == BlendMode.Cutout))
                {
                    if (alphaCutoff != null)
                    {
                        md.ShaderProperty(alphaCutoff, CutoffText.text);
                    }
                }
                else if ((BlendMode)material.GetFloat("_Mode") == BlendMode.Transparent)
                {
                    if (alphaCutoff != null)
                    {
                        md.ShaderProperty(alphaCutoff, CutoffText.text);
                    }
                    if (alpha != null)
                    {
                        md.ShaderProperty(alpha, AlphaText.text);
                    }
                }
            }
        }

        /// <summary>
        /// 法线
        /// </summary>
        public static void DoNormalArea(MaterialEditor md, MaterialProperty bumpMap,GUIContent Title = null)
        {
            if(Title!=null)
            {
                md.TexturePropertySingleLine(Title, bumpMap);
            }
            else
            {
                md.TexturePropertySingleLine(BumpMapText, bumpMap);
            }
        }

        /// <summary>
        /// 自发光
        /// </summary>
        public static bool DoEmissionArea(MaterialEditor md, MaterialProperty emissionColor, MaterialProperty emissionMap = null)
        {
            if(emissionMap==null)
            {
                ColorField(md, EmissionColorText.text, emissionColor, true);
            }
            else
            {
                if (md.EmissionEnabledProperty())
                {
                    bool hadEmissionTexture = emissionMap.textureValue != null;
                    md.TexturePropertyWithHDRColor(EmissionColorText, emissionMap, emissionColor, false);
                    float brightness = emissionColor.colorValue.maxColorComponent;
                    if (emissionMap.textureValue != null && !hadEmissionTexture && brightness <= 0f)
                    {
                        emissionColor.colorValue = Color.white;
                    }
                    md.LightmapEmissionFlagsProperty(MaterialEditor.kMiniTextureFieldLabelIndentLevel, true);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 闪白
        /// </summary>
        public static void DoFlashArea(MaterialEditor md, MaterialProperty flashValue, MaterialProperty flashColor, MaterialProperty flashWidth)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("闪白", EditorStyles.boldLabel);
            md.ShaderProperty(flashValue, flashValueText.text);
            md.ShaderProperty(flashColor, flashColorText.text);
            md.ShaderProperty(flashWidth, flashWidthText.text);
        }

        /// <summary>
        /// 消融
        /// </summary>
        public static void DoDissolveArea(MaterialEditor md, MaterialProperty dissolveValue, MaterialProperty dissolveTexScale, MaterialProperty dissolveRampType/*, MaterialProperty dissolveNoiseType*/)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("溶解", EditorStyles.boldLabel);
            md.ShaderProperty(dissolveTexScale, dissolveTexScaleText.text);
            md.ShaderProperty(dissolveValue, dissolveValueText.text);

            int maxRampType = 7;  //目前限制最大8种
            dissolveRampType.floatValue = EditorGUILayout.IntSlider(dissolveRampTypeText.text, Mathf.FloorToInt(dissolveRampType.floatValue), 0, maxRampType);

            int maxNoiseType = 2;  //目前限制最大3种 对应RGB通道
            //dissolveNoiseType.floatValue = EditorGUILayout.IntSlider(dissolveNoiseTypeText.text, Mathf.FloorToInt(dissolveNoiseType.floatValue), 0, maxNoiseType);
        }

        /// <summary>
        /// 流光
        /// </summary>
        public static void DoStreamArea(MaterialEditor md, MaterialProperty useStream, MaterialProperty streamFactor,MaterialProperty streamColorFactor,MaterialProperty streamTexFactor, 
            MaterialProperty streamOffsetX, MaterialProperty streamOffsetY, MaterialProperty streamTex)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("流光", EditorStyles.boldLabel);
            md.ShaderProperty(useStream, useStreamText.text);
            if(useStream.floatValue>0)
            {
                EditorGUILayout.LabelField("颜色贴图A通道控制流光局部强弱（值越低流光越强）");
                md.ShaderProperty(streamFactor, streamFactorText.text);
                md.ShaderProperty(streamTexFactor, streamTexFactorText.text);
                md.ShaderProperty(streamColorFactor, streamColorFactorText.text);
                md.ShaderProperty(streamOffsetX, streamOffsetXText.text);
                md.ShaderProperty(streamOffsetY, streamOffsetYText.text);
                md.TexturePropertySingleLine(streamTexText, streamTex);
            }
        }

        /// <summary>
        /// 细节法线
        /// </summary>
        public static void DoNormalDetailArea(MaterialEditor md, MaterialProperty useDetailNormal, MaterialProperty detailNormalTiling, MaterialProperty detailNormalMap)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("细节法线", EditorStyles.boldLabel);
            md.ShaderProperty(useDetailNormal, normalDetailText.text);
            if (useDetailNormal.floatValue > 0)
            {
                md.ShaderProperty(detailNormalTiling, detailNormalTilingText.text);
                md.TexturePropertySingleLine(detailNormalMapText, detailNormalMap);
            }
        }

        /// <summary>
        /// 冰冻
        /// </summary>
        public static void DoIceArea(MaterialEditor md, MaterialProperty effectIce, MaterialProperty iceOpacity, MaterialProperty iceFresnelWidth, 
            MaterialProperty iceFresnelStrength, MaterialProperty iceReflectColor, MaterialProperty iceReflectionStrength, MaterialProperty iceRefractionTex, 
            MaterialProperty iceMaskTexUvScale, MaterialProperty iceMaskTex)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("冰冻", EditorStyles.boldLabel);
            md.ShaderProperty(effectIce, effectIceText.text);
            if (effectIce.floatValue > 0)
            {
                EditorGUILayout.LabelField("颜色贴图A通道控制冰冻局部强弱（值越低冰冻越强）");
                md.ShaderProperty(iceOpacity, iceOpacityText.text);
                md.ShaderProperty(iceFresnelWidth, iceFresnelWidthText.text);
                md.ShaderProperty(iceFresnelStrength, iceFresnelStrengthText.text);
                md.ShaderProperty(iceReflectColor, iceReflectColorText.text);
                md.ShaderProperty(iceReflectionStrength, iceReflectionStrengthText.text);
                md.TexturePropertySingleLine(iceRefractionTexText, iceRefractionTex);
                md.ShaderProperty(iceMaskTexUvScale, iceMaskTexUvScaleText.text);
                md.TexturePropertySingleLine(iceMaskTexText, iceMaskTex);
            }
        }

        /// <summary>
        /// 颜色显示
        /// </summary>
        public static void ColorField(MaterialEditor md, string Title, MaterialProperty colorProperty,bool bHDR)
        {
            Rect rectForSingleLine = GetRectSingleLine();
            Rect leftRect = new Rect(rectForSingleLine.x, rectForSingleLine.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
            Rect RightRect = new Rect(rectForSingleLine.x + EditorGUIUtility.labelWidth, rectForSingleLine.y, EditorGUIUtility.fieldWidth, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(leftRect, Title);
            EditorGUI.BeginChangeCheck();
            Color color = EditorGUI.ColorField(RightRect, GUIContent.none, colorProperty.colorValue, true, false, bHDR);
            if (EditorGUI.EndChangeCheck())
            {
                colorProperty.colorValue = color;
            }
        }

        /// <summary>
        /// Vector显示
        /// </summary>
        public static void VectorField(MaterialEditor md, string Title, MaterialProperty vectorProperty)
        {
            Rect rectForSingleLine = GetRectSingleLine();
            Rect leftRect = new Rect(rectForSingleLine.x, rectForSingleLine.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
            Rect RightRect = new Rect(rectForSingleLine.x + EditorGUIUtility.labelWidth, rectForSingleLine.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(leftRect, Title);
            EditorGUI.BeginChangeCheck();
            Vector4 vec = EditorGUI.Vector4Field(RightRect, GUIContent.none, vectorProperty.vectorValue);
            if (EditorGUI.EndChangeCheck())
            {
                vectorProperty.vectorValue = vec;
            }
        }

        /// <summary>
        /// int显示
        /// </summary>
        public static int IntField(MaterialEditor md, string Title, int iVal)
        {
            Rect rectForSingleLine = GetRectSingleLine();
            Rect leftRect = new Rect(rectForSingleLine.x, rectForSingleLine.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
            Rect RightRect = new Rect(rectForSingleLine.x + EditorGUIUtility.labelWidth, rectForSingleLine.y, 50, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(leftRect, Title);
            EditorGUI.BeginChangeCheck();
            int val = EditorGUI.IntField(RightRect, GUIContent.none, iVal);
            if (EditorGUI.EndChangeCheck())
            {
                return val;
            }
            return -1;
        }

        /// <summary>
        /// 贴图显示
        /// </summary>
        public static void TextureField(MaterialEditor md, string Title, int TitleLen, MaterialProperty colorProperty, bool bHDR)
        {
            Rect rectForSingleLine = GetRectSingleLine();
            Rect leftRect = new Rect(rectForSingleLine.x, rectForSingleLine.y, TitleLen, EditorGUIUtility.singleLineHeight);
            Rect RightRect = new Rect(TitleLen, rectForSingleLine.y, EditorGUIUtility.fieldWidth, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(leftRect, Title);
            EditorGUI.BeginChangeCheck();
            Color color = EditorGUI.ColorField(RightRect, GUIContent.none, colorProperty.colorValue, true, false, bHDR);
            if (EditorGUI.EndChangeCheck())
            {
                colorProperty.colorValue = color;
            }
        }

        /// <summary>
        /// 单行区域
        /// </summary>
        public static Rect GetRectSingleLine()
        {
            return EditorGUILayout.GetControlRect(true, 18f, EditorStyles.layerMaskField);
        }

        /// <summary>
        /// 混合贴图
        /// </summary>
        public static void DoMaskArea(MaterialEditor md,WorkflowMode wm, MaterialProperty maskMap, MaterialProperty SpecularColor, MaterialProperty Roughness, MaterialProperty Metallic, MaterialProperty Reflection, MaterialProperty Occlusion, GUIContent Title = null)
        {
            if (wm == WorkflowMode.Specular)
            {
                md.ShaderProperty(Roughness, GlossinessText.text);
            }
            else if (wm == WorkflowMode.Metallic)
            {
                md.ShaderProperty(Roughness, RoughnessText.text);
            }
            if (Metallic != null)
            {
                md.ShaderProperty(Metallic, MetallicText.text);
            }
            if (Occlusion != null)
            {
                md.ShaderProperty(Occlusion, OcclusionText.text);
            }
            if (wm == WorkflowMode.Specular)
            {
                md.TexturePropertySingleLine(Title == null ? SpecularMaskMapText : Title, maskMap);
            }
            else if (wm == WorkflowMode.Metallic)
            {
                md.TexturePropertySingleLine(Title == null ? MetallicMaskMapText : Title, maskMap);
            }
            if (Reflection != null)
            {
                md.ShaderProperty(Reflection, ReflectionText.text);
            }
            if (SpecularColor != null)
            {
                ColorField(md, SpecularColorText.text, SpecularColor, false);
            }
        }

        /// <summary>
        /// 通用值
        /// </summary>
        public static void DoCommonValueArea(MaterialEditor md, WorkflowMode wm, MaterialProperty Roughness, MaterialProperty Metallic, MaterialProperty Reflection, MaterialProperty Occlusion, GUIContent Title = null)
        {
            if (wm == WorkflowMode.Specular)
            {
                md.ShaderProperty(Roughness, GlossinessText.text);
                md.ShaderProperty(Metallic, MetallicText.text);
                if(Occlusion!=null)
                {
                    md.ShaderProperty(Occlusion, OcclusionText.text);
                }
                if (Reflection != null)
                {
                    md.ShaderProperty(Reflection, ReflectionText.text);
                }
            }
            else if (wm == WorkflowMode.Metallic)
            {
                md.ShaderProperty(Roughness, RoughnessText.text);
                md.ShaderProperty(Metallic, MetallicText.text);
                if (Occlusion != null)
                {
                    md.ShaderProperty(Occlusion, OcclusionText.text);
                }
                if (Reflection != null)
                {
                    md.ShaderProperty(Reflection, ReflectionText.text);
                }
            }
        }

        /// <summary>
        /// 结尾
        /// </summary>
        public static void DoEnd(MaterialEditor md, bool queueField = false)
        {
            EditorGUILayout.Space();
            if (queueField)
            {
                md.RenderQueueField();
            }
            md.EnableInstancingField();
            md.DoubleSidedGIField();
        }

        /// <summary>
        /// 设置材质混合模式
        /// </summary>
        public static void SetMaterialBlendMode(Material material, BlendMode blendMode,int AlphaZWrite = 0, bool RenderQueue = true, string OpaqueTag = "", string CutoutTag = "TransparentCutout", string TransparentTag = "Transparent")
        {
            switch (blendMode)
            {
                case BlendMode.Opaque:
                    material.SetOverrideTag("RenderType", OpaqueTag);
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    material.SetInt("_ZWrite", 1);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.DisableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    if(RenderQueue)
                    {
                        material.renderQueue = -1;
                    }
                    break;
                case BlendMode.Cutout:
                    material.SetOverrideTag("RenderType", CutoutTag);
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    material.SetInt("_ZWrite", 1);
                    material.EnableKeyword("_ALPHATEST_ON");
                    material.DisableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    if (RenderQueue)
                    {
                        material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.AlphaTest;
                    }
                    break;
                case BlendMode.Transparent:
                    material.SetOverrideTag("RenderType", TransparentTag);
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    material.SetInt("_ZWrite", AlphaZWrite);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.EnableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    if (RenderQueue)
                    {
                        material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                    }
                    break;
            }
        }

        /// <summary>
        /// 设置材质混合模式
        /// </summary>
        public static void SetBaseMaterialBlendMode(Material material, BlendMode blendMode, bool RenderQueue = true,bool bZWrite = true)
        {
            switch (blendMode)
            {
                case BlendMode.Opaque:
                    material.SetOverrideTag("RenderType", "");
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    if(bZWrite)
                    {
                        material.SetInt("_ZWrite", 1);
                    }
                    material.DisableKeyword("_ALPHATEST_ON");
                    if (RenderQueue)
                    {
                        material.renderQueue = -1;
                    }
                    break;
                case BlendMode.Cutout:
                    material.SetOverrideTag("RenderType", "TransparentCutout");
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    if (bZWrite)
                    {
                        material.SetInt("_ZWrite", 1);
                    }
                    material.EnableKeyword("_ALPHATEST_ON");
                    if (RenderQueue)
                    {
                        material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.AlphaTest;
                    }
                    break;
                case BlendMode.Transparent:
                    material.SetOverrideTag("RenderType", "Transparent");
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    if (bZWrite)
                    {
                        material.SetInt("_ZWrite", 0);
                    }
                    material.DisableKeyword("_ALPHATEST_ON");
                    if (RenderQueue)
                    {
                        material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                    }
                    break;
            }
        }

        /// <summary>
        /// 材质变更
        /// </summary>
        public static void MaterialChanged(Material material, int AlphaZWrite = 0, bool RenderQueue = true, bool Emissive = true, string OpaqueTag = "", string CutoutTag = "TransparentCutout", string TransparentTag = "Transparent")
        {
            if (material.HasProperty("_Mode"))
            {
                SetMaterialBlendMode(material, (BlendMode)material.GetFloat("_Mode"), AlphaZWrite, RenderQueue, OpaqueTag, CutoutTag, TransparentTag);
            }
            if(Emissive)
            {
                MaterialEditor.FixupEmissiveFlag(material);
                bool shouldEmissionBeEnabled = (material.globalIlluminationFlags & MaterialGlobalIlluminationFlags.EmissiveIsBlack) == 0;
                SetKeyword(material, "_EMISSION", shouldEmissionBeEnabled);
            }
        }

        /// <summary>
        /// 更换新shader
        /// </summary>
        public static void AssignNewShaderToMaterial(Material material, Shader oldShader, Shader newShader)
        {
            if (material.HasProperty("_Emission"))
            {
                material.SetColor("_EmissionColor", material.GetColor("_Emission"));
            }

            if (oldShader == null || !oldShader.name.Contains("Legacy Shaders/"))
            {
                if(newShader.name == "AMLD/scene/scene_standard_leaves")
                {
                    SetMaterialBlendMode(material, BlendMode.Cutout);
                }
                if (newShader.name == "AMLD/scene/scene_standard_terrain")
                {
                    SetMaterialBlendMode(material, BlendMode.Opaque);
                }
                else
                {
                    SetMaterialBlendMode(material, (BlendMode)material.GetFloat("_Mode"));
                }
                return;
            }
            if (newShader.name != "AMLD/scene/scene_standard_leaves")
            {
                BlendMode blendMode = BlendMode.Opaque;
                if (oldShader.name.Contains("/Transparent/Cutout/"))
                {
                    blendMode = BaseShaderGUI.BlendMode.Cutout;
                }
                else if (oldShader.name.Contains("/Transparent/"))
                {
                    blendMode = BaseShaderGUI.BlendMode.Transparent;
                }
                material.SetFloat("_Mode", (float)blendMode);
            }
        }

        /// <summary>
        /// 设置Keyword
        /// </summary>
        public static void SetKeyword(Material m, string keyword, bool state)
        {
            if (state)
            {
                m.EnableKeyword(keyword);
            }
            else
            {
                m.DisableKeyword(keyword);
            }
        }
    }
}
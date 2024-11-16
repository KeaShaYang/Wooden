/*
作者：黄云龙
说明：角色眼睛
日期：2020-01-02
*/

using UnityEngine;

namespace UnityEditor
{
    public class RoleEyeGUI : ShaderGUI
    {
        public static GUIContent albedoMapText = EditorGUIUtility.TrTextContent("颜色贴图");
        public static GUIContent scleraColorText = EditorGUIUtility.TrTextContent("巩膜颜色");
        public static GUIContent irisColorText = EditorGUIUtility.TrTextContent("虹膜颜色");
        public static GUIContent irisSizeText = EditorGUIUtility.TrTextContent("虹膜大小");
        public static GUIContent bumpMapText = EditorGUIUtility.TrTextContent("角膜法线贴图");
        public static GUIContent maskMapText = EditorGUIUtility.TrTextContent("遮罩贴图（R 虹膜遮罩 G 高光遮罩 B AO遮罩）");
        public static GUIContent specSizeText = EditorGUIUtility.TrTextContent("高光大小");
        public static GUIContent scleraSpecText = EditorGUIUtility.TrTextContent("巩膜高光强度");

        MaterialProperty blendMode = null;
        MaterialProperty albedoMap = null;
        MaterialProperty alphaCutoff = null;
        MaterialProperty alpha = null;
        MaterialProperty specColor = null;
        MaterialProperty albedoColor = null;
        MaterialProperty scleraColor = null;
        MaterialProperty irisColor = null;
        MaterialProperty irisSize = null;
        MaterialProperty bumpMap = null;
        MaterialProperty maskMap = null;
        MaterialProperty glossiness = null;
        MaterialProperty reflection = null;
        MaterialProperty occlusion = null;
        MaterialProperty specSize = null;
        MaterialProperty scleraSpec = null;

        BaseShaderGUI.WorkflowMode m_WorkflowMode = BaseShaderGUI.WorkflowMode.Specular;
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
            specColor = FindProperty("_SpecColor", props);
            scleraColor = FindProperty("_scleraColor", props);
            irisColor = FindProperty("_irisColor", props);
            irisSize = FindProperty("_irisSize", props);
            bumpMap = FindProperty("_BumpMap", props);
            maskMap = FindProperty("_MaskMap", props);
            glossiness = FindProperty("_Roughness", props);
            reflection = FindProperty("_Reflection", props);
            occlusion = FindProperty("_Occlusion", props);
            specSize = FindProperty("_SpecSize", props);
            scleraSpec = FindProperty("_ScleraSpec", props);
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
                BaseShaderGUI.MaterialChanged(material,1, false, false);
                m_FirstTimeApply = false;
            }
            bool bModeUpdate = false;
            EditorGUI.BeginChangeCheck();
            {
                bModeUpdate = BaseShaderGUI.BlendModePopup(m_MaterialEditor, blendMode);
                BaseShaderGUI.DoAlbedoArea(m_MaterialEditor, albedoColor, albedoMap, material, alphaCutoff, alpha);
                BaseShaderGUI.DoNormalArea(m_MaterialEditor, bumpMap, bumpMapText);
                BaseShaderGUI.DoMaskArea(m_MaterialEditor, m_WorkflowMode, maskMap, specColor, glossiness, null, reflection, occlusion, maskMapText);

                EditorGUILayout.Space();
                m_MaterialEditor.ShaderProperty(scleraColor, scleraColorText);
                m_MaterialEditor.ShaderProperty(irisColor, irisColorText);

                EditorGUILayout.Space();
                m_MaterialEditor.ShaderProperty(irisSize, irisSizeText);
                m_MaterialEditor.ShaderProperty(specSize, specSizeText);
                m_MaterialEditor.ShaderProperty(scleraSpec, scleraSpecText);
            }

            if (EditorGUI.EndChangeCheck() || bModeUpdate)
            {
                BaseShaderGUI.MaterialChanged(material,1, bModeUpdate ,false, "", "TransparentCutout", "Transparent");
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
			BaseShaderGUI.MaterialChanged(material, 1, false, false);
        }
    }
}
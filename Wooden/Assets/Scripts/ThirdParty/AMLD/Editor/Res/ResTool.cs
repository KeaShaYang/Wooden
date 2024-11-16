/*
作者：张阳
说明：资源工具
日期：2019-08-22
*/

using System;
using System.Collections.Generic;
using System.IO;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;

namespace AMLD.Effect.Editor
{
    public class ResTool
    {
        /// <summary>
        /// 设置选中贴图格式
        /// </summary>
        [MenuItem("AMLD/资源工具/设置选中贴图格式")]
        static void SetTextureFromat()
        {
            Object[] vObj = Selection.GetFiltered(typeof(Texture), SelectionMode.DeepAssets);
            if (vObj == null || vObj.Length == 0)
            {
                EditorUtility.DisplayDialog("提示", "没有选中任何物体", "确定");
            }
            else
            {
                for (int i = 0; i < vObj.Length; i++)
                {
                    Texture tex = (Texture)vObj[i];
                    ChangeTexProperty(tex, 10000, false, true, true, false);
                }
                AssetDatabase.Refresh();
                AssetDatabase.SaveAssets();
            }
        }

        /// <summary>
        /// 更换模型属性
        /// </summary>
        static void ChangeModelProperty(string strPath, ModelImporterNormals mt, ModelImporterTangents tt)
        {
            ModelImporter mi = AssetImporter.GetAtPath(strPath) as ModelImporter;
            mi.importNormals = mt;
            mi.importTangents = tt;
        }

        /// <summary>
        /// 更换贴图属性
        /// </summary>
        static void ChangeTexProperty(Texture tex, int iMaxSize, bool bReadable, bool bCompressed, bool bMipMap, bool bSetMax, bool bAlphaIsTransparency = false)
        {
            string path = AssetDatabase.GetAssetPath(tex);
            TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
            textureImporter.textureType = TextureImporterType.Default;
            if (bSetMax)
            {
                textureImporter.maxTextureSize = iMaxSize;
            }
            else
            {
                if (textureImporter.maxTextureSize > iMaxSize)
                {
                    textureImporter.maxTextureSize = iMaxSize;
                }
            }
            if (bCompressed)
            {
                textureImporter.textureCompression = TextureImporterCompression.Compressed;
            }
            else
            {
                textureImporter.textureCompression = TextureImporterCompression.Uncompressed;
            }
            textureImporter.isReadable = bReadable;
            textureImporter.mipmapEnabled = bMipMap;
            textureImporter.alphaIsTransparency = bAlphaIsTransparency;
            textureImporter.anisoLevel = 0;
            textureImporter.ClearPlatformTextureSettings("Standalone");
            textureImporter.ClearPlatformTextureSettings("iPhone");
            textureImporter.ClearPlatformTextureSettings("Android");
            AssetDatabase.ImportAsset(path);
        }

        /// <summary>
        /// 移除材质中废弃属性
        /// </summary>
        [MenuItem("AMLD/资源工具/移除材质中废弃属性")]
        public static void ClearMatObseleteProperty()
        {
            Object[] vObj = Selection.GetFiltered(typeof(Material), SelectionMode.DeepAssets);
            if (vObj == null || vObj.Length == 0)
            {
                EditorUtility.DisplayDialog("提示", "没有选中任何材质和材质文件夹", "确定");
            }
            else
            {
                if (EditorUtility.DisplayDialog("提示", "是否确认移除材质中无用属性！", "确认", "取消"))
                {
                    string Dir = UtilTool.GetGUID();
                    string strDir = Application.dataPath.Replace("/Assets", "/");
                    string strPath = strDir + "MaterialBackup/" + Dir + "/";
                    for (int i = 0; i < vObj.Length; i++)
                    {
                        string sp = AssetDatabase.GetAssetPath(vObj[i]);
                        string strFrom = strDir + sp;
                        string strTo = strPath + sp;
                        UtilTool.CopyFile(strFrom, strTo);
                    }
                    for (int i = 0; i < vObj.Length; i++)
                    {
                        Material mat = (Material)vObj[i];
                        SerializedObject so = new SerializedObject(mat);
                        SerializedProperty m_SavedProperties = so.FindProperty("m_SavedProperties");
                        RemoveElement(mat, "m_TexEnvs", m_SavedProperties);
                        RemoveElement(mat, "m_Floats", m_SavedProperties);
                        RemoveElement(mat, "m_Colors", m_SavedProperties);
                        so.ApplyModifiedProperties();
                    }
                    AssetDatabase.Refresh();
                    AssetDatabase.SaveAssets();
                    UtilTool.ShowTips(0, "废弃属性移除完毕，备份材质保存在" + strPath + "目录里");
                }
            }
        }

        private static void RemoveElement(Material mat, string spName, SerializedProperty saveProperty)
        {
            SerializedProperty property = saveProperty.FindPropertyRelative(spName);
            for (int i = property.arraySize - 1; i >= 0; i--)
            {
                var prop = property.GetArrayElementAtIndex(i);
                string propertyName = prop.displayName;
                if (!mat.HasProperty(propertyName))
                {
                    property.DeleteArrayElementAtIndex(i);
                    Debug.Log("材质：" + mat.name + " 移除属性名称：" + propertyName);
                }
            }
        }

        [MenuItem("AMLD/资源工具/TPSheet数据转PNG")]
        public static void TPSheetToPng()
        {
            Object[] objs = Selection.objects;
            for (int j = 0; j < objs.Length; j++)
            {
                Object obj = objs[j];
                string sourcePath = AssetDatabase.GetAssetPath(obj);
                if (sourcePath.EndsWith(".tpsheet"))
                {
                    sourcePath = sourcePath.Replace("Assets", Application.dataPath);
                    string[] text = File.ReadAllLines(sourcePath);
                    float width = 0;
                    float height = 0;
                    List<string> data = new List<string>();
                    for (int i = 0; i < text.Length; i++)
                    {
                        if (text[i].StartsWith(":size="))
                        {
                            string temp = text[i].Replace(":size=", "");
                            string[] temp2 = temp.Split('x');
                            width = float.Parse(temp2[0]);
                            height = float.Parse(temp2[1]);
                        }
                        if (text[i].StartsWith("tex_"))
                        {
                            data.Add(text[i].Replace(" ","").Replace(" ",""));
                        }
                    }
                    if (data.Count>0 && width > 0 && height > 0)
                    {
                        List<Color> colordata = new List<Color>();
                        data.Sort(((s, s1) => { return s.CompareTo(s1); } ));
                        for (int i = 0; i < data.Count; i++)
                        {
                            string[] temp = data[i].Split(';');
                            int leftx = int.Parse(temp[1]);
                            int lefty = int.Parse(temp[2]);
                            int iconwidth = int.Parse(temp[3]);
                            int iconheight = int.Parse(temp[4]);
                            float centeroffsetx = float.Parse(temp[5]);
                            float centeroffsety = float.Parse(temp[6]);
                            float fullcenterx = (leftx + iconwidth * centeroffsetx) / width;
                            float fullcentery = (lefty + iconheight * centeroffsety) / height;
                            float fullleftx = leftx / width;
                            float fulllefty = lefty / height;
                            float fullrightx = (leftx + iconwidth) / width;
                            float fullrighty = (lefty + iconheight) / height;
                            
                            Vector2 enc1 = fullcenterx * new Vector2(1.0f, 255.0f);
                            enc1 = math.frac(enc1);
                            enc1.x -= enc1.y * 1.0f / 255.0f;
                            Vector2 enc2 = fullcentery * new Vector2(1.0f, 255.0f);
                            enc2 = math.frac(enc2);
                            enc2.x -= enc2.y * 1.0f / 255.0f;
                            Color color1 = new Color(enc1.x,enc1.y,enc2.x,enc2.y);

                            enc1 = fullleftx * new Vector2(1.0f, 255.0f);
                            enc1 = math.frac(enc1);
                            enc1.x -= enc1.y * 1.0f / 255.0f;
                            enc2 = fulllefty * new Vector2(1.0f, 255.0f);
                            enc2 = math.frac(enc2);
                            enc2.x -= enc2.y * 1.0f / 255.0f;
                            Color color2 = new Color(enc1.x,enc1.y,enc2.x,enc2.y);

                            if (Math.Abs(fullrightx - 1) < math.FLT_MIN_NORMAL)
                            {
                                enc1 = new Vector2(1, 0);
                            }
                            else
                            {
                                enc1 = fullrightx * new Vector2(1.0f, 255.0f);
                                enc1 = math.frac(enc1);
                                enc1.x -= enc1.y * 1.0f / 255.0f;
                            }
                            if (Math.Abs(fullrighty - 1) < math.FLT_MIN_NORMAL)
                            {
                                enc2 = new Vector2(1, 0);
                            }
                            else
                            {
                                enc2 = fullrighty * new Vector2(1.0f, 255.0f);
                                enc2 = math.frac(enc2);
                                enc2.x -= enc2.y * 1.0f / 255.0f;
                            }
                            Color color3 = new Color(enc1.x,enc1.y,enc2.x,enc2.y);
                            colordata.Add(color1);
                            colordata.Add(color2);
                            colordata.Add(color3);
                        }
                        int pixellength = Mathf.CeilToInt(Mathf.Sqrt(colordata.Count));
                        if (colordata.Count < pixellength * pixellength)
                        {
                            for (int i = colordata.Count; i < pixellength * pixellength; i++)
                            {
                                colordata.Add(Color.clear);
                            }
                        }
                        Texture2D newTex = new Texture2D(pixellength, pixellength,TextureFormat.RGBA32,false,true);
                        newTex.SetPixels(colordata.ToArray());
                        newTex.Apply();
                        byte[] bytes = newTex.EncodeToPNG();
                        File.WriteAllBytes(sourcePath.Replace(".tpsheet","_uv.png"), bytes);
                    }
                }
            }
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("", "导出完毕", "ok");
        }

        private static float pixelMeshRatio = 61f / 1024f; 
        [MenuItem("AMLD/资源工具/TPSheet数据转网格")]
        public static void TPSheetToMesh()
        {
            Object[] objs = Selection.objects;
            for (int j = 0; j < objs.Length; j++)
            {
                Object obj = objs[j];
                string sourcePath = AssetDatabase.GetAssetPath(obj);
                if (sourcePath.EndsWith(".tpsheet"))
                {
                    string fullPath = sourcePath.Replace("Assets", Application.dataPath);
                    string[] text = File.ReadAllLines(fullPath);
                    float width = 0;
                    float height = 0;
                    List<string> data = new List<string>();
                    for (int i = 0; i < text.Length; i++)
                    {
                        if (text[i].StartsWith(":size="))
                        {
                            string temp = text[i].Replace(":size=", "");
                            string[] temp2 = temp.Split('x');
                            width = float.Parse(temp2[0]);
                            height = float.Parse(temp2[1]);
                        }
                        if (text[i].StartsWith("tex_"))
                        {
                            data.Add(text[i].Replace(" ","").Replace(" ",""));
                        }
                    }
                    if (data.Count>0 && width > 0 && height > 0)
                    {
                        for (int i = 0; i < data.Count; i++)
                        {
                            string[] temp = data[i].Split(';');
                            string texName = temp[0];
                            int leftx = int.Parse(temp[1]);
                            int lefty = int.Parse(temp[2]);
                            int iconwidth = int.Parse(temp[3]);
                            int iconheight = int.Parse(temp[4]);
                            float centeroffsetx = float.Parse(temp[5]);
                            float centeroffsety = float.Parse(temp[6]);
                            
                            Mesh mesh = new Mesh();
                            float meshWidth = iconwidth * pixelMeshRatio;
                            float meshHeight = iconheight * pixelMeshRatio;
                            Vector3 lbVertexPos = new Vector3(-meshWidth * 0.5f, -meshHeight * 0.5f, 0);
                            lbVertexPos.x += (0.5f - centeroffsetx) * meshWidth;
                            lbVertexPos.y += (0.5f - centeroffsety) * meshHeight;
                            Vector3[] vertices = new Vector3[4];
                            vertices[0] = lbVertexPos;
                            vertices[1] = lbVertexPos + new Vector3(0, meshHeight, 0);
                            vertices[2] = lbVertexPos + new Vector3(meshWidth, meshHeight, 0);
                            vertices[3] = lbVertexPos + new Vector3(meshWidth, 0, 0);
                            mesh.SetVertices(vertices);
                            mesh.SetIndices(new []{0, 1, 2, 0, 2, 3}, MeshTopology.Triangles, 0);
                            float uvWidth = iconwidth / width;
                            float uvHeight = iconheight / height;
                            Vector2 lbVertexUv = new Vector2(leftx / width, lefty / height);
                            Vector2[] uvs = new Vector2[4];
                            uvs[0] = lbVertexUv;
                            uvs[1] = lbVertexUv + new Vector2(0, uvHeight);
                            uvs[2] = lbVertexUv + new Vector2(uvWidth, uvHeight);
                            uvs[3] = lbVertexUv + new Vector2(uvWidth, 0);
                            mesh.SetUVs(0, uvs);
                            AssetDatabase.CreateAsset(mesh, sourcePath.Substring(0, 
                                sourcePath.IndexOf("/tex", StringComparison.Ordinal)) + 
                                "/mod/" + texName.Replace("tex_", "mod_") + ".asset");
                        }
                    }
                }
            }
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("", "导出完毕", "ok");
        }
        
        [MenuItem("AMLD/资源工具/创建SLG部队材质")]
        static void Process5()
        {
            int index = 0;
            string[] strs = AssetDatabase.FindAssets("t:Texture", new []{"Assets/Art/Model/SLG/GodRelic/army/tex"});
            foreach (var str in strs)
            {
                string path = AssetDatabase.GUIDToAssetPath(str);
                if (path.Contains("_uv.png")) continue;
                var tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                var uv_tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path.Replace(".png", "_uv.png"));
                if (tex && uv_tex)
                {
                    Material mat = new Material(Shader.Find("AMLD/other/uv_offset_alpha_frames"));
                    mat.SetTexture("_MainTex", tex);
                    mat.SetTexture("_UvTex", uv_tex);
                    mat.SetFloat("_Speed", 12);
                    mat.renderQueue = 3100 + index++;
                    AssetDatabase.CreateAsset(mat, "Assets/Art/Model/SLG/GodRelic/army/mat/mat_" + tex.name + ".mat");
                }
            }
            
            EditorUtility.DisplayDialog("", "创建完成", "ok");
        }

        private static string[] buduiActions = { "attack0", "attack2", "attack4", "attack6", "idle4", 
            "march0", "march1", "march2", "march3", "march4", "march5", "march6", "march7", "retreat0", 
            "retreat1", "retreat2", "retreat3", "retreat4", "retreat5", "retreat6", "retreat7" };
        [MenuItem("AMLD/资源工具/创建SLG部队网格")]
        static void Process6()
        {
            int len = buduiActions.Length;
            for (int j = 0; j < 2; j++)
            {
                for (int i = 0; i < len; i++)
                {
                    Mesh mesh = new Mesh();
                    Vector3[] vertices = new Vector3[4];
                    vertices[0] = new Vector3(-0.5f, -0.5f, 0);
                    vertices[1] = new Vector3(-0.5f, 0.5f, 0);
                    vertices[2] = new Vector3(0.5f, 0.5f, 0);
                    vertices[3] = new Vector3(0.5f, -0.5f, 0);
                    mesh.SetVertices(vertices);
                    mesh.SetIndices(new []{0, 1, 2, 0, 2, 3}, MeshTopology.Triangles, 0);
                    Vector2[] uvs = new Vector2[4];
                    uvs[0] = new Vector2(0, 0);
                    uvs[1] = new Vector2(0, 1);
                    uvs[2] = new Vector2(1, 1);
                    uvs[3] = new Vector2(1, 0);
                    mesh.SetUVs(0, uvs);
                    Vector2[] uvs2 = new Vector2[4];
                    uvs2[0] = new Vector2(i * 8 + len * 8 * j, (i + 1) * 8 + len * 8 * j);
                    uvs2[1] = uvs2[0];
                    uvs2[2] = uvs2[0];
                    uvs2[3] = uvs2[0];
                    mesh.SetUVs(1, uvs2);
                    AssetDatabase.CreateAsset(mesh, "Assets/Art/Model/SLG/GodRelic/army/mod/mod_budui" + (j + 1) + "_" + buduiActions[i] + ".asset");
                }
            }
            
            EditorUtility.DisplayDialog("", "创建完成", "ok");
        }

        private static Vector3[][] buduiPos =
        {
            new []{
                new Vector3(0, 0, 3.5f), new Vector3(-1.5f, 0, 0), new Vector3(1.5f, 0, 0),
                new Vector3(-2.5f, 0, -4.5f), new Vector3(0, 0, -4.5f), new Vector3(2.5f, 0, -4.5f)
            }, 
            new []{
                new Vector3(0, 0, 3.3f), new Vector3(-1.5f, 0, 0), new Vector3(1.5f, 0, 0),
                new Vector3(-3f, 0, -3), new Vector3(0, 0, -3), new Vector3(3f, 0, -3)
            }, 
            new []{
                new Vector3(0, 0, 3.3f), new Vector3(-1.5f, 0, 0), new Vector3(1.5f, 0, 0),
                new Vector3(-3f, 0, -3), new Vector3(0, 0, -3), new Vector3(3f, 0, -3)
            }, 
            new []{
                new Vector3(0, 0, 5f), new Vector3(-1.5f, 0, 0), new Vector3(1.5f, 0, 0),
                new Vector3(-2.5f, 0, -4f), new Vector3(0, 0, -4f), new Vector3(2.5f, 0, -4f)
            }, 
            new []{
                new Vector3(0, 0, 6f), new Vector3(-1.5f, 0, 0), new Vector3(1.5f, 0, 0),
                new Vector3(-2.5f, 0, -4), new Vector3(0, 0, -4), new Vector3(2.5f, 0, -4)
            }, 
            new []{
                new Vector3(0, 0, 5f), new Vector3(-1.5f, 0, 0), new Vector3(1.5f, 0, 0),
                new Vector3(-2.5f, 0, -4f), new Vector3(0, 0, -4f), new Vector3(2.5f, 0, -4f)
            }, 
            new []{
                new Vector3(0, 0, 3.3f), new Vector3(-1.5f, 0, 0), new Vector3(1.5f, 0, 0),
                new Vector3(-3f, 0, -3), new Vector3(0, 0, -3), new Vector3(3f, 0, -3)
            }, 
            new []{
                new Vector3(0, 0, 3.3f), new Vector3(-1.5f, 0, 0), new Vector3(1.5f, 0, 0),
                new Vector3(-3f, 0, -3), new Vector3(0, 0, -3), new Vector3(3f, 0, -3)
            }, 
        };
        private static string[] buduiPrefabActions = { "attack_00", "attack_02", "attack_04", "attack_06", "idle_04", 
            "march_00", "march_01", "march_02", "march_03", "march_04", "march_05", "march_06", "march_07", "retreat_00", 
            "retreat_01", "retreat_02", "retreat_03", "retreat_04", "retreat_05", "retreat_06", "retreat_07" };
        private static int buduiCount = 4;
        [MenuItem("AMLD/资源工具/创建SLG部队预制体")]
        static void Process7()
        {
            for (int i = 1; i <= buduiCount; i++)
            {
                GameObject go = new GameObject("God_Relic_troops_" + i);
                Transform root = go.transform;
                foreach (var actionName in buduiPrefabActions)
                {
                    GameObject actionRootGo = new GameObject(actionName);
                    Transform actionRootTran = actionRootGo.transform; 
                    actionRootTran.SetParent(root, false);
                    int dir = int.Parse(actionName.Substring(actionName.Length - 1));
                    
                    GameObject shogun = new GameObject("shogun", typeof(MeshFilter), typeof(MeshRenderer));
                    Transform shogunTran = shogun.transform;
                    MeshFilter shogunMeshFilter = shogun.GetComponent<MeshFilter>();
                    MeshRenderer shogunMeshRenderer = shogun.GetComponent<MeshRenderer>();
                    shogunTran.SetParent(actionRootTran, false);
                    shogunTran.localPosition = Quaternion.AngleAxis(45 * (dir + 1), new Vector3(0, 1, 0)) * buduiPos[dir][0];
                    shogunTran.localEulerAngles = new Vector3(30, 45, 0);
                    shogunTran.localScale = new Vector3(16f, 16f, 16f);
                    shogunMeshFilter.sharedMesh = AssetDatabase.LoadAssetAtPath<Mesh>("Assets/Art/Model/SLG/GodRelic/army/mod/mod_budui1_" + 
                        actionName.Substring(0, actionName.Length - 3) + dir + ".asset");
                    shogunMeshRenderer.sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/Model/SLG/GodRelic/army/mat/mat_budui0" + i + ".mat");
                    shogunMeshRenderer.shadowCastingMode = ShadowCastingMode.Off;
                    shogunMeshRenderer.receiveShadows = false;
                    shogunMeshRenderer.lightProbeUsage = LightProbeUsage.Off;
                    shogunMeshRenderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
                    shogunMeshRenderer.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
                    shogunMeshRenderer.allowOcclusionWhenDynamic = false;

                    int length = buduiPos[dir].Length;
                    for (int j = 1; j < length; j++)
                    {
                        GameObject solider = new GameObject("solider" + j, typeof(MeshFilter), typeof(MeshRenderer));
                        Transform soliderTran = solider.transform;
                        MeshFilter soliderMeshFilter = solider.GetComponent<MeshFilter>();
                        MeshRenderer soliderMeshRenderer = solider.GetComponent<MeshRenderer>();
                        soliderTran.SetParent(actionRootTran, false);
                        soliderTran.localPosition = Quaternion.AngleAxis(45 * (dir + 1), new Vector3(0, 1, 0)) * buduiPos[dir][j];
                        soliderTran.localEulerAngles = new Vector3(30, 45, 0);
                        soliderTran.localScale = new Vector3(16f, 16f, 16f);
                        soliderMeshFilter.sharedMesh = AssetDatabase.LoadAssetAtPath<Mesh>("Assets/Art/Model/SLG/GodRelic/army/mod/mod_budui2_" + 
                            actionName.Substring(0, actionName.Length - 3) + dir + ".asset");
                        soliderMeshRenderer.sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/Model/SLG/GodRelic/army/mat/mat_budui0" + i + ".mat");
                        soliderMeshRenderer.shadowCastingMode = ShadowCastingMode.Off;
                        soliderMeshRenderer.receiveShadows = false;
                        soliderMeshRenderer.lightProbeUsage = LightProbeUsage.Off;
                        soliderMeshRenderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
                        soliderMeshRenderer.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
                        soliderMeshRenderer.allowOcclusionWhenDynamic = false;
                    }
                    
                    actionRootGo.SetActive(false);
                }
                
                for (int j = 1; j <= buduiCount; j++)
                {
                    GameObject fight = new GameObject("fight_0" + j);
                    Transform fightTran = fight.transform;
                    fightTran.SetParent(root, false);
                    for (int k = 0; k < 2; k++)
                    {
                        string actionName = k == 0 ? "attack_02" : "attack_06";
                        GameObject actionRootGo = new GameObject(actionName);
                        Transform actionRootTran = actionRootGo.transform; 
                        actionRootTran.SetParent(fightTran, false);
                        actionRootTran.localPosition = k == 0 ? new Vector3(-4, 0, 4) : new Vector3(4, 0, -4);
                        int dir = int.Parse(actionName.Substring(actionName.Length - 1));
                        
                        GameObject shogun = new GameObject("shogun", typeof(MeshFilter), typeof(MeshRenderer));
                        Transform shogunTran = shogun.transform;
                        MeshFilter shogunMeshFilter = shogun.GetComponent<MeshFilter>();
                        MeshRenderer shogunMeshRenderer = shogun.GetComponent<MeshRenderer>();
                        shogunTran.SetParent(actionRootTran, false);
                        shogunTran.localPosition = Quaternion.AngleAxis(45 * (dir + 1), new Vector3(0, 1, 0)) * buduiPos[dir][0];
                        shogunTran.localEulerAngles = new Vector3(30, 45, 0);
                        shogunTran.localScale = new Vector3(16f, 16f, 16f);
                        shogunMeshFilter.sharedMesh = AssetDatabase.LoadAssetAtPath<Mesh>("Assets/Art/Model/SLG/GodRelic/army/mod/mod_budui1_" + 
                            actionName.Substring(0, actionName.Length - 3) + dir + ".asset");
                        shogunMeshRenderer.sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>(k == 0 ? 
                            "Assets/Art/Model/SLG/GodRelic/army/mat/mat_budui0" + i + ".mat" : 
                            "Assets/Art/Model/SLG/GodRelic/army/mat/mat_budui0" + j + ".mat");
                        shogunMeshRenderer.shadowCastingMode = ShadowCastingMode.Off;
                        shogunMeshRenderer.receiveShadows = false;
                        shogunMeshRenderer.lightProbeUsage = LightProbeUsage.Off;
                        shogunMeshRenderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
                        shogunMeshRenderer.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
                        shogunMeshRenderer.allowOcclusionWhenDynamic = false;

                        int length = buduiPos[dir].Length;
                        for (int h = 1; h < length; h++)
                        {
                            GameObject solider = new GameObject("solider" + h, typeof(MeshFilter), typeof(MeshRenderer));
                            Transform soliderTran = solider.transform;
                            MeshFilter soliderMeshFilter = solider.GetComponent<MeshFilter>();
                            MeshRenderer soliderMeshRenderer = solider.GetComponent<MeshRenderer>();
                            soliderTran.SetParent(actionRootTran, false);
                            soliderTran.localPosition = Quaternion.AngleAxis(45 * (dir + 1), new Vector3(0, 1, 0)) * buduiPos[dir][h];
                            soliderTran.localEulerAngles = new Vector3(30, 45, 0);
                            soliderTran.localScale = new Vector3(16f, 16f, 16f);
                            soliderMeshFilter.sharedMesh = AssetDatabase.LoadAssetAtPath<Mesh>("Assets/Art/Model/SLG/GodRelic/army/mod/mod_budui2_" + 
                                actionName.Substring(0, actionName.Length - 3) + dir + ".asset");
                            soliderMeshRenderer.sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>(k == 0 ? 
                                "Assets/Art/Model/SLG/GodRelic/army/mat/mat_budui0" + i + ".mat" : 
                                "Assets/Art/Model/SLG/GodRelic/army/mat/mat_budui0" + j + ".mat");
                            soliderMeshRenderer.shadowCastingMode = ShadowCastingMode.Off;
                            soliderMeshRenderer.receiveShadows = false;
                            soliderMeshRenderer.lightProbeUsage = LightProbeUsage.Off;
                            soliderMeshRenderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
                            soliderMeshRenderer.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
                            soliderMeshRenderer.allowOcclusionWhenDynamic = false;
                        }
                    }
                    fight.SetActive(false);
                }

                PrefabUtility.SaveAsPrefabAsset(go, "Assets/Resources/Model/SLG/" + "God_Relic_troops_" + i + ".prefab");
            }
            
            EditorUtility.DisplayDialog("", "创建完成", "ok");
        }
    }
}
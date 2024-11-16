using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class UISantuSyncWindow : EditorWindow
{
    private HashSet<string> selectedSantuFolders = new HashSet<string>();

    // 需要处理的散图文件夹前缀
    public readonly static HashSet<string> needProcessSantuFolder = new HashSet<string>
    {
        "bag", "skills", "head", "headframe", "headsmall", "shenzhuangicon", "hunguicon", "buff", "hunhuanicon",
        "rongyuhuizhang", "soulwake", "douluozhiluicon", "douluowaizhuanicon", "huanicon", "npcfunctionicon"
    };
    
    private const int SANTU_COUNT_PER_AB = 100;
    private const string SANTU_AB_FOLDER_NAME = "UISantu";
    private const string SANTU_FOLDER_PATH = "Assets/Resources/UISantu";
    private const string SANTU_LIST_FOLDER_PATH = "Assets/Resources/UISantu/uisantulist";
    private const string SANTU_TABLE_PATH = "Assets/Resources/UISantu/uisantulist/uisantulist.asset";
    private static UIsantuTable UIsantuTable;
    private static UIsantuTable GetOrCreateUIsantuTable()
    {
        if (!UIsantuTable)
        {
            UIsantuTable = AssetDatabase.LoadAssetAtPath<UIsantuTable>(SANTU_TABLE_PATH);
            if (!UIsantuTable)
            {
                UIsantuTable = CreateInstance<UIsantuTable>();
            }
        }

        return UIsantuTable;
    }

    
    [MenuItem("AMLD/UI工具/散图导入工具")]
    static void ShowWin()
    {
        UISantuSyncWindow editor = (UISantuSyncWindow) GetWindow(typeof(UISantuSyncWindow), true, "散图导入工具", true);
        editor.minSize = new Vector2(400, 400);
        editor.ShowUtility();
    }

    private Vector2 scrollVec;

    private void OnGUI()
    {
        GUIStyle style = new GUIStyle(GUI.skin.GetStyle("Toggle"));
        style.fontSize = 20;
        scrollVec = GUILayout.BeginScrollView(scrollVec);
        foreach (var santuFolder in needProcessSantuFolder)
        {
            bool contains = selectedSantuFolders.Contains(santuFolder);
            if (GUILayout.Toggle(contains, santuFolder, style))
            {
                if (!contains)
                    selectedSantuFolders.Add(santuFolder);
            }
            else if (contains)
            {
                selectedSantuFolders.Remove(santuFolder);
            }
        }
        GUILayout.EndScrollView();

        GUIStyle style1 = new GUIStyle(GUI.skin.GetStyle("LargeButton"));
        style1.fontSize = 16;
        if (GUILayout.Button("全选", style1, GUILayout.Height(24)))
        {
            foreach (var santuFolder in needProcessSantuFolder)
            {
                if (!selectedSantuFolders.Contains(santuFolder))
                    selectedSantuFolders.Add(santuFolder);
            }
        }

        if (GUILayout.Button("同步", style1, GUILayout.Height(24)))
        {
            SyncSantus(selectedSantuFolders);
        }

        if (GUILayout.Button("清除Resources多余散图"))
        {
            CleanRedundantSantu();
        }
    }
    
    public static void ForceSyncAllSantu()
    {
        var santuTable = GetOrCreateUIsantuTable();
        santuTable.Clear();
        
        string[] santuResGUIDs = AssetDatabase.FindAssets("t:Texture", new[] {SANTU_FOLDER_PATH});
        string[] santuResPaths = new string[santuResGUIDs.Length];
        int index = 0;
        foreach (var santuResGUID in santuResGUIDs)
        {
            santuResPaths[index++] = AssetDatabase.GUIDToAssetPath(santuResGUID);
        }

        foreach (var santuResPath in santuResPaths)
        {
            string spName = Path.GetFileNameWithoutExtension(santuResPath);
            string abName = santuResPath.Replace(SANTU_FOLDER_PATH, SANTU_AB_FOLDER_NAME);
            abName = abName.Substring(0, abName.LastIndexOf("/", StringComparison.Ordinal));
            santuTable.AddSprite(spName, abName);
        }
                
        if (!Directory.Exists(Application.dataPath + SANTU_FOLDER_PATH.Replace("Assets", "")))
            Directory.CreateDirectory(Application.dataPath + SANTU_FOLDER_PATH.Replace("Assets", ""));
        if (!Directory.Exists(Application.dataPath + SANTU_LIST_FOLDER_PATH.Replace("Assets", "")))
            Directory.CreateDirectory(Application.dataPath + SANTU_LIST_FOLDER_PATH.Replace("Assets", ""));

        if (!AssetDatabase.Contains(santuTable))
            AssetDatabase.CreateAsset(santuTable, SANTU_TABLE_PATH);
        else
            EditorUtility.SetDirty(santuTable);
        
        SyncSantus(needProcessSantuFolder);
    }

    public static void SyncSantus(HashSet<string> folders)
    {
        int oldLimit = QualitySettings.masterTextureLimit;
        QualitySettings.masterTextureLimit = 0;
        UIsantuPostprocessor.enable = true;
        string[] paths = Directory.GetDirectories(Application.dataPath + "/Art/UI/santu");
        foreach (var selectedSantuFolder in folders)
        {
            string pattern = "/" + selectedSantuFolder + "[0-9]+";
            string end = "/" + selectedSantuFolder;
            foreach (var path in paths)
            {
                string path1 = path.Replace("\\", "/").ToLower();
                if (Regex.IsMatch(path1, pattern) || path1.EndsWith(end))
                {
                    string[] files = Directory.GetFiles(path1);
                    foreach (var file in files)
                    {
                        if (!file.EndsWith(".meta"))
                        {
                            SyncSantu(file);
                        }
                    }
                }
            }
        }
        
        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();

        UIsantuPostprocessor.enable = false;
        QualitySettings.masterTextureLimit = oldLimit;
    }
    
    static void SyncSantu(string path)
    {
        path = path.Replace("\\", "/");
        string santuFolderName = path.Substring(0, path.LastIndexOf('/'));
        santuFolderName = santuFolderName.Substring(santuFolderName.LastIndexOf('/') + 1);
        santuFolderName = GetSantuFolderName(santuFolderName);
        
        string spName = Path.GetFileNameWithoutExtension(path);

        var santuTable = GetOrCreateUIsantuTable();
        
        string abPath = santuTable.GetAbPathBySpName1(spName, santuFolderName);
        if (!string.IsNullOrEmpty(abPath) && GetSantuFolderNameByAbPath(abPath) != santuFolderName)
        {
            //出现重名散图
            abPath = "";
        }
        if (string.IsNullOrEmpty(abPath))
        {
            abPath = santuTable.GetMinSpCountAbPath(santuFolderName, out int spCount);
            if (spCount >= SANTU_COUNT_PER_AB) abPath = "";
            
            if (string.IsNullOrEmpty(abPath))
            {
                abPath = SANTU_AB_FOLDER_NAME + "/" + santuFolderName;
                int maxIndex = santuTable.GetMaxAbPathIndex(abPath);
                abPath = abPath + (maxIndex + 1);
            }
            santuTable.AddSprite(spName, abPath);
        }
        
        if (!Directory.Exists(Application.dataPath + SANTU_FOLDER_PATH.Replace("Assets", "")))
            Directory.CreateDirectory(Application.dataPath + SANTU_FOLDER_PATH.Replace("Assets", ""));
        if (!Directory.Exists(Application.dataPath + SANTU_LIST_FOLDER_PATH.Replace("Assets", "")))
            Directory.CreateDirectory(Application.dataPath + SANTU_LIST_FOLDER_PATH.Replace("Assets", ""));

        if (!AssetDatabase.Contains(santuTable))
            AssetDatabase.CreateAsset(santuTable, SANTU_TABLE_PATH);
        else
            EditorUtility.SetDirty(santuTable);

        string folderPath = Application.dataPath + "/Resources/" + abPath;
        string santuResPath = folderPath + "/" + spName + ".png";
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        CopySantu2Resources(path, santuResPath);
    }
    
    // 给散图边缘加四圈透明像素（手机上实测两圈还是有问题，不知道为什么插值能插值到两圈以外的区域，加到四圈，确保稳定）
    static void CopySantu2Resources(string path, string targetPath)
    {
        //需要加载外部原图，不能用unity压缩过的图
        WWW www = new WWW("file://" + path);
        Texture2D tex = www.texture;
        if (tex)
        {
            Texture2D newTex = new Texture2D(tex.width + 8, tex.height + 8, TextureFormat.RGBA32, 0, true);
            newTex.SetPixels(new Color[newTex.width * newTex.height]);
            newTex.Apply();
            Graphics.CopyTexture(tex, 0, 0, 0, 0, tex.width, tex.height, newTex, 0, 0, 4, 4);
            File.WriteAllBytes(targetPath, newTex.EncodeToPNG());
        }
    }
    
    static string GetSantuFolderName(string atlasTexName)
    {
        if (string.IsNullOrEmpty(atlasTexName)) return atlasTexName;
        if (atlasTexName == "zhongqiuhuodong2022") return atlasTexName;
        string tmpStr = atlasTexName.Substring(atlasTexName.Length - 1, 1);
        while (int.TryParse(tmpStr, out int num))
        {
            atlasTexName = atlasTexName.Substring(0, atlasTexName.Length - 1);
            tmpStr = atlasTexName.Substring(atlasTexName.Length - 1, 1);
        }
        return atlasTexName;
    }
    
    static string GetSantuFolderNameByAbPath(string abPath)
    {
        abPath = abPath.Replace(SANTU_AB_FOLDER_NAME + "/", "");
        return GetSantuFolderName(abPath);
    }

    static void CleanRedundantSantu()
    {
        var santuTable = GetOrCreateUIsantuTable();
        santuTable.Init();
        string[] GUIDs = AssetDatabase.FindAssets("t:Texture", new[] { SANTU_FOLDER_PATH });
        try
        {
            AssetDatabase.StartAssetEditing();
            foreach (var guid in GUIDs)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                string spName = Path.GetFileNameWithoutExtension(path);

                string santuFolderName = path.Substring(0, path.LastIndexOf('/'));
                santuFolderName = santuFolderName.Substring(santuFolderName.LastIndexOf('/') + 1);
                santuFolderName = GetSantuFolderName(santuFolderName);
                string atlasName = UIsantuTable.GetAtlasNameBySantuFolderName(santuFolderName);
                if (santuTable.HasSpName(spName, atlasName))
                {
                    string abPath = santuTable.GetAbPathBySpName(spName, atlasName);
                    if (path.Contains(abPath + "/" + spName))
                    {
                        continue;
                    }
                }
                
                AssetDatabase.DeleteAsset(path);
                Debug.Log("删除多余散图资源：" + path);
            }
        }
        finally
        {
            AssetDatabase.StopAssetEditing();
        }
    }
}

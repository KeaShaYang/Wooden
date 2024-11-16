using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using LitJson;
using Debug = UnityEngine.Debug;

public class UITools
{
    public static void GenerateSprites(TextureImporter assetImp, Texture2D tex,string localPath =null)
    {
        string path = assetImp.assetPath;
        int index1 = path.LastIndexOf('/');
        int index2 = path.LastIndexOf('.');
        if (index1 < 0 || index2 < 0) return;
        string atlasName = path.Substring(index1 + 1, index2 - index1 - 1);
        string basePath = localPath != null ? localPath : Application.dataPath.Replace("Assets", "");
        path = basePath + path.Substring(0, index1 + 1)+ atlasName+ ".txt";
        if (!File.Exists(path))
        {
            return;
        }
        Dictionary<string, Vector4> tIpterMap = new Dictionary<string,Vector4>();
        SaveBoreder(tIpterMap, assetImp);
        string json_txt = File.ReadAllText(path);
        JsonData json_data = JsonMapper.ToObject(json_txt);
        WriteMeta(json_data, assetImp, tex, tIpterMap);
        UGUIAtlasBorderTool.SyncAtlasBorder(assetImp);
        File.Delete(path);
        EditorUtility.SetDirty(assetImp);
        assetImp.SaveAndReimport();
        AssetDatabase.Refresh();
    }
    
    [MenuItem("Assets/Texture/GenerateSprites")]
    public static void GenerateSprites() {
        Texture2D tex = Selection.activeObject as Texture2D;
        if (tex == null) {
            return;
        }
        string texPath = AssetDatabase.GetAssetPath(tex);
        TextureImporter assetImp = AssetImporter.GetAtPath(texPath) as TextureImporter;
        if (assetImp)
        {
            GenerateSprites(assetImp, tex);
        }
    }
    
    [MenuItem("Assets/Texture/同步九宫数据")]
    public static void SyncAtlasBorder() {
        Texture2D tex = Selection.activeObject as Texture2D;
        if (tex == null) {
            return;
        }
        string texPath = AssetDatabase.GetAssetPath(tex);
        TextureImporter assetImp = AssetImporter.GetAtPath(texPath) as TextureImporter;
        if (assetImp)
        {
            UGUIAtlasBorderTool.SyncAtlasBorder(assetImp);
            EditorUtility.SetDirty(assetImp);
            assetImp.SaveAndReimport();
            AssetDatabase.Refresh();
        }
    }

    public static void GenerateSpritesByTexPath(string txtPath,string loaclPath)
    {
        Texture2D tex = (Texture2D)AssetDatabase.LoadAssetAtPath(txtPath, typeof(Texture2D));
        if (tex == null)
        {
            return;
        }

        string texPath = AssetDatabase.GetAssetPath(tex);
        TextureImporter assetImp = AssetImporter.GetAtPath(texPath) as TextureImporter;
        if (assetImp)
        {
            assetImp.SaveAndReimport();     //第一次导入新图集时，对该图集做一次reimport操作，因为Unity第一次impoirt图集会压缩失败
            GenerateSprites(assetImp, tex,loaclPath);
        }
    }
    
    //如果这张图集已经拉好了9宫格，需要先保存起来
    static void SaveBoreder(Dictionary<string, Vector4> tIpterMap, TextureImporter tIpter) {
        for (int i = 0, size = tIpter.spritesheet.Length; i < size; i++) {
            tIpterMap.Add(tIpter.spritesheet[i].name, tIpter.spritesheet[i].border);
        }
    }
    
    //写信息到SpritesSheet里
    static void WriteMeta(JsonData json_data, TextureImporter assetImp, Texture2D tex, Dictionary<string, Vector4> borders) {
        JsonData json_frames = json_data["frames"];
        SpriteMetaData[] metaData = new SpriteMetaData[json_frames.Count];
        Debugger.Log(string.Format("WriteMeta:{0}", json_frames.Count));
        int count = 0;
        foreach (var key in json_frames.Keys) {
            int i = key.LastIndexOf(".");
            string name = key.Substring(0, i);
            Rect rect = new Rect();
            var value = json_frames[key]["frame"];
            rect.width = int.Parse(value["w"].ToString());
            rect.height = int.Parse(value["h"].ToString());
            rect.x = int.Parse(value["x"].ToString());
            rect.y = tex.height - int.Parse(value["y"].ToString()) - rect.height;
            SpriteMetaData data = new SpriteMetaData();
            data.rect = rect;
            data.pivot = new Vector2(0.5f, 0.5f);
            data.name = name;
            if (borders.ContainsKey(name)) {
                data.border = borders[name];
            }
            metaData[count] = ProcessMirrorData(data);
            ++count;
        }
        assetImp.spritesheet = metaData;
        assetImp.textureType = TextureImporterType.Sprite;
        assetImp.spriteImportMode = SpriteImportMode.Multiple;
        assetImp.mipmapEnabled = false;
    }

    //处理镜像图片的边框
    static SpriteMetaData ProcessMirrorData(SpriteMetaData metaData)
    {
        if (metaData.name.Contains("_jx_"))
        {
            Rect newRect = new Rect();
            newRect.x = metaData.rect.x;
            newRect.y = metaData.rect.y;
            newRect.width = metaData.rect.width - 1;
            newRect.height = metaData.rect.height - 1;
            metaData.rect = newRect;
        }
        return metaData;
    }

    [MenuItem("AMLD/UI工具/处理镜像散图")]
    public static void ProcessMirrorTexture()
    {
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (path.StartsWith("Assets/Art/UI/santu") && path.Contains("_jx_"))
        {
            //需要加载外部原图，不能用unity压缩过的图
            WWW www = new WWW("file://" + Application.dataPath + path.Substring(6));
            Texture2D tex = www.texture;
            if (tex)
            {
                if (tex.width % 2 > 0 || tex.height % 2 > 0)
                {
                    EditorUtility.DisplayDialog("提示", "已经处理的图片不需要重复处理", "ok");
                    return;
                }
                Texture2D newTex = GetMirrorTexture(tex);
                SaveTextureFile(newTex, Application.dataPath + path.Substring(6));
                EditorUtility.DisplayDialog("提示", "处理完成", "ok");
            }
        }
        else
        {
            EditorUtility.DisplayDialog("提示", "选择图片路径或名称有误", "ok");
        }
    }

    [MenuItem("AMLD/UI工具/撤销镜像散图处理")]
    public static void UndoProcessMirrorTexture()
    {
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        //需要加载外部原图，不能用unity压缩过的图
        WWW www = new WWW("file://" + Application.dataPath + path.Substring(6));
        Texture2D tex = www.texture;
        if (tex)
        {
            if (tex.width % 2 == 0 || tex.height % 2 == 0)
            {
                EditorUtility.DisplayDialog("提示", "该图片未做过镜像处理，不需要撤销", "ok");
                return;
            }
            Texture2D newTex = UndoMirrorTexture(tex);
            SaveTextureFile(newTex, Application.dataPath + path.Substring(6));
            EditorUtility.DisplayDialog("提示", "处理完成", "ok");
        }
    }

    //获取此图片用于镜像的图片（扩展上边一行像素和右边一列像素）
    public static Texture2D GetMirrorTexture(Texture2D tex)
    {
        tex.wrapMode = TextureWrapMode.Clamp;
        tex.filterMode = FilterMode.Point;
        var renderTex = new RenderTexture(tex.width + 1, tex.height + 1, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
        renderTex.Create();
        Graphics.SetRenderTarget(renderTex);
        GL.PushMatrix();
        GL.Clear(true, true, Color.clear);
        GL.PopMatrix();
        var mat = new Material(Shader.Find("Unlit/CopyTexture"));
        mat.mainTexture = tex;
        Graphics.SetRenderTarget(renderTex);
        GL.PushMatrix();
        GL.LoadOrtho();
        mat.SetPass(0);
        GL.Begin(GL.QUADS);
        GL.TexCoord2(0, 0);
        GL.Vertex3(0, 0, 0);
        GL.TexCoord2(0, 1 + 1f / tex.height);
        GL.Vertex3(0, 1, 0);
        GL.TexCoord2(1 + 1f / tex.width, 1 + 1f / tex.height);
        GL.Vertex3(1, 1, 0);
        GL.TexCoord2(1 + 1f / tex.width, 0);
        GL.Vertex3(1, 0, 0);
        GL.End();
        GL.PopMatrix();
        var resultTex = new Texture2D(renderTex.width, renderTex.height, TextureFormat.ARGB32, false, true);
        RenderTexture.active = renderTex;
        resultTex.ReadPixels(new Rect(0, 0, resultTex.width, resultTex.height), 0, 0);
        resultTex.Apply();
        return resultTex;
    }

    public static Texture2D UndoMirrorTexture(Texture2D tex)
    {
        tex.wrapMode = TextureWrapMode.Clamp;
        tex.filterMode = FilterMode.Point;
        var renderTex = new RenderTexture(tex.width - 1, tex.height - 1, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
        renderTex.Create();
        Graphics.SetRenderTarget(renderTex);
        GL.PushMatrix();
        GL.Clear(true, true, Color.clear);
        GL.PopMatrix();
        var mat = new Material(Shader.Find("Unlit/CopyTexture"));
        mat.mainTexture = tex;
        Graphics.SetRenderTarget(renderTex);
        GL.PushMatrix();
        GL.LoadOrtho();
        mat.SetPass(0);
        GL.Begin(GL.QUADS);
        GL.TexCoord2(0, 0);
        GL.Vertex3(0, 0, 0);
        GL.TexCoord2(0, 1 - 1f / tex.height);
        GL.Vertex3(0, 1, 0);
        GL.TexCoord2(1 - 1f / tex.width, 1 - 1f / tex.height);
        GL.Vertex3(1, 1, 0);
        GL.TexCoord2(1 - 1f / tex.width, 0);
        GL.Vertex3(1, 0, 0);
        GL.End();
        GL.PopMatrix();
        var resultTex = new Texture2D(renderTex.width, renderTex.height, TextureFormat.ARGB32, false, true);
        RenderTexture.active = renderTex;
        resultTex.ReadPixels(new Rect(0, 0, resultTex.width, resultTex.height), 0, 0);
        resultTex.Apply();
        return resultTex;
    }

    //保存图片为图片文件
    public static void SaveTextureFile(Texture2D tex, string path)
    {
        using (var fs = File.OpenWrite(path))
        {
            var bytes = tex.EncodeToPNG();
            fs.Write(bytes, 0, bytes.Length);
        }
    }
    
    private static string svn_argument = "log {0} -q -l 1";

    [MenuItem("Assets/Texture/检查svn更新日期")]
    static void ProcessSvnCheckDate()
    {
        var objs = Selection.objects;
        if (objs.Length < 1) return;

        foreach (var o in objs)
        {
            ProcessSvnCheckDateSingle(o);
        }
    }

    static void ProcessSvnCheckDateSingle(Object o)
    {
        if (!o) return;
        
        string santuFolderPath, atlasPath, atlasName;
        string selectPath = Path.GetFullPath(AssetDatabase.GetAssetPath(o));
        if (selectPath.EndsWith(".png"))
        {
            if (selectPath.Contains("santu"))
            {
                santuFolderPath = Path.GetDirectoryName(selectPath);
                atlasPath = GetAtlasPathBySantuFolderPath(santuFolderPath, out atlasName);
            }
            else
            {
                atlasPath = selectPath;
                santuFolderPath = GetSantuFolderPathByAtlasPath(atlasPath, out atlasName);
            }
        }
        else
        {
            if (selectPath.Contains("santu"))
            {
                santuFolderPath = selectPath;
                atlasPath = GetAtlasPathBySantuFolderPath(santuFolderPath, out atlasName);
            }
            else
            {
                string[] GUIDs = AssetDatabase.FindAssets("t:Texture", new[] {AssetDatabase.GetAssetPath(o)});
                foreach (var guid in GUIDs)
                {
                    Object obj = AssetDatabase.LoadAssetAtPath<Object>(AssetDatabase.GUIDToAssetPath(guid));
                    ProcessSvnCheckDateSingle(obj);
                }
                return;
            }
        }
        PrintSvnInfo(atlasPath, atlasName, "图集");
        PrintSvnInfo(santuFolderPath, atlasName, "散图");
    }

    static string GetAtlasPathBySantuFolderPath(string santuFolderPath, out string atlasName)
    {
        atlasName = Path.GetFileNameWithoutExtension(santuFolderPath);
        string atlasFolder = AtlasRefCounter.GetRefAtlasName_Editor(atlasName);
        return santuFolderPath.Replace("santu", "Atlas/" + atlasFolder) + ".png";
    }
    
    static string GetSantuFolderPathByAtlasPath(string atlasPath, out string atlasName)
    {
        atlasName = Path.GetFileNameWithoutExtension(atlasPath);
        return Application.dataPath + "/Art/UI/santu/" + atlasName;
    }

    static void PrintSvnInfo(string path, string atlasName, string profix)
    {
        string exePath = PlayerPrefs.GetString("__Editor_svn_path");
        if (string.IsNullOrEmpty(exePath))
        {
            exePath = EditorUtility.OpenFilePanel("选择svn.exe", "", ".exe");
            if (string.IsNullOrEmpty(exePath)) return;
            if (!exePath.EndsWith("svn.exe"))
            {
                EditorUtility.DisplayDialog("错误", "选择的程序不是svn命令行程序", "ok");
                return;
            }
                
            PlayerPrefs.SetString("__Editor_svn_path", exePath);
        }
        
        var pInfo = new ProcessStartInfo(exePath);
        pInfo.Arguments = string.Format(svn_argument, path);
        pInfo.CreateNoWindow = true;
        pInfo.ErrorDialog = true;
        pInfo.UseShellExecute = false;
        pInfo.RedirectStandardOutput = true;

        var p = Process.Start(pInfo);
        p.StandardOutput.ReadLine();
        Debug.LogError($"{profix} {atlasName} 最近更新日志：{p.StandardOutput.ReadLine()}");
        p.WaitForExit();
        p.Close();
    }

    public static bool CheckIsPrefabRewardItem(GameObject go)
    {
        if (!go) return false;
        var count = go.transform.childCount;
        for (int i = 0; i < count; i++)
        {
            var child = go.transform.GetChild(i);
            //if (PrefabUtility.GetPrefabInstanceStatus(child) == PrefabInstanceStatus.Connected)
            {
                if (PrefabUtility.HasPrefabInstanceAnyOverrides(child.gameObject, false) && child.name.Contains("RewardItemView"))
                {
                    if (!PrefabUtility.IsAnyPrefabInstanceRoot(child.gameObject) || !PrefabUtility.IsOutermostPrefabInstanceRoot(child.gameObject))
                    {
                        return false;
                    }

                    var overridesList = PrefabUtility.GetObjectOverrides(child.gameObject);
                    foreach (var ovr in overridesList)
                    {
                        if (ovr.instanceObject != null && ovr.instanceObject.name.Contains("GodBody"))
                        {
                            return true;
                        }
                    }

                    return false;
                }
            }
            var isCheck = CheckIsPrefabRewardItem(child.gameObject);
            if (isCheck)
            {
                return true;
            }
        }
        return false;
    }

    [MenuItem("AMLD/UI工具/检查AtlasRefCounter脚本")]
    static void CheckAtlasRefCounterScripts()
    {
        string[] paths = new string[] { "Assets/Resources/UI" };
        var assets = AssetDatabase.FindAssets("t:Prefab", paths);
        foreach (var assetId in assets)
        {
            if (!string.IsNullOrEmpty(assetId))
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(assetId);
                var gameObj = (GameObject)AssetDatabase.LoadMainAssetAtPath(assetPath);
                if (gameObj != null)
                {
                    var array = gameObj.GetComponentsInChildren<AtlasRefCounter>();
                    if (array != null && array.Length > 0)
                    {
                        Debug.LogError("UI非法持有组件AtlasRefCounter：" + gameObj.name);
                        for (int i = 0; i < array.Length; i++)
                        {
                            Debug.LogError("UI非法持有组件AtlasRefCounter：" + gameObj.name + " 节点：" + array[i].name);
                        }
                    }
                }
            }
        }
    }
}

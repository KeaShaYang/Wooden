using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AtlasRefCounter : MonoBehaviour
{
    // 常驻内存的图集，有加新的常驻内存图集时这里也要同步
    public static Dictionary<string, int> atlasRefDic = new Dictionary<string, int>{ ["comm"] = 1, ["skills"] = 1, ["bag"] = 1, ["face"] = 1, ["hunhuanicon"] = 1 };

    public static bool HasAtlasRef(string atlasName)
    {
        return atlasRefDic.ContainsKey(atlasName);
    }
    
    static void AddAtlasRef_Editor(List<Image> imgList)
    {
        UnityEngine.Profiling.Profiler.BeginSample("collect atlas reference (editor only)");
        foreach (var image in imgList)
        {
            AddSingleAtlasSpriteRef_Editor(image.sprite);
        }
        UnityEngine.Profiling.Profiler.EndSample();
    }

    static void RemoveAtlasRef_Editor(List<Image> imgList)
    {
        UnityEngine.Profiling.Profiler.BeginSample("collect atlas reference (editor only)");
        foreach (var image in imgList)
        {
            RemoveSingleAtlasSpriteRef_Editor(image.sprite);
        }
        UnityEngine.Profiling.Profiler.EndSample();
    }

    public static void AddSingleAtlasSpriteRef_Editor(Sprite sprite)
    {
        if (!sprite) return;
        string atlasName = GetRefAtlasName_Editor(sprite.texture.name);
        if (!string.IsNullOrEmpty(atlasName))
        {
            atlasRefDic.TryGetValue(atlasName, out int refCount);
            refCount = refCount + 1;
            atlasRefDic[atlasName] = refCount;
        }
    }

    public static void RemoveSingleAtlasSpriteRef_Editor(Sprite sprite)
    {
        if (!sprite) return;
        string atlasName = GetRefAtlasName_Editor(sprite.texture.name);
        if (!string.IsNullOrEmpty(atlasName))
        {
            if (atlasRefDic.TryGetValue(atlasName, out int refCount))
            {
                refCount = refCount - 1;
                if (refCount == 0) atlasRefDic.Remove(atlasName);
                else atlasRefDic[atlasName] = refCount;
            }
        }
    }

    public static string GetRefAtlasName_Editor(string atlasTexName)
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
        
    private List<Image> images = new List<Image>();

    private bool hasInit;
    
    public void Init()
    {
        if (hasInit) return;
        hasInit = true;
        transform.GetComponentsInChildren(true, images);
        AddAtlasRef_Editor(images);
    }
    
    public void Release()
    {
        if (!hasInit) return;
        hasInit = false;
        RemoveAtlasRef_Editor(images);
    }
}

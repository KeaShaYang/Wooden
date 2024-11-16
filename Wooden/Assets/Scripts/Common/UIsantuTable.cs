using System;
using System.Collections.Generic;
using UnityEngine;

public class UIsantuTable : ScriptableObject
{
    [Serializable]
    struct SpriteAbPath
    {
        [SerializeField] 
        internal string spName;
        [SerializeField]
        internal string abPath;
    }

    [SerializeField]
    private List<string> spNames = new List<string>();
    [SerializeField]
    private List<string> abPaths = new List<string>();
    [SerializeField]
    private List<string> repeatSpAtlasNames = new List<string>();
    [SerializeField]
    private List<SpriteAbPath> repeatSpAbPaths = new List<SpriteAbPath>();
    
    private Dictionary<string, string> sp2abTable;
    private Dictionary<string, List<SpriteAbPath>> atlasSp2abTable;
    
#if UNITY_EDITOR
    private const string SANTU_AB_FOLDER_NAME = "UISantu";
#endif

    public void Init()
    {
        int count = spNames.Count;
        sp2abTable = new Dictionary<string, string>(count * 2);
        for (int i = 0; i < count; i++)
        {
            sp2abTable[spNames[i]] = abPaths[i];
        }

        count = repeatSpAtlasNames.Count;
        atlasSp2abTable = new Dictionary<string, List<SpriteAbPath>>(count * 2);
        for (int i = 0; i < count; i++)
        {
            if (!atlasSp2abTable.TryGetValue(repeatSpAtlasNames[i], out var list))
            {
                list = new List<SpriteAbPath>();
                atlasSp2abTable[repeatSpAtlasNames[i]] = list;
            }
            list.Add(repeatSpAbPaths[i]);
        }
    }

    public string GetAbPathBySpName(string spName, string atlasName)
    {
        if (sp2abTable.TryGetValue(spName, out string result))
        {
#if UNITY_EDITOR
            if (GetAtlasNameByAbPath(result).ToLower() != atlasName)
            {
                Debug.LogError("Sprite资源与图集名称不符，编辑器无法显示：" + spName + "," + atlasName);
                //result = null;  //确保通过这种方式拿出来的资源和图集名是对的上的，这样结果才和静态图集对得上
            }
#endif
            return result;
        }
        if (atlasSp2abTable.TryGetValue(atlasName, out var list))
        {
            foreach (var spriteAbPath in list)
            {
                if (spriteAbPath.spName == spName)
                    return spriteAbPath.abPath;
            }
        }
        return null;
    }

    public bool HasSpName(string spName, string atlasName)
    {
        if (sp2abTable.ContainsKey(spName))
            return true;
        if (atlasSp2abTable.TryGetValue(atlasName, out var list))
        {
            foreach (var spriteAbPath in list)
            {
                if (spriteAbPath.spName == spName)
                    return true;
            }
        }
        return false;
    }
    
#if UNITY_EDITOR
    public string GetAbPathBySpName1(string spName, string santuFolderName)
    {
        int count = spNames.Count;
        for (int i = 0; i < count; i++)
        {
            if (spNames[i] == spName)
                return abPaths[i];
        }

        string atlasName = GetAtlasNameBySantuFolderName(santuFolderName);
        count = repeatSpAbPaths.Count;
        for (int i = 0; i < count; i++)
        {
            if (repeatSpAbPaths[i].spName == spName && repeatSpAtlasNames[i] == atlasName)
                return repeatSpAbPaths[i].abPath;
        }

        return null;
    }
    
    public string GetMinSpCountAbPath(string santuFolderName, out int minCount)
    {
        Dictionary<string, int> tmpCountDic = new Dictionary<string, int>();
        foreach (var value in abPaths)
        {
            if (!tmpCountDic.TryGetValue(value, out int count))
                count = 0;
            tmpCountDic[value] = count + 1;
        }
        foreach (var value in repeatSpAbPaths)
        {
            if (!tmpCountDic.TryGetValue(value.abPath, out int count))
                count = 0;
            tmpCountDic[value.abPath] = count + 1;
        }

        string minPath = "";
        minCount = -1;
        string key = SANTU_AB_FOLDER_NAME + "/" + santuFolderName;
        foreach (var i in tmpCountDic)
        {
            if (i.Key.StartsWith(key))
            {
                int len = key.Length;
                if (i.Key.Length <= len || int.TryParse(i.Key.Substring(len), out int iValue))
                {
                    if (minCount == -1 || i.Value < minCount)
                    {
                        minCount = i.Value;
                        minPath = i.Key;
                    }
                }
            }
        }

        return minPath;
    }

    public int GetMaxAbPathIndex(string abPath)
    {
        int maxindex = -1;
        foreach (var value in abPaths)
        {
            if (value.StartsWith(abPath))
            {
                string str = value.Substring(abPath.Length);
                if (!int.TryParse(str, out int index)) index = 0;
                if (index > maxindex) maxindex = index;
            }
        }
        foreach (var value in repeatSpAbPaths)
        {
            if (value.abPath.StartsWith(abPath))
            {
                string str = value.abPath.Substring(abPath.Length);
                if (!int.TryParse(str, out int index)) index = 0;
                if (index > maxindex) maxindex = index;
            }
        }
        return maxindex;
    }

    public void AddSprite(string spName, string abPath)
    {
        int index = -1;
        for (int i = 0; i < spNames.Count; i++)
        {
            if (spNames[i] == spName)
            {
                index = i;
                break;
            }
        }

        if (index < 0)
        {
            AddNewSprite(spName, abPath);
        }
        else
        {
            string oldAbPath = abPaths[index];
            string oldAtlasName = GetAtlasNameByAbPath(oldAbPath);
            string newAtlasName = GetAtlasNameByAbPath(abPath);
            
            RemoveSprite(index);
            AddNewRepeatSprite(oldAtlasName, spName, oldAbPath);
            AddNewRepeatSprite(newAtlasName, spName, abPath);
        }
    }

    public void RemoveSprite(string spName, string santuFolderPath)
    {
        for (int i = 0; i < spNames.Count; i++)
        {
            if (spNames[i] == spName)
            {
                RemoveSprite(i);
                break;
            }
        }
        
        for (int i = repeatSpAbPaths.Count - 1; i >= 0; i--)
        {
            if (repeatSpAbPaths[i].spName == spName && repeatSpAtlasNames[i] == santuFolderPath)
            {
                RemoveRepeatSprite(i);
                break;
            }
        }

        int count = 0;
        for (int i = repeatSpAbPaths.Count - 1; i >= 0; i--)
        {
            if (repeatSpAbPaths[i].spName == spName)
            {
                count++;
            }
        }

        if (count == 1)
        {
            for (int i = repeatSpAbPaths.Count - 1; i >= 0; i--)
            {
                if (repeatSpAbPaths[i].spName == spName)
                {
                    AddNewSprite(spName, repeatSpAbPaths[i].abPath);
                    RemoveRepeatSprite(i);
                    break;
                }
            }
        }
    }

    void RemoveSprite(int index)
    {
        spNames.RemoveAt(index);
        abPaths.RemoveAt(index);
    }

    void RemoveRepeatSprite(int index)
    {
        repeatSpAtlasNames.RemoveAt(index);
        repeatSpAbPaths.RemoveAt(index);
    }

    void AddNewSprite(string spName, string abPath)
    {
        spNames.Add(spName);
        abPaths.Add(abPath);
    }

    void AddNewRepeatSprite(string atlasName, string spName, string abPath)
    {
        repeatSpAtlasNames.Add(atlasName);
        repeatSpAbPaths.Add(new SpriteAbPath {spName = spName, abPath = abPath});
    }
    
    public static string GetAtlasNameBySantuFolderName(string santuFolderName)
    {
        if (string.IsNullOrEmpty(santuFolderName)) return santuFolderName;
        if (santuFolderName == "zhongqiuhuodong2022") return santuFolderName;
        string tmpStr = santuFolderName.Substring(santuFolderName.Length - 1, 1);
        while (int.TryParse(tmpStr, out int num))
        {
            santuFolderName = santuFolderName.Substring(0, santuFolderName.Length - 1);
            tmpStr = santuFolderName.Substring(santuFolderName.Length - 1, 1);
        }
        if (santuFolderName == "soulwake") santuFolderName = "soulawake";
        return santuFolderName;
    }
    
    public static string GetAtlasNameByAbPath(string abPath)
    {
        abPath = abPath.Replace( "UISantu/", "");
        return GetAtlasNameBySantuFolderName(abPath);
    }

    public void Clear()
    {
        spNames.Clear();
        abPaths.Clear();
        repeatSpAbPaths.Clear();
        repeatSpAtlasNames.Clear();
    }
#endif
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class UIEditorTool
{
    public static bool CheckAsset<T>(string path, out T result) where T : UnityEngine.Object
    {
        result = null;
        if (!string.IsNullOrEmpty(path))
        {
            result = AssetDatabase.LoadAssetAtPath<T>(path);
        }
        return result;
    }

    public static bool CheckAssetList<T>(List<string> paths, out T[] results) where T : UnityEngine.Object
    {
        results = new T[paths.Count];
        for (int i = 0; i < paths.Count; i++)
        {
            T result = AssetDatabase.LoadAssetAtPath<T>(paths[i]);
            if (!result)
            {
                return false;
            }
            results[i] = result;
        }

        return true;
    }
    
    /// <summary>
    /// 获取Transform在给定根节点下的路径
    /// </summary>
    public static string TransformToPath(Transform root, Transform tran, out string indexPath)
    {
        string path = "";
        indexPath = "";
        while (tran != null && tran != root)
        {
            path = "/" + tran.name + path;
            int index = tran.GetSiblingIndex();
            indexPath = "/" + index + indexPath;
            tran = tran.parent;
        }

        return path;
    }

    /// <summary>
    /// 根据路径在给定根节点下找到对应节点
    /// </summary>
    public static bool PathToTransform(string path, string indexPath, Transform root, out Transform tran)
    {
        if (!string.IsNullOrEmpty(path))
        {
            string compPath = path.Substring(1);
            tran = root.Find(compPath);
            if (!tran)
            {
                if (!string.IsNullOrEmpty(indexPath))
                {
                    indexPath = indexPath.Substring(1);
                    string[] indexs = indexPath.Split(new []{'/'}, StringSplitOptions.RemoveEmptyEntries);
                    Transform temp = root;
                    for (int i = 0; i < indexs.Length; i++)
                    {
                        int index = int.Parse(indexs[i]);
                        if (temp.childCount <= index)
                        {
                            return false;
                        }
                        else
                        {
                            temp = temp.GetChild(index);
                            if (!temp)
                            {
                                return false;
                            }
                        }
                    }
                    tran = temp;
                    return true;
                }
                else
                {
                    tran = root;
                    return true;
                }
            }

            return true;
        }

        tran = root;
        return true;
    }
}

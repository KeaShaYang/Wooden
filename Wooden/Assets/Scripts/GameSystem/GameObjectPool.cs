using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectPool : Singleton<GameObjectPool>
{
    private Dictionary<string, List<GameObject>> objCache;
    private Transform poolRoot;
    public void F_Init()
    {
        if (null == poolRoot)
        {
            GameObject root = new GameObject("PoolRoot");
            poolRoot = root.GetComponent<Transform>();
        }
        objCache = new Dictionary<string, List<GameObject>>();
    }
    public GameObject CreateObject(string key, GameObject prefab, Vector3 pos,Vector2 scale)
    {
        GameObject go;
        go = FindUsableObject(key);
        if (go == null)
        {
            go = AddObject(key, prefab);
        }
        UseObject(go, pos, scale);
        return go;
    }
    public GameObject FindUsableObject(string key)
    {
        if (objCache.ContainsKey(key))
        {
            return objCache[key].Find(go => (null != go  && !go.activeInHierarchy));
        }
        else
            return null;
    }
    public GameObject AddObject(string key, GameObject prefab)
    {
        GameObject go = GameObject.Instantiate(prefab);
        if (!objCache.ContainsKey(key)) objCache.Add(key, new List<GameObject>() { go});
        objCache[key].Add(go);
        return go;
    }
    public void UseObject(GameObject go, Vector3 pos, Vector2 scale)
    {
        go.transform.position = pos;
        go.transform.localScale = scale;
        go.transform.eulerAngles = Vector3.zero;
        go.SetActive(true);
       
    }
    public void CollectObject(GameObject go, float delay = 0)
    {
        if (null != go)
        {
            go.transform.SetParent(poolRoot);
            TimerMgr.GetInstance().Schedule(() =>
            {
                if (null != go)
                {
                    go.SetActive(false);
                }
            }, delay, 0, 1);
        }
    }
    public void Clear(string key)
    {
        if (objCache.ContainsKey(key))
        {
            foreach (var item in objCache[key])
            {
                GameObject.Destroy(item);
            }
            objCache.Remove(key);
        }
    }
    public void ClearAll()
    {
        foreach (var item in new List<string>(objCache.Keys))
        {
            Clear(item);
        }
    }
}



using System.Collections.Generic;
using UnityEngine;


public class UITool
{
    public static void F_AddItemList<T>(int count, List<T> list, T prefab, Transform parent) where T : MonoBehaviour
    {
        for (int i = 0; i < count; i++)
        {
            if (list.Count <= i)
            {
                GameObject go = GameObject.Instantiate(prefab.gameObject, parent);
                T item = GameObject.Instantiate(prefab, parent);
                item.gameObject.SetActive(true);
                list.Add(item);
            }
            else
                list[i].gameObject.SetActive(true);
        }
        F_HideMore<T>(list, count);
    }
    public static T F_AddItem<T>(int index, List<T> list, T prefab, Transform parent) where T : MonoBehaviour
    {
        if (index > -1 && index < list.Count)
        {
            list[index].gameObject.SetActive(true);
            return list[index];
        }
        else
        {
            T item = GameObject.Instantiate(prefab, parent);
            item.gameObject.SetActive(true);
            list.Add(item);
            return item;
        }
    }
    public static void F_HideMore<T>(List<T> items, int count) where T : MonoBehaviour
    {
        if (null != items)
        {
            for (int i = count; i < items.Count; i++)
            {
                items[i].gameObject.SetActive(false);
            }
        }
    }
    public static string F_GetQualitBg(int quality,bool isActive = false)
    {
        if (!isActive)
            return "item_yxk0004";
        switch (quality)
        {
            case 1: return "item_yxk0003";
            case 2: return "item_yxk0002";
            case 3: return "item_yxk0002";
            case 4:return "item_yxk0001";
            default:return "item_yxk0004";
        }
    }
    public static string F_GetItemQualitBg(int quality)
    {
        switch (quality)
        {
            case 1: return "comm_pt";
            case 2: return "comm_zi";
            case 3: return "comm_cheng";
            case 4: return "comm_hong";
            default: return "comm_pt";
        }
    }
    public static string F_GetKingIcon(int king)
    {
        switch (king)
        {
            case 0: return "item_qun";
            case 1: return "item_wei";
            case 2: return "item_shu";
            case 3: return "item_wu";
            default: return "item_qun";
        }
    }
}


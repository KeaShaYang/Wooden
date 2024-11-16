using System.Collections.Generic;
using System;
using UnityEngine;


public class ExcelManager : Singleton<ExcelManager>
{
    Dictionary<Type, object> excelDataDic = new Dictionary<Type, object>();

    public T GetExcelData<T, V>() where T : ExcelDataBase<V> where V : ExcelItemBase
    {
        Type type = typeof(T);
        if (excelDataDic.ContainsKey(type) && excelDataDic[type] is T)
            return excelDataDic[type] as T;
        //, type.Name
        T excelData = Resources.Load<T>(DataPath.ExcelAssetDataPath + type.Name);

        if (excelData != null)
            excelDataDic.Add(type, excelData as T);
        else
            Debug.LogError("路径资源不存在"+ DataPath.ExcelAssetDataPath + type.Name + ".asset");

        return excelData;
    }

    public V GetExcelItem<T, V>(int targetId) where T : ExcelDataBase<V> where V : ExcelItemBase
    {
        var excelData = GetExcelData<T, V>();

        if (excelData != null)
            return excelData.GetExcelItem(targetId);
        return null;
    }

}
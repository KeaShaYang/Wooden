/*Auto Create, Don't Edit !!!*/

using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;

[Serializable]
public class q_colorExcelItem : ExcelItemBase
{
	public string name;
	public string valueType;
}

[CreateAssetMenu(fileName = "q_color", menuName = "Excel To ScriptableObject/Create q_color", order = 1)]
public class q_color : ExcelDataBase<q_colorExcelItem>
{
}

#if UNITY_EDITOR
public class q_colorAssetAssignment
{
	public static bool CreateAsset(List<Dictionary<string, string>> allItemValueRowList, string excelAssetPath)
	{
		if (allItemValueRowList == null || allItemValueRowList.Count == 0)
			return false;
		int rowCount = allItemValueRowList.Count;
		q_colorExcelItem[] items = new q_colorExcelItem[rowCount];
		for (int i = 0; i < items.Length; i++)
		{
			items[i] = new q_colorExcelItem();
			items[i].id = Convert.ToInt32(allItemValueRowList[i]["id"]);
			items[i].name = allItemValueRowList[i]["name"];
			items[i].valueType = allItemValueRowList[i]["valueType"];
		}
		q_color excelDataAsset = ScriptableObject.CreateInstance<q_color>();
		excelDataAsset.items = items;
		if (!Directory.Exists(excelAssetPath))
			Directory.CreateDirectory(excelAssetPath);
		string pullPath = excelAssetPath + "/" + typeof(q_color).Name + ".asset";
		UnityEditor.AssetDatabase.DeleteAsset(pullPath);
		UnityEditor.AssetDatabase.CreateAsset(excelDataAsset, pullPath);
		UnityEditor.AssetDatabase.Refresh();
		return true;
	}
}
#endif



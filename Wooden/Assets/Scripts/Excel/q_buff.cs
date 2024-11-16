/*Auto Create, Don't Edit !!!*/

using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;

[Serializable]
public class q_buffExcelItem : ExcelItemBase
{
	public string name;
	public int type;
	public int effect;
	public float duration;
}

[CreateAssetMenu(fileName = "q_buff", menuName = "Excel To ScriptableObject/Create q_buff", order = 1)]
public class q_buff : ExcelDataBase<q_buffExcelItem>
{
}

#if UNITY_EDITOR
public class q_buffAssetAssignment
{
	public static bool CreateAsset(List<Dictionary<string, string>> allItemValueRowList, string excelAssetPath)
	{
		if (allItemValueRowList == null || allItemValueRowList.Count == 0)
			return false;
		int rowCount = allItemValueRowList.Count;
		q_buffExcelItem[] items = new q_buffExcelItem[rowCount];
		for (int i = 0; i < items.Length; i++)
		{
			items[i] = new q_buffExcelItem();
			items[i].id = Convert.ToInt32(allItemValueRowList[i]["id"]);
			items[i].name = allItemValueRowList[i]["name"];
			items[i].type = Convert.ToInt32(allItemValueRowList[i]["type"]);
			items[i].effect = Convert.ToInt32(allItemValueRowList[i]["effect"]);
			items[i].duration = Convert.ToSingle(allItemValueRowList[i]["duration"]);
		}
		q_buff excelDataAsset = ScriptableObject.CreateInstance<q_buff>();
		excelDataAsset.items = items;
		if (!Directory.Exists(excelAssetPath))
			Directory.CreateDirectory(excelAssetPath);
		string pullPath = excelAssetPath + "/" + typeof(q_buff).Name + ".asset";
		UnityEditor.AssetDatabase.DeleteAsset(pullPath);
		UnityEditor.AssetDatabase.CreateAsset(excelDataAsset, pullPath);
		UnityEditor.AssetDatabase.Refresh();
		return true;
	}
}
#endif



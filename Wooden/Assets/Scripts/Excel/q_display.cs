/*Auto Create, Don't Edit !!!*/

using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;

[Serializable]
public class q_displayExcelItem : ExcelItemBase
{
	public string name;
	public string resPath;
	public int scale;
	public int scaleY;
}

[CreateAssetMenu(fileName = "q_display", menuName = "Excel To ScriptableObject/Create q_display", order = 1)]
public class q_display : ExcelDataBase<q_displayExcelItem>
{
}

#if UNITY_EDITOR
public class q_displayAssetAssignment
{
	public static bool CreateAsset(List<Dictionary<string, string>> allItemValueRowList, string excelAssetPath)
	{
		if (allItemValueRowList == null || allItemValueRowList.Count == 0)
			return false;
		int rowCount = allItemValueRowList.Count;
		q_displayExcelItem[] items = new q_displayExcelItem[rowCount];
		for (int i = 0; i < items.Length; i++)
		{
			items[i] = new q_displayExcelItem();
			items[i].id = Convert.ToInt32(allItemValueRowList[i]["id"]);
			items[i].name = allItemValueRowList[i]["name"];
			items[i].resPath = allItemValueRowList[i]["resPath"];
			items[i].scale = Convert.ToInt32(allItemValueRowList[i]["scale"]);
			items[i].scaleY = Convert.ToInt32(allItemValueRowList[i]["scaleY"]);
		}
		q_display excelDataAsset = ScriptableObject.CreateInstance<q_display>();
		excelDataAsset.items = items;
		if (!Directory.Exists(excelAssetPath))
			Directory.CreateDirectory(excelAssetPath);
		string pullPath = excelAssetPath + "/" + typeof(q_display).Name + ".asset";
		UnityEditor.AssetDatabase.DeleteAsset(pullPath);
		UnityEditor.AssetDatabase.CreateAsset(excelDataAsset, pullPath);
		UnityEditor.AssetDatabase.Refresh();
		return true;
	}
}
#endif



/*Auto Create, Don't Edit !!!*/

using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;

[Serializable]
public class q_levelExcelItem : ExcelItemBase
{
	public int colorNum;
	public int stingNum;
	public string model;
	public string reward;
}

[CreateAssetMenu(fileName = "q_level", menuName = "Excel To ScriptableObject/Create q_level", order = 1)]
public class q_level : ExcelDataBase<q_levelExcelItem>
{
}

#if UNITY_EDITOR
public class q_levelAssetAssignment
{
	public static bool CreateAsset(List<Dictionary<string, string>> allItemValueRowList, string excelAssetPath)
	{
		if (allItemValueRowList == null || allItemValueRowList.Count == 0)
			return false;
		int rowCount = allItemValueRowList.Count;
		q_levelExcelItem[] items = new q_levelExcelItem[rowCount];
		for (int i = 0; i < items.Length; i++)
		{
			items[i] = new q_levelExcelItem();
			items[i].id = Convert.ToInt32(allItemValueRowList[i]["id"]);
			items[i].colorNum = Convert.ToInt32(allItemValueRowList[i]["colorNum"]);
			items[i].stingNum = Convert.ToInt32(allItemValueRowList[i]["stingNum"]);
			items[i].model = allItemValueRowList[i]["model"];
			items[i].reward = allItemValueRowList[i]["reward"];
		}
		q_level excelDataAsset = ScriptableObject.CreateInstance<q_level>();
		excelDataAsset.items = items;
		if (!Directory.Exists(excelAssetPath))
			Directory.CreateDirectory(excelAssetPath);
		string pullPath = excelAssetPath + "/" + typeof(q_level).Name + ".asset";
		UnityEditor.AssetDatabase.DeleteAsset(pullPath);
		UnityEditor.AssetDatabase.CreateAsset(excelDataAsset, pullPath);
		UnityEditor.AssetDatabase.Refresh();
		return true;
	}
}
#endif



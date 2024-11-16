using System;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "CustomDrawModeAsset", menuName = "CustomDrawModeAsset", order = 99)]
public class CustomDrawModeAsset : ScriptableObject 
{
	[Serializable]
	public struct CustomDrawMode
	{
		public string name;
		public string category;
		public Shader shader;
        public bool replaceRenderType;
    }
	public CustomDrawMode[] customDrawModes ;
}

public static class CustomDrawModeAssetObject
{
	public static CustomDrawModeAsset cdma;

	public static bool SetUpObject()
	{
		if(cdma == null)
		{
			cdma = (CustomDrawModeAsset)AssetDatabase.LoadAssetAtPath("Assets/ThirdParty/AMLD/Editor/Module/DrawMode/CustomDrawModeAsset.asset", typeof(CustomDrawModeAsset));
		}
        return cdma != null;
	}
}

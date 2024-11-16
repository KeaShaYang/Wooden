using UnityEngine;

public class PathHelper

{
    public static readonly string ExcelExtend = ".xlsx";
    public static readonly string ExcelToCsharpOutputPath = Application.dataPath + "/Scripts/Data";
    public static readonly string ExcelPath = Application.dataPath .Replace("/Assets", "/Excel") ;
    public static readonly string ExcelAssetDataPath =  "Assets/Resources/ExcelData/";
    public static readonly string ExcelConfigPath = Application.dataPath + "/Scripts/Config/";
    public static readonly string EditorPath = Application.dataPath + "/Editor/";
    public const string EM_WinMainPath = "UI";

}
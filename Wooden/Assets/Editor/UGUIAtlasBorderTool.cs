using System;
using UnityEditor;
using UnityEngine;
using File = System.IO.File;
using Object = UnityEngine.Object;

public class UGUIAtlasBorderTool
{
//    [MenuItem("AMLD/UI工具/同步图集九宫数据")]
//    public static void SyncAtlasBorder()
//    {
//        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
//        TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
//        if (textureImporter)
//        {
//            SyncAtlasBorder(textureImporter);
//            EditorUtility.DisplayDialog("提示", "图集九宫数据同步完成", "ok");
//            textureImporter.SaveAndReimport();
//        }
//        else
//        {
//            EditorUtility.DisplayDialog("提示", "请先选中要同步的图集", "ok");
//        }
//    }

    public static void SyncAtlasBorder(TextureImporter textureImporter)
    {
        if (textureImporter)
        {
            string assetPath = textureImporter.assetPath;
            int index1 = assetPath.LastIndexOf('/');
            int index2 = assetPath.LastIndexOf('.');
            if (index1 < 0 || index2 < 0) return;
            string atlasName = assetPath.Substring(index1 + 1, index2 - index1 - 1);
            string santuFolderPath = "Assets/Art/UI/santu/" + atlasName;
            var spriteSheet = textureImporter.spritesheet;
            string[] guids = AssetDatabase.FindAssets("t:Sprite", new[] {santuFolderPath});
            for (int i = 0; i < guids.Length; i++)
            {
                string spritePath = AssetDatabase.GUIDToAssetPath(guids[i]);
                Sprite sp = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
                if (sp)
                {
                    for (int j = 0; j < spriteSheet.Length; j++)
                    {
                        if (sp.name == spriteSheet[j].name)
                        {
                            var data = spriteSheet[j];
                            data.border = sp.border;
                            spriteSheet[j] = data;
                            break;
                        }
                    }
                }
            }
            textureImporter.spritesheet = spriteSheet;
        }
    }
}
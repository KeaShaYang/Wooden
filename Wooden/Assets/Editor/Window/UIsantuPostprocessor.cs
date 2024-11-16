using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class UIsantuPostprocessor : AssetPostprocessor
{
    public static bool enable = false;
    private const string SANTU_FOLDER_PATH = "Assets/Resources/UISantu";

    private void OnPostprocessTexture(Texture2D texture)
    {
        if (!enable) return;
        if (assetPath.StartsWith(SANTU_FOLDER_PATH))
        {
            TextureImporter ti = assetImporter as TextureImporter;
            if (ti)
            {
                ti.textureType = TextureImporterType.Default;
                ti.mipmapEnabled = false;
                ti.npotScale = TextureImporterNPOTScale.None;
                ti.sRGBTexture = false;
                ti.alphaIsTransparency = true;
                TextureImporterPlatformSettings iosSettings = ti.GetPlatformTextureSettings("iOS");
                iosSettings.overridden = true;
                iosSettings.format = TextureImporterFormat.RGBA32;
                ti.SetPlatformTextureSettings(iosSettings);
                TextureImporterPlatformSettings androidSettings = ti.GetPlatformTextureSettings("Android");
                androidSettings.overridden = true;
                androidSettings.format = TextureImporterFormat.RGBA32;
                ti.SetPlatformTextureSettings(androidSettings);
                TextureImporterPlatformSettings standaloneSettings = ti.GetPlatformTextureSettings("Standalone");
                standaloneSettings.overridden = true;
                standaloneSettings.format = TextureImporterFormat.RGBA32;
                ti.SetPlatformTextureSettings(standaloneSettings);
                // EditorUtility.SetDirty(ti);
            }
        }
    }

    private static HashSet<string> hasImportedSantu = new HashSet<string>();
    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
        string[] movedFromAssetPaths)
    {
        if (!enable) return;
        
        // 由于unity的导入机制bug，强制对散图导入2次，不然散图的导入格式会有问题
        List<string> santuAssests = new List<string>();
        foreach (var importedAsset in importedAssets)
        {
            if (importedAsset.StartsWith(SANTU_FOLDER_PATH) && !hasImportedSantu.Contains(importedAsset))
            {
                hasImportedSantu.Add(importedAsset);
                santuAssests.Add(importedAsset);
            }
        }
        if (santuAssests.Count > 0)
        {
            // 用try catch防止中间报错导致unity的AssetDatabase刷新机制坏掉
            try
            {
                AssetDatabase.StartAssetEditing();
                foreach (var santuAssest in santuAssests)
                {
                    AssetDatabase.ImportAsset(santuAssest, ImportAssetOptions.ForceUpdate);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
            }
        }
    }
}

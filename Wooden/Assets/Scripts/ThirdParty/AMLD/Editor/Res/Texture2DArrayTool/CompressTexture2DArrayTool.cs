using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CompressTexture2DArrayTool : ScriptableWizard
{

    public enum Action
    {
        Compress,
        Decompress
    }
    public Action action = Action.Compress;

    [Header("贴图数组")]
    public Texture2DArray textures;

    public bool mipChain = true;
    public bool linear = false;

    public TextureFormat format;
    public UnityEditor.TextureCompressionQuality quality;

    [MenuItem("AMLD/场景工具/Texture2DArray工具/Texture2DArray压缩")]
    static void CompressTexArray()
    {
        ScriptableWizard.DisplayWizard<CompressTexture2DArrayTool>("Texture2DArray压缩", "生成!");
    }

    private void OnWizardUpdate()
    {
        helpString = "按步骤操作";
        isValid = (textures != null);
    }

    private void OnWizardCreate()
    {
        string savePath = EditorUtility.SaveFilePanel("选择存储数组的位置", Application.dataPath, "New Texture Array", "asset");
        int subIndex = savePath.IndexOf("Assets");
        if (subIndex < 0)
        {
            return;
        }
        savePath = savePath.Substring(subIndex);


        Texture2DArray texArr = new Texture2DArray(textures.width, textures.height, textures.depth, format, mipChain, linear);
        texArr.wrapMode = textures.wrapMode;
        texArr.filterMode = textures.filterMode;

        Texture2D[] temp = new Texture2D[textures.depth];

        for (int i = 0; i < temp.Length; i++)
        {
            temp[i] = new Texture2D(textures.width, textures.height, TextureFormat.RGBA32, mipChain);
            temp[i].SetPixels(textures.GetPixels(i));
            temp[i].Apply();

            if (action == Action.Compress)
            {
                EditorUtility.CompressTexture(temp[i], format, quality);
            }
        }
        for (int i = 0; i < temp.Length; i++)
        {
            Graphics.CopyTexture(temp[i], 0, texArr, i);
        }

        //texArr.Apply(true, true);  //压缩完后不再修改，第二个参数设置为true，noreadable


        AssetDatabase.CreateAsset(texArr, savePath);
        AssetDatabase.Refresh();
    }
}

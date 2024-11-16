using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class GenerateTexture2DArrayTool : ScriptableWizard
{

    [Header("贴图放置处")]
    public Texture2D[] textures;

    [Header("是否开启MipMap")]
    public bool mipChain = true;
    [Header("色彩空间")]
    public bool linear = false;

    [MenuItem("AMLD/场景工具/Texture2DArray工具/Texture2DArray生成工具")]
    static void GenerateTexture2DArray()
    {
        ScriptableWizard.DisplayWizard<GenerateTexture2DArrayTool>("Texture2DArray生成工具", "生成!");
    }

    private void OnWizardUpdate()
    {
        helpString = "请确保生成数组的所有贴图与第一张的尺寸和格式一致。\n生成的贴图数组的尺寸和格式将由第一张贴图决定。";
        isValid = (textures != null && textures.Length > 0);
    }

    private void OnWizardCreate()
    {
        string savePath = EditorUtility.SaveFilePanel("选择存储的位置", Application.dataPath, "New Texture Array", "asset");
        int subIndex = savePath.IndexOf("Assets");
        if (subIndex < 0)
        {
            return;
        }
        savePath = savePath.Substring(subIndex);

        Texture2DArray texArr = new Texture2DArray(textures[0].width, textures[0].height, textures.Length, textures[0].format, mipChain, linear);
        texArr.wrapMode = TextureWrapMode.Repeat;
        texArr.filterMode = FilterMode.Bilinear;

        for (int i = 0; i < textures.Length; i++)
        {
            Graphics.CopyTexture(textures[i], 0, texArr, i);
        }

        AssetDatabase.CreateAsset(texArr, savePath);

        AssetDatabase.Refresh();
    }


}

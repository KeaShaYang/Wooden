using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;


public class CombineTex2ArrayTool : ScriptableWizard
{

    public enum CombineType
    {
        Combine_15_1, //前15张 和最后一张底图
        Combine_16,
        Combine_4
    }

    [Header("拼接贴图")]
    public Texture2D texture;

    [Header("拼接类型")]
    [Tooltip("1.Combine_15_1: 16张拼接图 15张+最后1张底图\n2.Combine_16:16张拼接图\n3.Combine_4: 4张拼接图")]
    public CombineType combineType;

    [Header("Texture2DArray设置")]
    public bool mipChain = true;
    public bool linear = false;
    public TextureWrapMode wrapMode = TextureWrapMode.Repeat;
    public FilterMode filterMode = FilterMode.Bilinear;

    [Header("是否移除边缘收缩8px")]
    public bool removeShrinkage = false;

    [MenuItem("AMLD/场景工具/Texture2DArray工具/拼接贴图转Texture2DArray")]
    static void CombineTex2Array()
    {
        ScriptableWizard.DisplayWizard<CombineTex2ArrayTool>("拼接贴图转数组", "生成!");
    }

    private void OnWizardUpdate()
    {
        helpString = "按步骤操作";
        isValid = (texture != null);
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


        int width = 0;
        int height = 0;
        int depth = 0;
        Texture[] textures = null;
        switch (combineType)
        {
            case CombineType.Combine_15_1:
                width = texture.width / 4;
                height = texture.height / 4;
                depth = 16;
                textures = SplitTexture2D(texture, 4, 4, mipChain, linear);
                break;
            case CombineType.Combine_4:
                width = texture.width / 2;
                height = texture.height / 2;
                depth = 4;
                textures = SplitTexture2D(texture, 2, 2, mipChain, linear);
                break;
        }


        Texture2DArray texArr = new Texture2DArray(width, height, depth, texture.format, mipChain, linear);
        texArr.wrapMode = this.wrapMode;
        texArr.filterMode = this.filterMode;

        if (textures != null)
        {
            switch (combineType)
            {
                case CombineType.Combine_15_1:
                    Graphics.CopyTexture(textures[3], 0, texArr, 0);
                    Graphics.CopyTexture(textures[12], 0, texArr, 1);
                    Graphics.CopyTexture(textures[13], 0, texArr, 2);
                    Graphics.CopyTexture(textures[14], 0, texArr, 3);
                    Graphics.CopyTexture(textures[15], 0, texArr, 4);
                    Graphics.CopyTexture(textures[8], 0, texArr, 5);
                    Graphics.CopyTexture(textures[9], 0, texArr, 6);
                    Graphics.CopyTexture(textures[10], 0, texArr, 7);
                    Graphics.CopyTexture(textures[11], 0, texArr, 8);
                    Graphics.CopyTexture(textures[4], 0, texArr, 9);
                    Graphics.CopyTexture(textures[5], 0, texArr, 10);
                    Graphics.CopyTexture(textures[6], 0, texArr, 11);
                    Graphics.CopyTexture(textures[7], 0, texArr, 12);
                    Graphics.CopyTexture(textures[0], 0, texArr, 13);
                    Graphics.CopyTexture(textures[1], 0, texArr, 14);
                    Graphics.CopyTexture(textures[2], 0, texArr, 15);
                    break;
                case CombineType.Combine_16:
                    Graphics.CopyTexture(textures[12], 0, texArr, 0);
                    Graphics.CopyTexture(textures[13], 0, texArr, 1);
                    Graphics.CopyTexture(textures[14], 0, texArr, 2);
                    Graphics.CopyTexture(textures[15], 0, texArr, 3);
                    Graphics.CopyTexture(textures[8], 0, texArr, 4);
                    Graphics.CopyTexture(textures[9], 0, texArr, 5);
                    Graphics.CopyTexture(textures[10], 0, texArr, 6);
                    Graphics.CopyTexture(textures[11], 0, texArr, 7);
                    Graphics.CopyTexture(textures[4], 0, texArr, 8);
                    Graphics.CopyTexture(textures[5], 0, texArr, 9);
                    Graphics.CopyTexture(textures[6], 0, texArr, 10);
                    Graphics.CopyTexture(textures[7], 0, texArr, 11);
                    Graphics.CopyTexture(textures[0], 0, texArr, 12);
                    Graphics.CopyTexture(textures[1], 0, texArr, 13);
                    Graphics.CopyTexture(textures[2], 0, texArr, 14);
                    Graphics.CopyTexture(textures[3], 0, texArr, 15);
                    break;
                case CombineType.Combine_4:
                    Graphics.CopyTexture(textures[2], 0, texArr, 0);
                    Graphics.CopyTexture(textures[0], 0, texArr, 1);
                    Graphics.CopyTexture(textures[3], 0, texArr, 2);
                    Graphics.CopyTexture(textures[1], 0, texArr, 3);
                    break;
            }
        }

        AssetDatabase.CreateAsset(texArr, savePath);
        AssetDatabase.Refresh();
    }


    /// <summary>
    /// 切分贴图
    /// </summary>
    private Texture2D[] SplitTexture2D(Texture2D source, int x, int y, bool mipmap, bool linear)
    {
        Texture2D[] tex = new Texture2D[x * y];
        int newX = Mathf.CeilToInt(source.width / x);
        int newY = Mathf.CeilToInt(source.height / y);
        for (int i = 0; i < tex.Length; i++)
        {
            tex[i] = new Texture2D(newX, newY, source.format, mipmap, linear);
        }
        for (int h = 0; h < source.height; h++)
        {
            for (int w = 0; w < source.width; w++)
            {
                int index = (y - 1 - (int)(h / newY)) * y + (int)(w / newX);

                Color c = source.GetPixel(w, h);
                //c.a = 1;

                tex[index].SetPixel(w - (int)(w / newX) * newX, h - (y - 1 - (int)(h / newY)) * newY, c);
            }
        }
        for (int i = 0; i < tex.Length; i++)
        {
            tex[i].Apply();

            if (removeShrinkage)
            {
                RemoveTexBorder(tex[i]);
            }
        }
        return tex;
    }

    private void RemoveTexBorder(Texture2D tex)
    {
        int shrinkage = 8;

        int width = tex.width;
        int height = tex.height;
        Color[] scaleColor = tex.GetPixels(shrinkage, shrinkage, width - 2 * shrinkage, height - 2 * shrinkage);
        Texture2D temp = new Texture2D(width - 2 * shrinkage, height - 2 * shrinkage);
        temp.SetPixels(scaleColor);
        temp.Apply();

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                Color newColor = temp.GetPixelBilinear((float)j / (float)width, (float)i / (float)height);
                tex.SetPixel(j, i, newColor);
            }
        }
        tex.Apply();
    }


}

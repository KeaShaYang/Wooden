using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// 纠正颜色版
/// </summary>
public class TextGamma : Text
{
    [SerializeField]
    private bool m_UseShrinkBestFit = false;
    /// <summary>
    /// 是否需要处理标点打头的情况
    /// </summary>
    [SerializeField] private bool m_IsNeedDealHeadChar = true;
    
    /// <summary>
    /// 是否有标点首行的情况
    /// </summary>
    private bool m_NeedDealHeadChar = false;

    static readonly UIVertex[] m_TempVerts = new UIVertex[4];
    static readonly HashSet<char> charset = new HashSet<char>(new char[]
        {',', ';', '.', '?', '!', '，', '。', '；', '？', '！', ')', '”', '’', '）', '》'});
    private static List<UILineInfo> lines = new List<UILineInfo>();

    private void UseFitSettings()
    {
        TextGenerationSettings settings = GetGenerationSettings(rectTransform.rect.size);
        settings.resizeTextForBestFit = false;
        if (!resizeTextForBestFit)
        {
            cachedTextGenerator.PopulateWithErrors(text, settings, gameObject);
            return;
        }

        int minSize = resizeTextMinSize;
        int txtLen = text.Length;
        for (int i = resizeTextMaxSize; i >= minSize; --i)
        {
            settings.fontSize = i;
            cachedTextGenerator.PopulateWithErrors(text, settings, gameObject);
            if (cachedTextGenerator.characterCountVisible == txtLen) break;
        }
    }

    protected override void OnPopulateMesh(VertexHelper toFill)
    {
        if (font == null)
            return;
        // We don't care if we the font Texture changes while we are doing our Update.
        // The end result of cachedTextGenerator will be valid for this instance.
        // Otherwise we can get issues like Case 619238.
        m_DisableFontTextureRebuiltCallback = true;
        if (m_UseShrinkBestFit)
        {
            UseFitSettings();
        }
        else
        {
            Vector2 extents = rectTransform.rect.size;
            var settings = GetGenerationSettings(extents);
            cachedTextGenerator.PopulateWithErrors(text, settings, gameObject);
        }
        
        //标点首行判断
        cachedTextGenerator.GetLines(lines);
        if (m_IsNeedDealHeadChar && lines.Count > 1) //大于1行才要判断
        {
            if (rectTransform.rect.width > fontSize * 2)
            {
                for (int i = 1; i < lines.Count; i++)
                {
                    if (text.Length>lines[i].startCharIdx && text.Length>lines.Count
                                                          && charset.Contains(text[lines[i].startCharIdx]) 
                                                          && CheckNeedDealHeadChar(text[lines[i].startCharIdx-1]))
                    {
                        m_NeedDealHeadChar = true;
                        break;
                    }
                }
            }
        }
        
        // Apply the offset to the vertices
        IList<UIVertex> verts = cachedTextGenerator.verts;
        float unitsPerPixel = 1 / pixelsPerUnit;
        int vertCount = verts.Count;

        // We have no verts to process just return (case 1037923)
        if (vertCount <= 0)
        {
            toFill.Clear();
            return;
        }

        Vector2 roundingOffset = new Vector2(verts[0].position.x, verts[0].position.y) * unitsPerPixel;
        roundingOffset = PixelAdjustPoint(roundingOffset) - roundingOffset;
        toFill.Clear();
        if (roundingOffset != Vector2.zero)
        {
            for (int i = 0; i < vertCount; ++i)
            {
                int tempVertsIndex = i & 3;
                m_TempVerts[tempVertsIndex] = verts[i];
                m_TempVerts[tempVertsIndex].position *= unitsPerPixel;
                m_TempVerts[tempVertsIndex].position.x += roundingOffset.x;
                m_TempVerts[tempVertsIndex].position.y += roundingOffset.y;
                Color col = ((Color) verts[i].color).gamma;
                col.a = color.a;
                m_TempVerts[tempVertsIndex].color = col;
                if (tempVertsIndex == 3)
                    toFill.AddUIVertexQuad(m_TempVerts);
            }
        }
        else
        {
            for (int i = 0; i < vertCount; ++i)
            {
                int tempVertsIndex = i & 3;
                m_TempVerts[tempVertsIndex] = verts[i];
                m_TempVerts[tempVertsIndex].position *= unitsPerPixel;
                Color col = ((Color) verts[i].color).gamma;
                col.a = color.a;
                m_TempVerts[tempVertsIndex].color = col;
                if (tempVertsIndex == 3)
                    toFill.AddUIVertexQuad(m_TempVerts);
            }
        }
        m_DisableFontTextureRebuiltCallback = false;
    }
    
    private void Update()
    {
        if (m_NeedDealHeadChar && !m_UseShrinkBestFit)
        {
            try
            {
                int trycount = 20;
                cachedTextGenerator.GetLines(lines);
                while (m_NeedDealHeadChar && lines.Count > 1 && trycount>0)
                {
                    string currenttext = text;
                    for (int i = 1; i < lines.Count; i++)
                    {
                        if (currenttext.Length>lines[i].startCharIdx && currenttext.Length>lines.Count
                            && charset.Contains(currenttext[lines[i].startCharIdx]) 
                            && CheckNeedDealHeadChar(currenttext[lines[i].startCharIdx-1]))
                        {
                            currenttext = currenttext.Insert(lines[i].startCharIdx - 1, "\n");//加换行符把上一行的最后一个字弄下来
                            m_NeedDealHeadChar = false;
                            break;
                        }
                    }
                    text = currenttext;
                    Vector2 extents = rectTransform.rect.size;
                    var settings = GetGenerationSettings(extents);
                    cachedTextGenerator.PopulateWithErrors(text, settings, gameObject);
                    cachedTextGenerator.GetLines(lines);
                    if (rectTransform.rect.width > fontSize * 2)
                    {
                        if (lines.Count > 1) //大于1行才要判断
                        {
                            for (int i = 1; i < lines.Count; i++)
                            {
                                if (text.Length>lines[i].startCharIdx && text.Length>lines.Count
                                                                      && charset.Contains(text[lines[i].startCharIdx]) 
                                                                      && CheckNeedDealHeadChar(text[lines[i].startCharIdx-1]))
                                {
                                    m_NeedDealHeadChar = true;
                                    break;
                                }
                            }
                        }
                    }
                    trycount--;
                }
            }
            catch (Exception e)
            {
                
            }
            m_NeedDealHeadChar = false;
        }
    }

    bool CheckNeedDealHeadChar(char c)
    {
        if(c!='\n' && c!='\r'&& c!=' ' && c!='>'&&!charset.Contains(c))
        {
            return true;
        }
        return false;
    }

#if AMLD_HW
    public void SetIsNeedDealHeadChar(bool value)
    {
        m_IsNeedDealHeadChar = value;
    }

    public void SetUseShrinkBestFit(bool value)
    {
        m_UseShrinkBestFit = value;
    }
#endif
}
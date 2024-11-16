using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GradientAndOutline : BaseMeshEffect
{
    [SerializeField]
    private Color m_GradientTop = Color.white;
    
    [SerializeField]
    private Color m_GradientBottom = Color.black;
    
    [SerializeField]
    private bool m_SingleChar = false;
    
    
    [SerializeField]
    private Color m_OutlineColor = new Color(0f, 0f, 0f, 0.5f);

    [SerializeField]
    private Vector2 m_OutlineDistance = new Vector2(1f, 1f);
    
    [SerializeField]
    private bool m_UseGraphicAlpha = true;
    
    
    static List<UIVertex> tmpUIVertexList = new List<UIVertex>();

    public Color GradientTop { get { return m_GradientTop; } set { m_GradientTop = value; graphic.SetAllDirty(); } }
    public Color GradientBottom { get { return m_GradientBottom; } set { m_GradientBottom = value; graphic.SetAllDirty(); } }
    public Color OutlineColor { get { return m_OutlineColor; } set { m_OutlineColor = value; graphic.SetAllDirty(); } }

    public override void ModifyMesh(VertexHelper vh)
    {
        int count = vh.currentVertCount;
        if (!IsActive() || count == 0)
        {
            return;
        }

        //gradient
        UIVertex uiVertex = new UIVertex();
        if (!m_SingleChar)
        {
            float bottomY = int.MaxValue;
            float topY = int.MinValue;

            for (int i = 0; i < 2; i++)
            {
                vh.PopulateUIVertex(ref uiVertex, i);
                if (uiVertex.position.y > topY)
                {
                    topY = uiVertex.position.y;
                }
            }

            int count1 = vh.currentVertCount;
            if (count1 > 1)
            {
                for (int i = count1 - 1; i > count1 - 3; i--)
                {
                    vh.PopulateUIVertex(ref uiVertex, i);
                    if (uiVertex.position.y < bottomY)
                    {
                        bottomY = uiVertex.position.y;
                    }
                }
            }
            else
            {
                vh.PopulateUIVertex(ref uiVertex, 0);
                bottomY = uiVertex.position.y;
            }

            byte alpha = uiVertex.color.a;
            float uiElementHeight = topY - bottomY;
            for (int i = 0; i < count; i++)
            {
                vh.PopulateUIVertex(ref uiVertex, i);
                uiVertex.color = Color.Lerp(m_GradientBottom, m_GradientTop, (uiVertex.position.y - bottomY) / uiElementHeight).gamma;
                uiVertex.color.a = alpha;
                vh.SetUIVertex(uiVertex, i);
            }

        }
        else
        {
            int charVertCount = 4;
            int charCount = vh.currentVertCount / charVertCount;
            for(int i = 0; i < charCount; i++)
            {

                float bottomY = int.MaxValue;
                float topY = int.MinValue;
                for (int j = 0; j < charVertCount; j++)
                {
                    vh.PopulateUIVertex(ref uiVertex, i * charVertCount + j);
                    if (uiVertex.position.y > topY)
                    {
                        topY = uiVertex.position.y;
                    }
                    if (uiVertex.position.y < bottomY)
                    {
                        bottomY = uiVertex.position.y;
                    }
                }
                float uiElementHeight = topY - bottomY;
                byte alpha = uiVertex.color.a;
                for (int j = 0; j < charVertCount; j++)
                {
                    vh.PopulateUIVertex(ref uiVertex, i * charVertCount + j);
                    uiVertex.color = Color.Lerp(m_GradientBottom, m_GradientTop, (uiVertex.position.y - bottomY) / uiElementHeight).gamma;
                    uiVertex.color.a = alpha;
                    vh.SetUIVertex(uiVertex, i * charVertCount + j);
                }
            }
        }
        
        //outline
        vh.GetUIVertexStream(tmpUIVertexList);
        var neededCpacity = tmpUIVertexList.Count * 9;
        if (tmpUIVertexList.Capacity < neededCpacity)
            tmpUIVertexList.Capacity = neededCpacity;
        
        Color outlineColor = m_OutlineColor.gamma;
        var start = 0;
        var end = tmpUIVertexList.Count;
        ApplyShadow(tmpUIVertexList, outlineColor, start, end, m_OutlineDistance.x, m_OutlineDistance.y);
        start = end;
        end = tmpUIVertexList.Count;
        ApplyShadow(tmpUIVertexList, outlineColor, start, end, m_OutlineDistance.x, -m_OutlineDistance.y);
        start = end;
        end = tmpUIVertexList.Count;
        ApplyShadow(tmpUIVertexList, outlineColor, start, end, -m_OutlineDistance.x, m_OutlineDistance.y);
        start = end;
        end = tmpUIVertexList.Count;
        ApplyShadow(tmpUIVertexList, outlineColor, start, end, -m_OutlineDistance.x, -m_OutlineDistance.y);
        start = end;
        end = tmpUIVertexList.Count;
        ApplyShadow(tmpUIVertexList, outlineColor, start, end, m_OutlineDistance.x, 0);
        start = end;
        end = tmpUIVertexList.Count;
        ApplyShadow(tmpUIVertexList, outlineColor, start, end, -m_OutlineDistance.x, 0);
        start = end;
        end = tmpUIVertexList.Count;
        ApplyShadow(tmpUIVertexList, outlineColor, start, end, 0, m_OutlineDistance.y);
        start = end;
        end = tmpUIVertexList.Count;
        ApplyShadow(tmpUIVertexList, outlineColor, start, end, 0, -m_OutlineDistance.y);
        
        vh.Clear();
        vh.AddUIVertexTriangleStream(tmpUIVertexList);
    }
    
    void ApplyShadow(List<UIVertex> verts, Color32 color, int start, int end, float x, float y)
    {
        var neededCapacity = verts.Count + end - start;
        if (verts.Capacity < neededCapacity)
            verts.Capacity = neededCapacity;

        for (int i = start; i < end; ++i)
        {
            var vt = verts[i];
            verts.Add(vt);

            Vector3 v = vt.position;
            v.x += x;
            v.y += y;
            vt.position = v;
            var newColor = color;
            if (m_UseGraphicAlpha)
                newColor.a = (byte)((newColor.a * verts[i].color.a) / 255);
            vt.color = newColor;
            verts[i] = vt;
        }
    }
}

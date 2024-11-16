using System.Collections.Generic;
using Tool.UGUIExtend.Scripts;
using UnityEngine;
using UnityEngine.UI;

public class OutlineAndShadow : BaseMeshEffect
{
    [SerializeField]
    public bool UseOutline = true;

    [SerializeField]
    private Color m_OutlineColor = new Color(0f, 0f, 0f, 0.5f);

    [SerializeField]
    private Vector2 m_OutlineDistance = new Vector2(1f, 1f);

    [SerializeField]
    public bool UseShadow = true;

    [SerializeField]
    private Color m_ShadowColor = new Color(0f, 0f, 0f, 0.5f);

    [SerializeField]
    private Vector2 m_ShadowDistance = new Vector2(0f, -2f);
    
    [SerializeField]
    private bool m_UseGraphicAlpha = true;
    
    static List<UIVertex> tmpUIVertexList = new List<UIVertex>();
    public override void ModifyMesh(VertexHelper vh)
    {
        if (!IsActive() || (!UseOutline && !UseShadow))
            return;

        vh.GetUIVertexStream(tmpUIVertexList);
        int count = tmpUIVertexList.Count;
        
        var neededCpacity = UseShadow ? count * (OutlineBetter.split + 2) : count * (OutlineBetter.split + 1);
        if (tmpUIVertexList.Capacity < neededCpacity)
            tmpUIVertexList.Capacity = neededCpacity;

        if (UseShadow)
        {
            //shadow
            ApplyShadow(tmpUIVertexList, m_ShadowColor.gamma, 0, count, m_ShadowDistance.x, m_ShadowDistance.y);
        }

        if (UseOutline)
        {
            //outline
            Color outlineColor = m_OutlineColor.gamma;
            var start = UseShadow ? count : 0;
            var end = tmpUIVertexList.Count;
            for (int i = 0; i < OutlineBetter.split; i++)
            {
                float offsetX = OutlineBetter.cosValues[i] * m_OutlineDistance.x;
                float offsetY = OutlineBetter.sinValues[i] * m_OutlineDistance.y;
                ApplyShadow(tmpUIVertexList, outlineColor, start, end, offsetX, offsetY);
                start = end;
                end = tmpUIVertexList.Count;
            }
//            ApplyShadow(tmpUIVertexList, outlineColor, start, end, m_OutlineDistance.x, m_OutlineDistance.y);
//            start = end;
//            end = tmpUIVertexList.Count;
//            ApplyShadow(tmpUIVertexList, outlineColor, start, end, m_OutlineDistance.x, -m_OutlineDistance.y);
//            start = end;
//            end = tmpUIVertexList.Count;
//            ApplyShadow(tmpUIVertexList, outlineColor, start, end, -m_OutlineDistance.x, m_OutlineDistance.y);
//            start = end;
//            end = tmpUIVertexList.Count;
//            ApplyShadow(tmpUIVertexList, outlineColor, start, end, -m_OutlineDistance.x, -m_OutlineDistance.y);
//            start = end;
//            end = tmpUIVertexList.Count;
//            ApplyShadow(tmpUIVertexList, outlineColor, start, end, m_OutlineDistance.x, 0);
//            start = end;
//            end = tmpUIVertexList.Count;
//            ApplyShadow(tmpUIVertexList, outlineColor, start, end, -m_OutlineDistance.x, 0);
//            start = end;
//            end = tmpUIVertexList.Count;
//            ApplyShadow(tmpUIVertexList, outlineColor, start, end, 0, m_OutlineDistance.y);
//            start = end;
//            end = tmpUIVertexList.Count;
//            ApplyShadow(tmpUIVertexList, outlineColor, start, end, 0, -m_OutlineDistance.y);
        }
  
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

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using UnityEngine.Sprites;
using UnityEngine.UI;

[RequireComponent(typeof(Graphic))]
public class UIMirror : BaseMeshEffect
{
public enum UIMirrorType
{
    /// <summary>
    /// 水平
    /// </summary>
    Horizontal, 

    /// <summary>
    /// 垂直
    /// </summary>
    Vertical,

    /// <summary>
    /// 四分之一
    /// 相当于水平，然后再垂直
    /// </summary>
    Quarter,
}

/// <summary>
/// 镜像类型
/// </summary>
[SerializeField]
private UIMirrorType m_MirrorType = UIMirrorType.Horizontal;

public UIMirrorType MirrorType
{
    get { return m_MirrorType; }
    set
    {
        if (m_MirrorType != value)
        {
            m_MirrorType = value;
            if(graphic != null){
                graphic.SetVerticesDirty();
            }
        }
    }
}

[NonSerialized]
private RectTransform m_RectTransform;

public RectTransform rectTransform
{
    get { return m_RectTransform ?? (m_RectTransform = GetComponent<RectTransform>()); }
}

/// <summary>
/// 设置原始尺寸
/// </summary>
public void SetNativeSize()
{
    if (graphic != null)
    {
        float w = 0;
        float h = 0;
        if (graphic is Image)
        {
            Sprite overrideSprite = (graphic as Image).overrideSprite;

            if(overrideSprite != null){
                w = overrideSprite.rect.width / (graphic as Image).pixelsPerUnit;
                h = overrideSprite.rect.height / (graphic as Image).pixelsPerUnit;
                rectTransform.anchorMax = rectTransform.anchorMin;
            }
        }
        else
        {
            w = graphic.mainTexture.width;
            h = graphic.mainTexture.height;
        }

        switch (m_MirrorType)
        {
            case UIMirrorType.Horizontal:
                rectTransform.sizeDelta = new Vector2(w * 2, h);
                break;
            case UIMirrorType.Vertical:
                rectTransform.sizeDelta = new Vector2(w, h * 2);
                break;
            case UIMirrorType.Quarter:
                rectTransform.sizeDelta = new Vector2(w * 2, h * 2);
                break;
        }

        graphic.SetVerticesDirty();
    }
}

List<UIVertex> m_vertexs = new List<UIVertex>();
public override void ModifyMesh(VertexHelper vh)
{
    if (!IsActive())
    {
        return;
    }

    m_vertexs.Clear();

    vh.GetUIVertexStream(m_vertexs);

    int count = m_vertexs.Count;

    if (graphic is Image)
    {
        Image.Type type = (graphic as Image).type;

        switch (type)
        {
            case Image.Type.Simple:
                DrawSimple(m_vertexs, count);
                break;
            case Image.Type.Sliced:
                DrawSliced(m_vertexs, count);
                break;
            case Image.Type.Tiled:
                Debug.LogWarning("镜像组件不支持Tiled");
               //DrawTiled(m_vertexs, count);
                break;
            case Image.Type.Filled:
                Debug.LogWarning("镜像组件不支持Filled");
                break;
        }
    }
    else
    {
        DrawSimple(m_vertexs, count);
    }

    vh.Clear();
    vh.AddUIVertexTriangleStream(m_vertexs);
}

protected void DrawSimple(List<UIVertex> output, int count)
{
    Rect rect = graphic.GetPixelAdjustedRect();

    SimpleScale(rect, output, count);
    
    switch (m_MirrorType)
    {
        case UIMirrorType.Horizontal:
            ExtendCapacity(output, count);
            MirrorVerts(rect, output, count, true);
            break;
        case UIMirrorType.Vertical:
            ExtendCapacity(output, count);
            MirrorVerts(rect, output, count, false);
            break;
        case UIMirrorType.Quarter:
            ExtendCapacity(output, count * 3);
            MirrorVerts(rect, output, count, true);
            MirrorVerts(rect, output, count * 2, false);
            break;
    }
}

protected void DrawSliced(List<UIVertex> output, int count)
{
    if (!(graphic as Image).hasBorder)
    {
        DrawSimple(output, count);
        return;
    }

    Rect rect = graphic.GetPixelAdjustedRect();

    SlicedScale(rect, output, count);

    count = SliceExcludeVerts(output, count);
    
    switch (m_MirrorType)
    {
        case UIMirrorType.Horizontal:
            ExtendCapacity(output, count);
            MirrorVerts(rect, output, count, true);
            break;
        case UIMirrorType.Vertical:
            ExtendCapacity(output, count);
            MirrorVerts(rect, output, count, false);
            break;
        case UIMirrorType.Quarter:
            ExtendCapacity(output, count * 3);
            MirrorVerts(rect, output, count, true);
            MirrorVerts(rect, output, count * 2, false);
            break;
    }
}

protected void DrawTiled(List<UIVertex> verts, int count)
{
    Sprite overrideSprite = (graphic as Image).overrideSprite;

    if (overrideSprite == null)
    {
        return;
    }

    Rect rect = graphic.GetPixelAdjustedRect();

    //此处使用inner是因为Image绘制Tiled时，会把透明区域也绘制了。
    
    Vector4 inner = DataUtility.GetInnerUV(overrideSprite);
    
    float w = overrideSprite.rect.width / (graphic as Image).pixelsPerUnit;
    float h = overrideSprite.rect.height / (graphic as Image).pixelsPerUnit;

    int len = count / 3;

    for (int i = 0; i < len; i++)
    {
        UIVertex v1 = verts[i * 3];
        UIVertex v2 = verts[i * 3 + 1];
        UIVertex v3 = verts[i * 3 + 2];

        float centerX = GetCenter(v1.position.x, v2.position.x, v3.position.x);

        float centerY = GetCenter(v1.position.y, v2.position.y, v3.position.y);

        if (m_MirrorType == UIMirrorType.Horizontal || m_MirrorType == UIMirrorType.Quarter)
        {
            //判断三个点的水平位置是否在偶数矩形内，如果是，则把UV坐标水平翻转
            if (Mathf.FloorToInt((centerX - rect.xMin) / w) % 2 == 1)
            {
                v1.uv0 = GetOverturnUV(v1.uv0, inner.x, inner.z, true);
                v2.uv0 = GetOverturnUV(v2.uv0, inner.x, inner.z, true);
                v3.uv0 = GetOverturnUV(v3.uv0, inner.x, inner.z, true);
            }
        }

        if (m_MirrorType == UIMirrorType.Vertical || m_MirrorType == UIMirrorType.Quarter)
        {
            //判断三个点的垂直位置是否在偶数矩形内，如果是，则把UV坐标垂直翻转
            if (Mathf.FloorToInt((centerY - rect.yMin) / h) % 2 == 1)
            {
                v1.uv0 = GetOverturnUV(v1.uv0, inner.y, inner.w, false);
                v2.uv0 = GetOverturnUV(v2.uv0, inner.y, inner.w, false);
                v3.uv0 = GetOverturnUV(v3.uv0, inner.y, inner.w, false);
            }
        }

        verts[i * 3] = v1;
        verts[i * 3 + 1] = v2;
        verts[i * 3 + 2] = v3;
    }
}

protected void ExtendCapacity(List<UIVertex> verts, int addCount)
{
    var neededCapacity = verts.Count + addCount;
    if (verts.Capacity < neededCapacity)
    {
        verts.Capacity = neededCapacity;
    }
}

protected void SimpleScale(Rect rect, List<UIVertex> verts, int count)
{
    for (int i = 0; i < count; i++)
    {
        UIVertex vertex = verts[i];

        Vector3 position = vertex.position;

        if (m_MirrorType == UIMirrorType.Horizontal || m_MirrorType == UIMirrorType.Quarter)
        {
            position.x = (position.x + rect.x) * 0.5f;
        }

        if (m_MirrorType == UIMirrorType.Vertical || m_MirrorType == UIMirrorType.Quarter)
        {
            position.y = (position.y + rect.y) * 0.5f;
        }

        vertex.position = position;

        verts[i] = vertex;
    }
}

protected void SlicedScale(Rect rect, List<UIVertex> verts, int count)
{
    byte xBorderType = 1, yBorderType = 1;
    Vector4 border = (graphic as Image).overrideSprite.border;
    float borderWidth = border.x + border.z;
    float borderHeight = border.y + border.w;
    float halfWidth = rect.width * 0.5f;
    float halfHeight = rect.height * 0.5f;
    if (m_MirrorType == UIMirrorType.Horizontal || m_MirrorType == UIMirrorType.Quarter)
    {
        if (rect.width < borderWidth) xBorderType = 3;
        else if (halfWidth < borderWidth) xBorderType = 2;
    }
    if (m_MirrorType == UIMirrorType.Vertical || m_MirrorType == UIMirrorType.Quarter)
    {
        if (rect.height < borderHeight) yBorderType = 3;
        else if (halfHeight < borderHeight) yBorderType = 2;
    }

    if (m_MirrorType == UIMirrorType.Horizontal)
    {
        float rectLeft = rect.x + 0.9f;
        float borderLeft = rect.x + border.x + 0.9f;
        float borderOffsetW = borderWidth - halfWidth;
        for (int i = 0; i < count; i++)
        {
            UIVertex vertex = verts[i];
            Vector3 position = vertex.position;
            if (xBorderType == 1)
            {
                if (position.x > borderLeft) position.x -= halfWidth;
            }
            else if (xBorderType == 2)
            {
                if (position.x > borderLeft) position.x -= halfWidth;
                else if (position.x > rectLeft) position.x -= borderOffsetW;
            }
            else if (xBorderType == 3)
            {
                if (position.x > rectLeft) position.x -= halfWidth;
            }
            vertex.position = position;
            verts[i] = vertex;
        }
    }
    else if (m_MirrorType == UIMirrorType.Vertical)
    {
        float rectBottom = rect.y + 0.9f;
        float borderBottom = rect.y + border.y + 0.1f;
        float borderOffsetH = borderHeight - halfHeight;
        for (int i = 0; i < count; i++)
        {
            UIVertex vertex = verts[i];
            Vector3 position = vertex.position;
            if (yBorderType == 1)
            {
                if (position.y > borderBottom) position.y -= halfHeight;
            }
            else if (yBorderType == 2)
            {
                if (position.y > borderBottom) position.y -= halfHeight;
                else if (position.y > rectBottom) position.y -= borderOffsetH;
            }
            else if (yBorderType == 3)
            {
                if (position.y > rectBottom) position.y -= halfHeight;
            }
            vertex.position = position;
            verts[i] = vertex;
        }
    }
    else if (m_MirrorType == UIMirrorType.Quarter)
    {
        float rectLeft = rect.x + 0.9f;
        float rectBottom = rect.y + 0.9f;
        float borderLeft = rect.x + border.x + 0.1f;
        float borderBottom = rect.y + border.y + 0.1f;
        float borderOffsetW = borderWidth - halfWidth;
        float borderOffsetH = borderHeight - halfHeight;
        for (int i = 0; i < count; i++)
        {
            UIVertex vertex = verts[i];
            Vector3 position = vertex.position;
            if (xBorderType == 1)
            {
                if (position.x > borderLeft) position.x -= halfWidth;
            }
            else if (xBorderType == 2)
            {
                if (position.x > borderLeft) position.x -= halfWidth;
                else if (position.x > rectLeft) position.x -= borderOffsetW;
            }
            else if (xBorderType == 3)
            {
                if (position.x > rectLeft) position.x -= halfWidth;
            }
            if (yBorderType == 1)
            {
                if (position.y > borderBottom) position.y -= halfHeight;
            }
            else if (yBorderType == 2)
            {
                if (position.y > borderBottom) position.y -= halfHeight;
                else if (position.y > rectBottom) position.y -= borderOffsetH;
            }
            else if (yBorderType == 3)
            {
                if (position.y > rectBottom) position.y -= halfHeight;
            }
            vertex.position = position;
            verts[i] = vertex;
        }
    }
}

protected void MirrorVerts(Rect rect, List<UIVertex> verts, int count, bool isHorizontal = true)
{
    for (int i = 0; i < count; i++)
    {
        UIVertex vertex = verts[i];

        Vector3 position = vertex.position;

        if (isHorizontal)
        {
            position.x = rect.center.x * 2 - position.x;
        }
        else
        {
            position.y = rect.center.y * 2 - position.y;
        }

        vertex.position = position;

        verts.Add(vertex);
    }
}

protected int SliceExcludeVerts(List<UIVertex> verts, int count)
{
    int realCount = count;

    int i = 0;

    while (i < realCount)
    {
        UIVertex v1 = verts[i];
        UIVertex v2 = verts[i + 1];
        UIVertex v3 = verts[i + 2];

        if (v1.position == v2.position || v2.position == v3.position || v3.position == v1.position)
        {
            verts[i] = verts[realCount - 3];
            verts[i + 1] = verts[realCount - 2];
            verts[i + 2] = verts[realCount - 1];

            realCount -= 3;
            continue;
        }

        i += 3;
    }

    if (realCount < count)
    {
        verts.RemoveRange(realCount, count - realCount);
    }

    return realCount;
}

protected Vector4 GetAdjustedBorders(Rect rect)
{
    Sprite overrideSprite = (graphic as Image).overrideSprite;

    Vector4 border = overrideSprite.border;

    border = border / (graphic as Image).pixelsPerUnit;

    for (int axis = 0; axis <= 1; axis++)
    {
        float combinedBorders = border[axis] + border[axis + 2];
        if (rect.size[axis] < combinedBorders && combinedBorders != 0)
        {
            float borderScaleRatio = rect.size[axis] / combinedBorders;
            border[axis] *= borderScaleRatio;
            border[axis + 2] *= borderScaleRatio;
        }
    }

    return border;
}

protected float GetCenter(float p1, float p2, float p3)
{
    float max = Mathf.Max(Mathf.Max(p1, p2), p3);

    float min = Mathf.Min(Mathf.Min(p1, p2), p3);

    return (max + min) / 2;
}

protected Vector2 GetOverturnUV(Vector2 uv, float start, float end, bool isHorizontal = true)
{
    if (isHorizontal)
    {
        uv.x = end - uv.x + start;
    }
    else
    {
        uv.y = end - uv.y + start;
    }

    return uv;
}

}


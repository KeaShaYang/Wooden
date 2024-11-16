using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShadowGamma : Shadow
{

#if UNITY_EDITOR
    protected override void Reset()
    {
        //base.Reset();

        // 美术那边说都是统一方向，统一距离的字体阴影
        effectDistance = new Vector2(0, -2);
    }
#endif

    protected static List<UIVertex> tmpUIVertexList = new List<UIVertex>();
    public override void ModifyMesh(VertexHelper vh)
    {
        if (!IsActive())
            return;
        vh.GetUIVertexStream(tmpUIVertexList);
        ApplyShadow(tmpUIVertexList, effectColor.gamma, 0, tmpUIVertexList.Count, effectDistance.x, effectDistance.y);
        vh.Clear();
        vh.AddUIVertexTriangleStream(tmpUIVertexList);
    }
}
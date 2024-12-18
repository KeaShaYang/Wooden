﻿using UnityEngine.UI;

public class EmptyImage : MaskableGraphic
{
    protected EmptyImage()
    {
        useLegacyMeshGeneration = false;
    }
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
    }
}

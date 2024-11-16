using System;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
[RequireComponent(typeof(Image))]
public class ImageMaterialProperty : MonoBehaviour
{
    static int MaterialID_ColorFactor = Shader.PropertyToID("_ColorFactor");
    static int MaterialID_Tint = Shader.PropertyToID("_Color");

    public float colorFactor;
    public Color tint;
    private Material imgMat;

    private void Awake()
    {
        Image img = GetComponent<Image>();
        if (img)
        {
            imgMat = img.material;
            //拿到不是我们的自定义材质，不做处理
            if (imgMat == Image.defaultETC1GraphicMaterial || imgMat == Graphic.defaultGraphicMaterial)
                imgMat = null;
#if UNITY_EDITOR
            if (imgMat)
            {
                if (imgMat.HasProperty(MaterialID_ColorFactor))
                    colorFactor = imgMat.GetFloat(MaterialID_ColorFactor);
                if (imgMat.HasProperty(MaterialID_Tint))
                    tint = imgMat.GetColor(MaterialID_Tint);
            }
#endif
        }
    }

    private void Update()
    {
        if (!imgMat) return;
        if (imgMat.HasProperty(MaterialID_ColorFactor))
            imgMat.SetFloat(MaterialID_ColorFactor, colorFactor);
        if (imgMat.HasProperty(MaterialID_Tint))
            imgMat.SetColor(MaterialID_Tint, tint);
    }

#if UNITY_EDITOR
    private void OnDidApplyAnimationProperties()
    {
        if (!imgMat) return;
        if (imgMat.HasProperty(MaterialID_ColorFactor))
            imgMat.SetFloat(MaterialID_ColorFactor, colorFactor);
        if (imgMat.HasProperty(MaterialID_Tint))
            imgMat.SetColor(MaterialID_Tint, tint);
    }
#endif
}

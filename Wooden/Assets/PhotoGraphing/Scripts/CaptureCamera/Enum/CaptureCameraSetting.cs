using System;
using UnityEngine;

namespace MolSpace.PhotoGraphing
{
    [Serializable]
    public class CaptureCameraSetting
    {
        [Tooltip("相机显示内容")]
        public CameraClearFlags cameraClearFlag = CameraClearFlags.Depth;
        [Tooltip ("相机截图的layer层,需指定，并在layer层中添加")]
        public LayerMask cullingMask;
        [Tooltip("图片背景颜色")]
        public Color backgroundColor = Color.black;
        [Tooltip("截图的texture与原图的比例")]
        public float viewRate = 1;
        [Tooltip("相机视野")]
        public int clippingNear = -10;
        [Tooltip("相机视野")]
        public int clippingFar = 10;
        [Tooltip("相机显示内容")]
        public int cameraSize = 1;
        [Tooltip("相机渲染Texture")]
        public RenderTexture targetTexture;      
       

    }
}

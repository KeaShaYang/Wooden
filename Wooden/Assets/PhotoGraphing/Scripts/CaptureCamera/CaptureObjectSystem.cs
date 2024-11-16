using UnityEngine;
using BounderHelper;

namespace MolSpace.PhotoGraphing
{
    /// <summary>
    /// 单个模型截图系统
    /// 场景中有且只有一个此脚本
    /// 对应物体则为截图相机
    /// </summary>
    [RequireComponent(typeof(CreateCaptureCamera))]
    [RequireComponent(typeof(CameraPhotoGraphing))]
    public class CaptureObjectSystem : MonoSingleton<CaptureObjectSystem>
    {
        private CreateCaptureCamera createCaptureCameraScript;
        private CameraPhotoGraphing cameraPhotoGraphingScript;
        private new void Awake()
        {
            createCaptureCameraScript = GetComponent<CreateCaptureCamera>();
            cameraPhotoGraphingScript = GetComponent<CameraPhotoGraphing>();
        }

        /// <summary>
        /// 截图
        /// </summary>
        /// <param name="obj">被摄物体</param>
        public Texture2D Capture(GameObject obj)
        {                    
            LayerMask targetLayer = LayerNum(createCaptureCameraScript.m_setting.cullingMask.value);
            LayerRecord.ChangeAllLayer(obj.transform, targetLayer);
            Texture2D objTxture = cameraPhotoGraphingScript.CaptureCamera(obj);
            LayerRecord.RecoverAllLayer(obj.transform);
            return objTxture;
        }
        #region 私有方法，辅助计算layer
        /// <summary>
        /// 计算cullingMask对应的layermask的int值
        /// </summary>
        /// <param name="num">CullingMask的value值</param>
        /// <returns>返回log2（value）</returns>
        private int LayerNum(int num)
        {
            int layerNum = 0;
            if (num <= 1) return layerNum;
            while (num /2 >0)
            {
                num = num / 2;
                layerNum++;
            }
            return layerNum;
        }
        #endregion
    }
}

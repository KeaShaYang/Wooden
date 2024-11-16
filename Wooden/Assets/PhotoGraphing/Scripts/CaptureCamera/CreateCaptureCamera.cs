using UnityEngine;
using BounderHelper;

namespace MolSpace.PhotoGraphing
{
    public class CreateCaptureCamera : MonoSingleton<CreateCaptureCamera>
    {
        public CaptureCameraSetting m_setting = new CaptureCameraSetting();
        Camera m_cam;
        // Use this for initialization
        private void Start()
        {
            m_cam = this.gameObject.AddComponent<Camera>();
            SetCamOnce();
        }

        /// <summary>
        /// 第一次创建相机时调用
        /// </summary>
        private void SetCamOnce()
        {
            m_cam.orthographic = true;
            m_cam.clearFlags = m_setting.cameraClearFlag;
            m_cam.cullingMask = m_setting.cullingMask;
            m_cam.backgroundColor = m_setting.backgroundColor;
            m_cam.targetTexture = m_setting.targetTexture;
        }

        /// <summary>
        ///  在instance被调用时调用
        /// </summary>
        public static int count = 0;
        public override void Init()
        {
            base.Init();
            count++;
            if (m_cam == null)
                m_cam = GetComponent<Camera>();
            // m_cam.enabled = true;
            gameObject.SetActive(true);
        }
        /// <summary>
        /// 重置相机位置,大小,Camera.SetActive(true)
        /// </summary>
        /// <param name="photoObj"></param>
        public Camera CreateCamera(GameObject photoObj)
        {
            gameObject.transform.position = photoObj.transform.position;
            BounderHelper.Bounds objBound = CalculateBounder.ComputeObjBounds(photoObj.transform);
            gameObject.transform.position = objBound.center;
            float scaleX = objBound.size.x;
            float scaleY = objBound.size.y;
            float maxScale = scaleX > scaleY ? scaleX : scaleY;
            m_cam.orthographicSize =  maxScale * m_setting.cameraSize *  0.4f;
            m_cam.nearClipPlane = m_setting.clippingNear * m_setting.viewRate * objBound.size.z;
            m_cam.farClipPlane = m_setting.clippingFar * m_setting.viewRate * objBound.size.z ;
            return m_cam;
        }
        public void HideCaptureCamera()
        {
            // m_cam.enabled = false;
            gameObject.SetActive(false);
        }
    }
}

using UnityEngine;

namespace MolSpace.PhotoGraphing
{
    /// <summary>
    /// 在被摄物体上，当单机鼠标左键时对物体进行一次拍照，且只进行一次
    /// </summary>
    public class BeenShot : MonoBehaviour
    {
        [Tooltip("物体是否已经被截图过")]
        [SerializeField]
        private bool isCapture = false;
        // Use this for initialization
        private void Start()
        {
        }
        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                PhotoGraphing();
            }
        }

        private void PhotoGraphing()
        {
            if (!isCapture)
            {
                CaptureObjectSystem.Instance.Capture(this.gameObject);
                isCapture = true;
            }
        }
    }
}

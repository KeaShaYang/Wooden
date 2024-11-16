using System.IO;
using UnityEngine;
using FilePathTool;

namespace MolSpace.PhotoGraphing
{
    /// <summary>
    /// 截图
    /// </summary>
    public class CameraPhotoGraphing : MonoBehaviour
    {

        private float textureRate = 0.5f;
        private RenderTexture camTargetTexture;
        private readonly string today = System.DateTime.Today.ToString().Substring(0, 10).Replace("/", "");
        private GameObject camObj;
        private Camera cam;
        public int photoWidth = 512;
        public int photoHeight = 512;
        public Texture2D CaptureCamera(GameObject viewObj)
        {
            //生成相机
            // camObj = Resources.Load("CaptureCamera")as GameObject;
            // Camera cam = Instantiate(camObj).GetComponent<Camera>();
            string defaultFilePath = Application.dataPath + "/Pictures/" + today + "/" + viewObj.name + ".png";
            Texture2D objTexture = CaptureCamera(viewObj, defaultFilePath);
            return objTexture;
        }
        public Texture2D CaptureCamera(GameObject viewObj, string url)
        {
            return CaptureCamera(viewObj, viewObj.name, url);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewObj">被摄物体</param>
        /// <param name="fileName">文件名（不带后缀）</param>
        /// <param name="url">文件存储路径</param>
        /// <returns></returns>
        public Texture2D CaptureCamera(GameObject viewObj, string fileName, string url)
        {
            //生成相机
            // camObj = Resources.Load("CaptureCamera") as GameObject;
            // Camera cam = Instantiate(camObj).GetComponent<Camera>();
            cam = CreateCaptureCamera.Instance.CreateCamera(viewObj);
            return CaptureCamera(cam, new Rect(0, 0, photoWidth, photoHeight), viewObj.name, url);
        }
        /// <summary>  
        /// 对相机截图。   
        /// </summary>  
        /// <returns>The screenshot2.</returns>  
        /// <param name="camera">Camera.要被截屏的相机</param>  
        /// <param name="rect">Rect.截屏的区域</param>  
        private Texture2D CaptureCamera(Camera camera, Rect rect, string fileName, string url)
        {
            camTargetTexture = new RenderTexture((int)( rect.width ), (int)( rect.height), 24);
            camera.targetTexture = camTargetTexture;
            camera.Render();
            RenderTexture.active = camTargetTexture;
            Texture2D screenShot = new Texture2D((int)(rect.width), (int)(rect.height), TextureFormat.RGB24, false);
            screenShot.ReadPixels(rect, 0, 0);
            // 注：这个时候，它是从RenderTexture.active中读取像素  
            screenShot.Apply();
            camera.targetTexture = null;
            RenderTexture.active = null;
            byte[] bytes = screenShot.EncodeToPNG();
            //图片存储路径——按天新建文件夹
            string filepath = url;
            //string newFilepath = RepathFile(filepath,ref fileName);
            string newFilePath = null;
            FilePathFactory newFile = new FilePathFactory();
            newFilePath = newFile.CreateOrOpenFile(filepath, fileName);
            File.WriteAllBytes(newFilePath, bytes);
            CreateCaptureCamera.Instance.HideCaptureCamera();
            return screenShot;
        }

    }
}

/*
作者：张阳
说明：工具通用函数
日期：2019-10-23
*/

using UnityEngine;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using System.Diagnostics;
using System.Security.Cryptography;
using UnityEditor.SceneManagement;
using System.Linq;

namespace AMLD.Effect.Editor
{
    public class UtilTool
    {
        /// <summary>
        /// 获取路径
        /// </summary>
        public static string GetPath(GameObject Obj, Transform Par = null)
        {
            List<string> vName = new List<string>();
            vName.Add(Obj.name);
            Transform parent = Obj.transform.parent;
            while (parent != Par)
            {
                vName.Add(parent.gameObject.name);
                parent = parent.parent;
            }
            string strPath = "";
            for (int i = vName.Count - 1; i >= 0; i--)
            {
                strPath += vName[i];
                if (i > 0)
                {
                    strPath += "/";
                }
            }
            return strPath;
        }

        /// <summary>
        /// 根据旧的子物体路径获取新物体
        /// </summary>
        public static GameObject GetNewChild(GameObject newPre, GameObject oldPre, GameObject childPre)
        {
            string strPath = GetPath(childPre, oldPre.transform);
            Transform child = newPre.transform.Find(strPath);
            if (child != null)
            {
                return child.gameObject;
            }
            else
            {
                UnityEngine.Debug.LogWarning("找不到新物体上的子物体：" + strPath);
                return null;
            }
        }


        /// <summary>
        /// 文件是否存在
        /// </summary>
        public static bool IsFileExists(string path)
        {
            return File.Exists(path);
        }

        /// <summary>
        /// 获取文件列表
        /// </summary>
        public static List<string> GetFileList(string strPath, string strEx, bool bRelPath, string strPattern = "*", string filter = "")
        {
            List<string> vPath = new List<string>();
            string serchPath = "";
            if (bRelPath)
            {
                serchPath = Application.dataPath + "/" + strPath;
            }
            else
            {
                serchPath = strPath;
            }
            if (Directory.Exists(serchPath))
            {
                string[] allFiles = Directory.GetFiles(serchPath, strPattern + strEx, SearchOption.AllDirectories);
                for (int i = 0; i < allFiles.Length; i++)
                {
                    if (!string.IsNullOrEmpty(filter))
                    {
                        if (allFiles[i].Contains(filter))
                        {
                            continue;
                        }
                    }

                    if (allFiles[i].EndsWith(strEx))
                    {
                        string str = allFiles[i].Replace("\\", "/");
                        if (bRelPath)
                        {
                            str = str.Replace(Application.dataPath, "Assets");
                            vPath.Add(str);
                        }
                        else
                        {
                            vPath.Add(str);
                        }
                    }
                }
            }
            return vPath;
        }

        /// <summary>
        /// 获取文件夹列表
        /// </summary>
        public static List<string> GetDirectoryList(string strPath, string strEx, bool bRelPath, string strPattern = "*")
        {
            List<string> vPath = new List<string>();
            string serchPath = "";
            if (bRelPath)
            {
                serchPath = Application.dataPath + "/" + strPath;
            }
            else
            {
                serchPath = strPath;
            }
            if (Directory.Exists(serchPath))
            {
                string[] allFiles = Directory.GetDirectories(serchPath, strPattern + strEx, SearchOption.AllDirectories);
                for (int i = 0; i < allFiles.Length; i++)
                {
                    if (allFiles[i].EndsWith(strEx))
                    {
                        string str = allFiles[i].Replace("\\", "/");
                        if (bRelPath)
                        {
                            str = str.Replace(Application.dataPath, "Assets");
                            vPath.Add(str);
                        }
                        else
                        {
                            vPath.Add(str);
                        }
                    }
                }
            }
            return vPath;
        }


        /// <summary>
        /// 保存预制
        /// </summary>
        public static void SavePrefab(Object Prefab)
        {
            string strPath = AssetDatabase.GetAssetPath(Prefab);
            //先检查路径
            string allPath = strPath.Substring(0, strPath.LastIndexOf('/'));
            //判断路径是否合法
            if (!Directory.Exists(allPath))
            {
                //如果目录不存在就要创建
                Directory.CreateDirectory(allPath);
            }
            Object loadObject = PrefabUtility.InstantiateAttachedAsset(Prefab);
            GameObject GameObj = (GameObject)loadObject;
            PrefabUtility.ReplacePrefab(GameObj, Prefab);
            GameObject.DestroyImmediate(loadObject);
        }

        /// <summary>
        /// 保存预制
        /// </summary>
        public static void SavePrefabPath(GameObject Obj, string Path)
        {
            //先检查路径
            string allPath = Path.Substring(0, Path.LastIndexOf('/'));
            //判断路径是否合法
            if (!Directory.Exists(allPath))
            {
                //如果目录不存在就要创建
                Directory.CreateDirectory(allPath);
            }
            GameObject Pre = PrefabUtility.CreatePrefab(Path, Obj);
            PrefabUtility.ReplacePrefab(Obj, Pre, ReplacePrefabOptions.ConnectToPrefab);
        }

        /// <summary>
        /// 保存预制
        /// </summary>
        public static void SavePrefabByObj(Object Prefab, GameObject Obj)
        {
            string strPath = AssetDatabase.GetAssetPath(Prefab);
            //先检查路径
            string allPath = strPath.Substring(0, strPath.LastIndexOf('/'));
            //判断路径是否合法
            if (!Directory.Exists(allPath))
            {
                //如果目录不存在就要创建
                Directory.CreateDirectory(allPath);
            }
            PrefabUtility.ReplacePrefab(Obj, Prefab);
        }

        /// <summary>
        /// 加载资源
        /// </summary>
        public static Object LoadAsset(string strPath)
        {
            Object loadObject = AssetDatabase.LoadMainAssetAtPath(strPath);
            return loadObject;
        }

        /// <summary>
        /// 创建资源
        /// </summary>
        public static void CreteAssets(Object obj, string strPath)
        {
            AssetDatabase.CreateAsset(obj, strPath);
        }

        /// <summary>
        /// 复制资源
        /// </summary>
        public static GameObject InstantiateObj(Object Obj)
        {
            GameObject loadObject = (GameObject)PrefabUtility.InstantiateAttachedAsset(Obj);
            return loadObject;
        }

        /// <summary>
        /// 复制资源
        /// </summary>
        public static GameObject LoadInstantiateObj(string strPath)
        {
            GameObject obj = AssetDatabase.LoadMainAssetAtPath(strPath) as GameObject;
            if (obj != null)
            {
                GameObject go = GameObject.Instantiate<GameObject>(obj);
                go.name = obj.name;
                return go;
            }
            return null;
        }




        /// <summary>
        /// 复制资源2
        /// </summary>
        public static GameObject InstantiateObj2(Object Obj)
        {
            GameObject loadObject = (GameObject)PrefabUtility.InstantiatePrefab(Obj);
            return loadObject;
        }

        /// <summary>
        /// 是否是根物体
        /// </summary>
        public static bool IsObjRoot(GameObject root, GameObject child)
        {
            if (child != null && root != null)
            {
                Transform roottan = root.transform;
                Transform parent = child.transform;
                while (parent != null && parent != roottan)
                {
                    parent = parent.parent;
                }
                return parent == roottan;
            }
            return false;
        }

        /// <summary>
        /// 获取文件列表
        /// </summary>
        public static List<string> GetFilePathList(string strPath, string strEx)
        {
            List<string> vPath = new List<string>();
            string[] allFiles = Directory.GetFiles(strPath, "*" + strEx + "*", SearchOption.AllDirectories);
            for (int i = 0; i < allFiles.Length; i++)
            {
                if (allFiles[i].EndsWith(strEx))
                {
                    string str = allFiles[i].Replace("\\", "/");
                    vPath.Add(str);
                }
            }
            return vPath;
        }

        /// <summary>
        /// 保存文本文件
        /// </summary>
        public static void SaveFileText(string strPath, List<string> vText, bool bAppend, int iStartRow = 0, bool bFullPath = false)
        {
            if (vText.Count > 0)
            {
                string txtPath = "";
                if (bFullPath)
                {
                    txtPath = strPath;
                }
                else
                {
                    txtPath = Application.dataPath + "/" + strPath;
                }
                StreamWriter sw = null;
                if (!File.Exists(txtPath))
                {
                    sw = File.CreateText(txtPath);
                }
                else
                {
                    sw = new StreamWriter(txtPath, bAppend);
                }
                for (int i = 0; i < iStartRow; i++)
                {
                    sw.WriteLine("");
                }
                for (int i = 0; i < vText.Count; i++)
                {
                    sw.WriteLine(vText[i]);
                }
                sw.Close();
            }
        }

        /// <summary>
        /// 查找路径
        /// </summary>
        public static string FindPath(GameObject Obj, GameObject Root)
        {
            List<string> vName = new List<string>();
            vName.Add(Obj.name);
            Transform parent = Obj.transform.parent;
            while (parent != null && parent != Root.transform)
            {
                vName.Add(parent.gameObject.name);
                parent = parent.parent;
            }
            string strPath = "";
            for (int i = vName.Count - 1; i >= 0; i--)
            {
                strPath += vName[i];
                if (i > 0)
                {
                    strPath += "/";
                }
            }
            return strPath;
        }

        /// <summary>
        /// 保存资源
        /// </summary>
        public static void SaveAssets()
        {
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();

        }

        /// <summary>
        /// 获取文件名
        /// </summary>
        public static string GetNameByPath(string strPath, bool bExt = false)
        {
            string[] vPath = strPath.Split('/');
            string strName = vPath[vPath.Length - 1];
            if (!bExt)
            {
                int ipos = strName.LastIndexOf('.');
                if (ipos > 0)
                {
                    strName = strName.Substring(0, ipos);
                }
            }
            return strName;
        }

        /// <summary>
        /// 获取文件名
        /// </summary>
        public static string GetExByPath(string strPath)
        {
            int ipos = strPath.LastIndexOf('.');
            if (ipos > 0)
            {
                return strPath.Substring(ipos);
            }
            return "";
        }

        /// <summary>
        /// 获取文件夹路径
        /// </summary>
        public static string GetPathDir(string strPath, int UpNum = 1)
        {
            strPath = strPath.Replace("\\", "/");
            string[] vPath = strPath.Split('/');
            string strDir = "";
            if (UpNum < vPath.Length)
            {
                for (int i = 0; i < vPath.Length - UpNum; i++)
                {
                    strDir += vPath[i] + "/";
                }
            }
            return strDir;
        }

        /// <summary>
        /// 获取文件夹路径
        /// </summary>
        public static string GetPathName(string strPath, int UpNum = 1)
        {
            strPath = strPath.Replace("\\", "/");
            string[] vPath = strPath.Split('/');
            string strDir = "";
            if (UpNum < vPath.Length)
            {
                return vPath[vPath.Length - UpNum];
            }
            return "";
        }

        /// <summary>
        /// 路径名添加后缀
        /// </summary>
        public static string AddPathEnd(string strPath, string strEnd)
        {
            strPath = strPath.Replace("\\", "/");
            string strdir = GetPathDir(strPath);
            string strName = GetNameByPath(strPath, false);
            string strEx = GetExByPath(strPath);
            string str = strdir + strName + strEnd + strEx;
            return str;
        }

        /// <summary>
        /// 复制文件
        /// </summary>
        public static void CopyFile(string sourcePath, string destinationPath, bool bRelPath = false)
        {
            if (bRelPath)
            {
                sourcePath = sourcePath.Replace("Assets/", Application.dataPath + "/");
                destinationPath = destinationPath.Replace("Assets/", Application.dataPath + "/");
            }
            if (File.Exists(sourcePath))
            {
                string strPath = Path.GetDirectoryName(destinationPath);
                if (!Directory.Exists(strPath))
                {
                    Directory.CreateDirectory(strPath);
                }
                File.Copy(sourcePath, destinationPath, true);
            }
        }

        /// <summary>
        /// 删除文件夹
        /// </summary>
        public static void DeleteFile(string strParh)
        {
            if (File.Exists(strParh))
            {
                File.Delete(strParh);
            }
        }

        /// <summary>
        /// 删除文件夹
        /// </summary>
        public static void DeleteDirectory(string dir)
        {
            DirectoryInfo Dc = new DirectoryInfo(dir);
            if (Dc.Exists)
            {
                DirectoryInfo[] childs = Dc.GetDirectories();
                if (childs != null && childs.Length > 0)
                {
                    for (int i = 0; i < childs.Length; i++)
                    {
                        childs[i].Delete(true);
                    }
                }
                Dc.Delete(true);
            }
        }

        /// <summary>
        /// 创建目录
        /// </summary>
        public static void CreateDirectory(string dir)
        {
            DirectoryInfo Dc = new DirectoryInfo(dir);
            if (!Dc.Exists)
            {
                Directory.CreateDirectory(dir);
            }
        }

        /// <summary>
        /// 拷贝文件夹
        /// </summary>
        public static void CopyDirectory(string sourcePath, string destinationPath)
        {
            try
            {
                if (destinationPath[destinationPath.Length - 1] != Path.DirectorySeparatorChar)
                {
                    destinationPath += System.IO.Path.DirectorySeparatorChar;
                }
                if (!Directory.Exists(destinationPath))
                {
                    Directory.CreateDirectory(destinationPath);
                }
                string[] filelist = Directory.GetFileSystemEntries(sourcePath);
                foreach (string file in filelist)
                {
                    if (Directory.Exists(file))
                    {
                        CopyDirectory(file, destinationPath + Path.GetFileName(file));
                    }
                    else
                    {
                        File.Copy(file, destinationPath + Path.GetFileName(file), true);
                    }
                }
            }
            catch (System.Exception e)
            {
                throw;
            }
        }

        /// <summary>
        /// 运行程序
        /// </summary>
        public static void RunCmd(string cmdExe, string cmdStr, bool bWait)
        {
            try
            {
                using (Process myPro = new Process())
                {
                    myPro.StartInfo.FileName = cmdExe;
                    myPro.StartInfo.Arguments = cmdStr;
                    myPro.Start();
                    if (bWait)
                    {
                        myPro.WaitForExit();
                    }
                }
            }
            catch
            {

            }
        }

        /// <summary>
        /// 获取查找文件列表
        /// </summary>
        public static void GetFindNameList(string strName, List<string> vl)
        {
            vl.Clear();
            if (!string.IsNullOrEmpty(strName))
            {
                string str = strName.Replace(" ", "");
                str = str.Replace("\\n", "");
                str = str.Replace("\n", "");
                str = str.Replace("\\r", "");
                str = str.Replace("\r", "");
                string[] vn = str.Split('、');
                for (int i = 0; i < vn.Length; i++)
                {
                    vl.Add(vn[i]);
                }
            }
        }

        /// <summary>
        /// 连接字符串
        /// </summary>
        public static string JoinString(string[] str, string Separator)
        {
            string strJoin = "";
            if (str != null && str.Length > 0)
            {
                for (int i = 0; i < str.Length; i++)
                {
                    strJoin += str[i];
                    if (i < str.Length - 1)
                    {
                        strJoin += Separator;
                    }
                }
            }
            return strJoin;
        }

        /// <summary>
        /// 获取当前场景名称
        /// </summary>
        public static string GetCurSceneName()
        {
            UnityEngine.SceneManagement.Scene sc = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            if (sc != null)
            {
                return sc.name;
            }
            return "";
        }

        /// <summary>
        /// 获取当前场景路径
        /// </summary>
        public static string GetCurScenePath()
        {
            UnityEngine.SceneManagement.Scene sc = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            if (sc != null)
            {
                return sc.path;
            }
            return "";
        }

        /// <summary>
        /// 获取当前场景物体
        /// </summary>
        public static List<GameObject> GetCurSceneObj()
        {
            List<GameObject> root = new List<GameObject>();
            UnityEngine.SceneManagement.Scene sc = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            if (sc != null)
            {
                GameObject[] obj = sc.GetRootGameObjects();
                if (obj != null && obj.Length > 0)
                {
                    root.AddRange(obj);
                }
            }
            return root;
        }

        /// <summary>
        ///保存场景
        /// </summary>
        public static void SaveScene()
        {
            UnityEngine.SceneManagement.Scene sc = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            if (sc != null)
            {
                EditorSceneManager.SaveScene(sc);
            }
        }

        /// <summary>
        /// 设置场景有变化
        /// </summary>
        public static void SetSceneDirty()
        {
            if (!Application.isPlaying)
            {
                UnityEngine.SceneManagement.Scene sc = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
                if (sc != null)
                {
                    EditorSceneManager.MarkSceneDirty(sc);
                }
            }
        }

        /// <summary>
        /// 设置贴图可读写
        /// </summary>
        public static void SetTexReadEnable(Texture tex, bool bReadable)
        {
            if (tex != null && tex.isReadable != bReadable)
            {
                string path = AssetDatabase.GetAssetPath(tex);
                TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
                if (textureImporter != null)
                {
                    textureImporter.isReadable = bReadable;
                    AssetDatabase.ImportAsset(path);
                }
            }
        }

        /// <summary>
        /// 设置贴图sRGB
        /// </summary>
        public static void SetTexReadsRGB(string strPath, bool sRGB)
        {
            if (!string.IsNullOrEmpty(strPath))
            {
                TextureImporter textureImporter = AssetImporter.GetAtPath(strPath) as TextureImporter;
                if (textureImporter != null && textureImporter.sRGBTexture != sRGB)
                {
                    textureImporter.sRGBTexture = sRGB;
                    AssetDatabase.ImportAsset(strPath);
                }
            }
        }


        public static void SetTexCompressedQuality(Texture tex, TextureImporterCompression compression)
        {
            if (tex != null)
            {
                string path = AssetDatabase.GetAssetPath(tex);
                SetTexCompressedQuality(path, compression);
            }
        }


        public static void SetTexCompressedQuality(string strPath, TextureImporterCompression compression)
        {
            if (!string.IsNullOrEmpty(strPath))
            {
                TextureImporter textureImporter = AssetImporter.GetAtPath(strPath) as TextureImporter;
                if (textureImporter != null && textureImporter.textureCompression != compression)
                {
                    textureImporter.textureCompression = compression;
                    AssetDatabase.ImportAsset(strPath);
                }
            }
        }

        /// <summary>
        /// 设置贴图最大分辨率
        /// </summary>
        /// <returns>旧的分辨率</returns>
        public static int SetTexMaxSize(Texture tex, int newMaxSize)
        {
            if (tex != null)
            {
                string path = AssetDatabase.GetAssetPath(tex);
                return SetTexMaxSize(path, newMaxSize);
            }
            return 2048;
        }

        /// <summary>
        /// 设置贴图最大分辨率
        /// </summary>
        /// <returns>旧的分辨率</returns>
        public static int SetTexMaxSize(string strPath, int newMaxSize)
        {
            if (!string.IsNullOrEmpty(strPath))
            {
                TextureImporter textureImporter = AssetImporter.GetAtPath(strPath) as TextureImporter;
                if (textureImporter != null)
                {
                    int oldMaxSize = textureImporter.maxTextureSize;
                    if (textureImporter.maxTextureSize != newMaxSize)
                    {
                        textureImporter.maxTextureSize = newMaxSize;
                        AssetDatabase.ImportAsset(strPath);
                    }
                    return oldMaxSize;
                }
            }
            return 2048;
        }

        /// <summary>
        /// 裁切纹理
        /// </summary>
        public static void CutTex(Texture2D tex, int x, int y, int width, int height)
        {
            string path = AssetDatabase.GetAssetPath(tex);
            TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
            if (textureImporter != null)
            {
                bool oldReadable = textureImporter.isReadable;
                var oldSettings = textureImporter.GetDefaultPlatformTextureSettings();

                textureImporter.isReadable = true;
                TextureImporterPlatformSettings tp = new TextureImporterPlatformSettings();
                tp.format = TextureImporterFormat.RGBA32;
                tp.textureCompression = TextureImporterCompression.Uncompressed;
                textureImporter.SetPlatformTextureSettings(tp);
                textureImporter.SaveAndReimport();

                var cols = tex.GetPixels(x, y, width, height);
                Texture2D newTex = new Texture2D(width, height);
                newTex.SetPixels(cols);
                newTex.Apply();

                byte[] bytes = newTex.EncodeToPNG();
                File.WriteAllBytes(path, bytes);

                textureImporter.isReadable = oldReadable;
                textureImporter.SetPlatformTextureSettings(oldSettings);
                textureImporter.SaveAndReimport();
            }
        }

        /// <summary>
        /// 移除贴图Alpha通道
        /// </summary>
        public static void RemoveTexAlphaChannel(Texture2D tex)
        {
            string path = AssetDatabase.GetAssetPath(tex);
            TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
            if (textureImporter != null)
            {
                bool oldReadable = textureImporter.isReadable;
                var oldSettings = textureImporter.GetDefaultPlatformTextureSettings();

                textureImporter.isReadable = true;
                TextureImporterPlatformSettings tp = new TextureImporterPlatformSettings();
                tp.format = TextureImporterFormat.RGBA32;
                tp.textureCompression = TextureImporterCompression.Uncompressed;
                textureImporter.SetPlatformTextureSettings(tp);
                textureImporter.SaveAndReimport();

                var cols = tex.GetPixels();
                for (int i = 0; i < cols.Length; i++)
                {
                    cols[i].a = 1;
                }
                tex.SetPixels(cols);
                tex.Apply();

                byte[] bytes = tex.EncodeToPNG();
                File.WriteAllBytes(path, bytes);

                textureImporter.isReadable = oldReadable;
                textureImporter.SetPlatformTextureSettings(oldSettings);
                textureImporter.SaveAndReimport();
            }
        }

        /// <summary>
        /// 设置网格可读写
        /// </summary>
        public static void SetMeshMeshEnable(Mesh ms, bool bReadable)
        {
            string path = AssetDatabase.GetAssetPath(ms);
            if (!path.EndsWith(".mesh"))
            {
                ModelImporter modelImporter = AssetImporter.GetAtPath(path) as ModelImporter;
                if (modelImporter != null && modelImporter.isReadable != bReadable)
                {
                    modelImporter.isReadable = bReadable;
                    AssetDatabase.ImportAsset(path);
                }
            }
        }

        /// <summary>
        /// 设置贴图类型
        /// </summary>
        public static void SetTexType(Texture tex, TextureImporterType TexType)
        {
            string path = AssetDatabase.GetAssetPath(tex);
            TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
            if (textureImporter.textureType != TexType)
            {
                textureImporter.textureType = TexType;
                AssetDatabase.ImportAsset(path);
            }
        }

        /// <summary>
        /// 设置贴图类型
        /// </summary>
        public static void SetTexShape(Texture tex, TextureImporterShape TexShape)
        {
            string path = AssetDatabase.GetAssetPath(tex);
            TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
            if (textureImporter.textureShape != TexShape)
            {
                textureImporter.textureShape = TexShape;
                AssetDatabase.ImportAsset(path);
            }
        }

        /// <summary>
        /// 设置贴图类型
        /// </summary>
        public static void SetTexsRGB(Texture tex, bool sRGB)
        {
            string path = AssetDatabase.GetAssetPath(tex);
            TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
            if (textureImporter.sRGBTexture != sRGB)
            {
                textureImporter.sRGBTexture = sRGB;
                AssetDatabase.ImportAsset(path);
            }
        }

        /// <summary>
        /// 获取md5码
        /// </summary>
        public static string GetMD5(string str)
        {
            MD5 md = MD5.Create();
            byte[] bt = md.ComputeHash(System.Text.Encoding.UTF8.GetBytes(str));
            string strMd5 = System.BitConverter.ToString(bt);
            strMd5 = strMd5.Replace("-", "");
            return strMd5.ToLower();
        }

        /// <summary>
        /// 缩放贴图
        /// </summary>
        public static Texture2D ScaleTexture(Texture2D source, int iWidth, int iHeight, bool bHDR = false)
        {
            RenderTexture rt = null;
            if (bHDR)
            {
                rt = RenderTexture.GetTemporary(iWidth, iHeight, 0, RenderTextureFormat.ARGBFloat);
            }
            else
            {
                rt = RenderTexture.GetTemporary(iWidth, iHeight, 0, RenderTextureFormat.ARGB32);
            }
            Graphics.Blit(source, rt);
            RenderTexture.active = rt;
            Texture2D tex = null;
            if (bHDR)
            {
                tex = new Texture2D(iWidth, iHeight, TextureFormat.RGBAFloat, false);
            }
            else
            {
                tex = new Texture2D(iWidth, iHeight, TextureFormat.RGBA32, false);
            }
            tex.ReadPixels(new Rect(0, 0, iWidth, iHeight), 0, 0);
            tex.Apply();
            RenderTexture.ReleaseTemporary(rt);
            return tex;
        }

        /// <summary>
        /// 贴图属性是否相等
        /// </summary>
        public static bool IsTexSame(Material sm1, Material sm2, string strName, bool Tex)
        {
            float fmin = 0.001f;
            Vector2 sc1 = sm1.GetTextureScale(strName);
            Vector2 sc2 = sm2.GetTextureScale(strName);
            if (Mathf.Abs(sc1.x - sc2.y) < fmin && Mathf.Abs(sc1.y - sc2.y) < fmin)
            {
                Vector2 sf1 = sm1.GetTextureOffset(strName);
                Vector2 sf2 = sm2.GetTextureOffset(strName);
                if (Mathf.Abs(sf1.x - sf2.y) < fmin && Mathf.Abs(sf1.y - sf2.y) < fmin)
                {
                    if (Tex)
                    {
                        Texture te1 = sm1.GetTexture(strName);
                        Texture te2 = sm2.GetTexture(strName);
                        if (te1 == te2)
                        {
                            return true;
                        }
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            return false;
        }


        /// <summary>
        /// 颜色是否相等
        /// </summary>
        public static bool IsColorSame(Material sm1, Material sm2, string strName)
        {
            float fmin = 0.001f;
            Color sc1 = sm1.GetColor(strName);
            Color sc2 = sm2.GetColor(strName);
            if (Mathf.Abs(sc1.r - sc2.r) < fmin && Mathf.Abs(sc1.g - sc2.g) < fmin
                && Mathf.Abs(sc1.b - sc2.b) < fmin && Mathf.Abs(sc1.a - sc2.a) < fmin)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 值是否相等
        /// </summary>
        public static bool IsMatFloatSame(Material sm1, Material sm2, string strName)
        {
            float fmin = 0.1f;
            float sc1 = sm1.GetFloat(strName);
            float sc2 = sm2.GetFloat(strName);
            if (Mathf.Abs(sc1 - sc2) < fmin)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 值是否相等
        /// </summary>
        public static bool IsMatFloatValueSame(float f1, float f2)
        {
            float fmin = 0.1f;
            if (Mathf.Abs(f1 - f2) < fmin)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 值是否相等
        /// </summary>
        public static bool IsFloatSame(Material sm1, Material sm2, string strName)
        {
            float fmin = 0.001f;
            float sc1 = sm1.GetFloat(strName);
            float sc2 = sm2.GetFloat(strName);
            if (Mathf.Abs(sc1 - sc2) < fmin)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 值是否相等
        /// </summary>
        public static bool IsFloatValueSame(float f1, float f2)
        {
            float fmin = 0.001f;
            if (Mathf.Abs(f1 - f2) < fmin)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 值是否相等
        /// </summary>
        public static bool IsVector3ValueSame(Vector3 f1, Vector3 f2)
        {
            float fmin = 0.001f;
            if (Mathf.Abs(f1.x - f2.x) > fmin || Mathf.Abs(f1.y - f2.y) > fmin || Mathf.Abs(f1.z - f2.z) > fmin)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 值是否相等
        /// </summary>
        public static bool IsVector2ValueSame(Vector2 f1, Vector2 f2)
        {
            float fmin = 0.001f;
            if (Mathf.Abs(f1.x - f2.x) > fmin || Mathf.Abs(f1.y - f2.y) > fmin)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 获取GUID
        /// </summary>
        public static string GetGUID()
        {
            System.DateTime dt = System.DateTime.Now;
            string guid = dt.Year.ToString();
            guid += "-" + dt.Month.ToString();
            guid += "-" + dt.Day.ToString();
            guid += "-" + dt.Hour.ToString();
            guid += "-" + dt.Minute.ToString();
            guid += "-" + dt.Second.ToString();
            return guid;
        }

        /// <summary>
        /// 获取中心位置
        /// </summary>
        public static Vector3 GetCenterPos()
        {
            Camera cam = Camera.current;
            if (cam == null)
            {
                if (UnityEditor.SceneView.lastActiveSceneView != null)
                {
                    cam = UnityEditor.SceneView.lastActiveSceneView.camera;
                }
            }
            if (cam != null)
            {
                Transform tran = cam.transform;
                return tran.position + tran.forward * 3;
            }
            else
            {
                return Vector3.zero;
            }
        }

        /// <summary>
        /// 绘制属性
        /// </summary>
        public static SerializedProperty DrawProperty(SerializedObject obj, string strName, GUIContent lab, bool bChild = false)
        {
            SerializedProperty sp = obj.FindProperty(strName);
            if (sp != null)
            {
                EditorGUILayout.PropertyField(sp, lab, bChild);
            }
            return sp;
        }

        /// <summary>
        /// 获取所有场景物体
        /// </summary>
        public static List<GameObject> GetAllSceneObjects(bool bCurScene = false)
        {
            var allTransforms = Resources.FindObjectsOfTypeAll(typeof(Transform));
            var previousSelection = Selection.objects;
            Selection.objects = null;
            if (bCurScene)
            {
                UnityEngine.SceneManagement.Scene sc = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
                GameObject[] go = null;
                if (sc != null)
                {
                    go = sc.GetRootGameObjects();
                }
                Selection.objects = allTransforms.Cast<Transform>()
                .Where(x => x != null && IsCurSceneObj(x.gameObject, go))
                .Select(x => x.gameObject)
                .Cast<UnityEngine.Object>().ToArray();
            }
            else
            {
                Selection.objects = allTransforms.Cast<Transform>()
                .Where(x => x != null)
                .Select(x => x.gameObject)
                .Cast<UnityEngine.Object>().ToArray();
            }

            var selectedTransforms = Selection.GetTransforms(SelectionMode.Editable | SelectionMode.ExcludePrefab);
            Selection.objects = previousSelection;

            return selectedTransforms.Select(tr => tr.gameObject).ToList();
        }

        /// <summary>
        /// 是否是当前场景的物体
        /// </summary>
        public static bool IsCurSceneObj(GameObject go, GameObject[] parent)
        {
            if (go != null && parent != null && parent.Length > 0)
            {
                Transform par = go.transform;
                while (par != null)
                {
                    for (int i = 0; i < parent.Length; i++)
                    {
                        if (par.gameObject == parent[i])
                        {
                            return true;
                        }
                    }
                    par = par.parent;
                }
            }
            return false;
        }

        /// <summary>
        /// 显示标题
        /// </summary>
        public static bool Foldout(string title, bool expanded)
        {
            GUIStyle foldoutStyle = "ShurikenModuleTitle";
            foldoutStyle.font = (new GUIStyle("Label")).font;
            foldoutStyle.border = new RectOffset(15, 7, 4, 4);
            foldoutStyle.fixedHeight = 20f;
            foldoutStyle.contentOffset = new Vector2(20f, -2f);


            Rect rect = GUILayoutUtility.GetRect(16, 22, foldoutStyle);
            rect.x += 3;
            rect.width -= 6;  //为了好看的对齐
            GUI.Box(rect, title, foldoutStyle);

            Rect toggleRect = new Rect(rect.x + 4f, rect.y + 2f, 13f, 13f);
            if (Event.current.type == EventType.Repaint)
                EditorStyles.foldout.Draw(toggleRect, false, false, expanded, false);

            Event e = Event.current;
            if (e.type == EventType.MouseDown && rect.Contains(e.mousePosition))
            {
                expanded = !expanded;
                e.Use();
            }

            return expanded;
        }

        /// <summary>
        /// 保存字节
        /// </summary>
        public static void SaveByte(string strPath, byte[] vByte)
        {
            UtilTool.CreateDir(strPath);
            FileStream fsWrite = new FileStream(strPath, FileMode.Create, FileAccess.Write);
            BinaryWriter bw = new BinaryWriter(fsWrite);
            bw.Write(vByte);
            bw.Close();
            fsWrite.Close();
        }

        /// <summary>
        /// 显示滑杆
        /// </summary>
        public static float SliderShow(string title, float val, float min, float max, float TitleWidth)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(title, GUILayout.Width(TitleWidth));
            val = EditorGUILayout.Slider(val, min, max);
            EditorGUILayout.EndHorizontal();
            return val;
        }

        /// <summary>
        /// 获取场景物体
        /// </summary>
        public static GameObject GetSceneObjByShader(List<GameObject> list, string strShaderName, bool bCurScene = false)
        {
            if (list == null)
            {
                list = GetAllSceneObjects(bCurScene);
            }
            foreach (GameObject obj in list)
            {
                MeshRenderer mr = obj.GetComponent<MeshRenderer>();
                if (mr != null)
                {
                    Material mat = mr.sharedMaterial;
                    if (mat != null && mat.shader != null && mat.shader.name.StartsWith(strShaderName))
                    {
                        return mr.gameObject;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 获取场景物体
        /// </summary>
        public static List<GameObject> GetAllSceneObjByShader(List<GameObject> list, string strShaderName, bool bCurScene = false)
        {
            if (list == null)
            {
                list = GetAllSceneObjects(bCurScene);
            }
            List<GameObject> go = new List<GameObject>();
            foreach (GameObject obj in list)
            {
                MeshRenderer mr = obj.GetComponent<MeshRenderer>();
                if (mr != null)
                {
                    Material mat = mr.sharedMaterial;
                    if (mat != null && mat.shader != null && mat.shader.name.StartsWith(strShaderName))
                    {
                        go.Add(obj);
                    }
                }
            }
            return go;
        }

        /// <summary>
        /// 获取场景物体
        /// </summary>
        public static GameObject GetSceneObjByScripts(List<GameObject> list, System.Type tp, bool bCurScene = false)
        {
            if (list == null)
            {
                list = GetAllSceneObjects(bCurScene);
            }
            foreach (GameObject obj in list)
            {
                Component mr = obj.GetComponent(tp);
                if (mr != null)
                {
                    return mr.gameObject;
                }
            }
            return null;
        }

        /// <summary>
        /// 获取场景物体
        /// </summary>
        public static List<GameObject> GetAllSceneObjByScripts(List<GameObject> list, System.Type tp, bool bCurScene = false)
        {
            if (list == null)
            {
                list = GetAllSceneObjects(bCurScene);
            }
            List<GameObject> go = new List<GameObject>();
            foreach (GameObject obj in list)
            {
                Component mr = obj.GetComponent(tp);
                if (mr != null)
                {
                    go.Add(obj);
                }
            }
            return go;
        }

        /// <summary>
        /// 解绑物体下所有预制体
        /// </summary>
        public static void UnpackPrefabs(GameObject obj)
        {
            if (PrefabUtility.IsAnyPrefabInstanceRoot(obj))
            {
                PrefabUtility.UnpackPrefabInstance(obj, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
            }
            int iChild = obj.transform.childCount;
            for (int i = 0; i < iChild; i++)
            {
                Transform tran = obj.transform.GetChild(i);
                UnpackPrefabs(tran.gameObject);
            }
        }

        /// <summary>
        /// 查找是否有预制体
        /// </summary>
        public static bool FindPrefabs(GameObject obj)
        {
            if (PrefabUtility.IsAnyPrefabInstanceRoot(obj))
            {
                return true;
            }
            else
            {
                int iChild = obj.transform.childCount;
                for (int i = 0; i < iChild; i++)
                {
                    Transform tran = obj.transform.GetChild(i);
                    if (FindPrefabs(tran.gameObject))
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// 排开渲染队列
        /// </summary>
        public static List<string> OrderEffectRenderQueue(List<Material> mat)
        {
            List<string> output = new List<string>();
            mat.Sort((p1, p2) => p1.renderQueue - p2.renderQueue);
            int iRenderQueue = 0;
            for (int i = 0; i < mat.Count; i++)
            {
                if (iRenderQueue > 0)
                {
                    if (mat[i].renderQueue <= iRenderQueue)
                    {
                        iRenderQueue++;
                        string outStr = "材质" + mat[i].name + "渲染队列排序调整：" + mat[i].renderQueue + "=>" + iRenderQueue;
                        output.Add(outStr);
                        mat[i].renderQueue = iRenderQueue;
                    }
                    else
                    {
                        iRenderQueue = mat[i].renderQueue;
                    }
                }
                else
                {
                    iRenderQueue = mat[i].renderQueue;
                }
            }
            return output;
        }

        /// <summary>
        /// 检查创建目录
        /// </summary>
        public static void CreateDir(string strPath)
        {
            //先检查路径
            string DirPath = strPath.Substring(0, strPath.LastIndexOf('/'));
            //判断路径是否合法
            if (!Directory.Exists(DirPath))
            {
                //如果目录不存在就要创建
                Directory.CreateDirectory(DirPath);
            }
        }

        /// <summary>
        /// 显示提示(0: Dialog 1:Error 2:Warning 3:Log)
        /// </summary>
        public static void ShowTips(int iType, string strText, string strTitle = "")
        {
            switch (iType)
            {
                case 0:
                    {
                        if (strTitle == "")
                        {
                            strTitle = "提示";
                        }
                        EditorUtility.DisplayDialog(strTitle, strText, "ok");
                        break;
                    }
                case 1:
                    {
                        UnityEngine.Debug.LogError(strText);
                        break;
                    }
                case 2:
                    {
                        Debugger.LogWarning(strText);
                        break;
                    }
                case 3:
                    {
                        Debugger.Log(strText);
                        break;
                    }
            }
        }
    }
}

using System.IO;

namespace FilePathTool
{
    /// <summary>
    /// 路径扩展方法
    /// </summary>
    public class FilePathFactory
    {
        /// <summary>
        /// 查找并创建上层文件夹
        /// </summary>
        /// <param name="folderPath">上层目录路径</param>
        private void CreateOrOpenFolder(string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
                if (!Directory.Exists(folderPath))
                    CreateOrOpenFolder(folderPath);
                return;
            }
            return;
        }
        /// <summary>
        /// 创建改文件，并解决上层文件夹不存在的问题
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="fileName">文件名</param>
        /// <returns></returns>
        public string CreateOrOpenFile(string filePath, string fileName)
        {
            //string folderPath = "";
            //folderPath = GetParentDirectory(filePath);
            string newFilePath = RepathFile(filePath, fileName);
            //每层检测文件夹是否存在，不存在则创建        
            //string filepath = folderPath + fileName +count.ToString()+ ".png";       
            //File.Create(newFilePath);
            //FileStream fs1 = new FileStream(filepath, FileMode.Create, FileAccess.Write);//创建写入文件 
            return newFilePath;

        }

        static int count = 0;
        /// <summary>
        /// 修改重名文件名，返回修改后路径
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="fileName">文件名</param>
        /// <returns></returns>
        private string RepathFile(string filePath, string fileName)
        {
            count++;
            string newFilePath = "";
            string path = filePath;
            string folderPath = GetParentDirectory(path);
            newFilePath = folderPath + fileName + count.ToString() + ".png";
            if (File.Exists(newFilePath))
                return RepathFile(path, fileName);
            return newFilePath;
        }
        /// <summary>
        /// 获取上层目录，且如果上层目录不存在则创建
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private string GetParentDirectory(string filePath)
        {
            string[] paths = filePath.Split('/');
            string folderPath = "";
            //每层检测文件夹是否存在，不存在则创建
            for (int i = 0; i < paths.Length - 1; i++)
            {
                folderPath += paths[i];
                CreateOrOpenFolder(folderPath);
                folderPath += "/";
            }
            return folderPath;
        }
    }
}
    


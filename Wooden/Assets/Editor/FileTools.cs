
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
namespace EditorUtil
{
    public class FileTools
    {
        public static List<FileNameData> GetAllFiles(string root, string exName)
        {
            if (null != exName)
            {
                string path = string.Format("{0}", root);
                //string path = string.Format("{0}", @"C:\Users\USER\Desktop\JXBWG\Assets\StreamingAssets");
                List<FileNameData> res = new List<FileNameData>();
                //获取指定路径下面的所有资源文件  
                if (Directory.Exists(root))
                {
                    DirectoryInfo direction = new DirectoryInfo(path);
                    FileInfo[] files = direction.GetFiles("*"+exName);
                    for (int i = 0; i < files.Length; i++)
                    {
                        //忽略关联文件
                        if (files[i].Name.EndsWith(".meta") && !files[i].Name.Contains("$"))
                        {
                            continue;
                        }
                        string noExtendPath = files[i].FullName;
                        noExtendPath = noExtendPath.Replace(exName,"");
                        FileNameData data = new FileNameData(files[i].Name, files[i].FullName, noExtendPath);
                        //Debug.Log("文件名:" + files[i].Name);
                        //Debug.Log("文件绝对路径:" + files[i].FullName);
                        //Debug.Log("文件所在目录:" + files[i].DirectoryName);
                        res.Add(data);
                    }
                }
                return res;
            }
            return null;
        }
        public static void RenameFile(string path,string oldName,string newName)
        {          
            //读取文本　
            StreamReader sr = new StreamReader(path);
            string str = sr.ReadToEnd();
            sr.Close();
            //替换文本
            str = str.Replace(oldName, newName);
            string newPath = path.Replace(oldName, newName);
            if (File.Exists(newPath))
            {

            }
            else
            {
                //更改保存文本
                StreamWriter sw = new StreamWriter(newPath, false);
                sw.WriteLine(str);
                sw.Close();
            }          
        }
        private void WriteContext(string Context, string path)
        {
            StreamWriter sw = new StreamWriter(path);
            sw.Write(Context);
            sw.Close();
            sw.Dispose();
        }

        private string ReadContext(string path)
        {
            FileStream fs = new FileStream(path, FileMode.Open);
            StreamReader sr = new StreamReader(fs);
            string context = sr.ReadToEnd();
            fs.Close();
            sr.Close();
            sr.Dispose();
            fs.Dispose();
            return context;
        }
      
    }
}
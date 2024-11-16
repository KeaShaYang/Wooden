using UnityEditor;
using System.IO;
using Excel;
using System.Data;
using System.CodeDom;
using System.Reflection;
using System.CodeDom.Compiler;
using System;
using System.Numerics;
using System.Collections.Generic;
using UnityEngine;

namespace EditorUtil
{
    public class ExcelTool : Editor
    {
        //[MenuItem("Excel/Excel表转成C#类")]
        public static void TableToCSharp()
        {
            List<FileNameData> excelPaths = FileTools.GetAllFiles(PathHelper.ExcelPath, PathHelper.ExcelExtend); //获取指定路径下的后缀名为xlsx的所有Excel文件
            if (null != excelPaths)
            {
                for (int i = 0; i < excelPaths.Count; i++)
                {
                    string className = excelPaths[i].ClassName; //截取路径获得文件名（这是我写的拓展方法）                   
                    FileStream file = File.Open(excelPaths[i].FullName, FileMode.Open);
                    //使用c#读取Excel表的库
                    IExcelDataReader excelData = ExcelReaderFactory.CreateOpenXmlReader(file);
                    DataSet dataSet = excelData.AsDataSet();
                    //取第一张表
                    DataRowCollection rowCollection = dataSet.Tables[0].Rows;

                    CodeTypeDeclaration myClass = new CodeTypeDeclaration(className); //生成类
                    myClass.IsClass = true;
                    myClass.TypeAttributes = TypeAttributes.Public;
                    myClass.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference("System.Serializable"))); //添加序列化的特性

                    for (int j = 0; j < dataSet.Tables[0].Columns.Count - 1; j++)
                    {
                        string type = rowCollection[2][j].ToString(); //这边我的Excel第三行填的是数据类型，这里把他读出来
                        if (!string.IsNullOrEmpty(type))
                        {
                            string filed = rowCollection[0][j].ToString();//第一行字段名

                            CodeMemberField member = new CodeMemberField(GetTheType(type), filed); //生成字段

                            member.Attributes = MemberAttributes.Public;

                            myClass.Members.Add(member); //把生成的字段加入到生成的类中
                        }
                    }

                    CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
                    CodeGeneratorOptions options = new CodeGeneratorOptions();    //代码生成风格
                    options.BracingStyle = "C";
                    options.BlankLinesBetweenMembers = true;

                    string outputPath = PathHelper.ExcelToCsharpOutputPath + @"\" + className + ".cs";
                    using (StreamWriter sw = new StreamWriter(outputPath))
                    {
                        provider.GenerateCodeFromType(myClass, sw, options); //生成文件
                    }
                    Debugger.Log("生成C#  " + outputPath);
                }
            }
        }
        /// <summary>
        /// 获取类型
        /// </summary>
        /// <returns></returns>
        public static Type GetTheType(string type)
        {
            switch (type)
            {
                case "string":
                    return typeof(String);
                case "int":
                    return typeof(Int32);
                case "uint":
                    return typeof(UInt32);
                case "float":
                    return typeof(Single);
                case "long":
                    return typeof(Int64);
                case "double":
                    return typeof(Double);
                case "bigInteger":
                    return typeof(BigInteger);
                default:
                    return typeof(String);
            }
        }
        [MenuItem("Excel/生成表转换器")]
        public static void CreateConfigs()
        {
            string dirPath = PathHelper.ExcelPath;
            List<FileNameData> excelPaths = FileTools.GetAllFiles(PathHelper.ExcelPath, PathHelper.ExcelExtend);
            if (null != excelPaths)
            {
                for (int i = 0; i < excelPaths.Count; i++)
                {

                    string className = excelPaths[i].Name; //截取路径获得文件名（这是我写的拓展方法）
                    //生成config
                    //生成reader
                    FileTools.RenameFile(PathHelper.ExcelConfigPath + "TempleteExcelDataConfig.cs", "Templete", excelPaths[i].ClassName);
                    //FileTools.RenameFile(PathHelper.EditorPath + "TempleteDataReader.cs", "TempleteData", excelPaths[i].ClassName);
                    //DisplayDataReader.CreateOneData(className, excelPaths[i]);
                    
                }
            }
            AssetDatabase.Refresh();
            Debugger.Log("生成转换器");
        }
        //[MenuItem("Excel/生成表数据")]
        public static void CreateSignInData()
        {
            string dirPath = PathHelper.ExcelPath;
            List<FileNameData> excelPaths = FileTools.GetAllFiles(PathHelper.ExcelPath, PathHelper.ExcelExtend);
            if (null != excelPaths)
            {
                for (int i = 0; i < excelPaths.Count; i++)
                {
                    string className = excelPaths[i].Name; //截取路径获得文件名（这是我写的拓展方法）                      
                                                           //通过反射拿displayDataReader且调用方法
                    Type _type = Type.GetType(excelPaths[i].ClassName + "Reader"); // 类名：带命名空间的全名，不然会找不到类 例如 Murray.ClassName
                    object obj = System.Activator.CreateInstance(_type); // 通过类型创建实例
                                                                         //MethodInfo method = type.GetMethod("GuideMarkPosByID", new Type[] {}); // 方法无参
                    MethodInfo method = _type.GetMethod("CreateOneData", new Type[] { typeof(string), typeof(string) });
                    //displayDataReader.CreateOneData(className, excelPaths[i]);
                    object[] parameters = { className, excelPaths[i].FullName };
                    method.Invoke(obj, parameters);
                    Debugger.Log("转换数据成功" + excelPaths[i].ClassName);
                }
            }
            AssetDatabase.Refresh();
            Debugger.Log("读取数据成功");
        }
        //判断是否是空行
        public static bool IsEmptyRow(DataRow collect, int columnNum)
        {
            for (int i = 0; i < columnNum; i++)
            {
                if (!collect.IsNull(i)) return false;
            }
            return true;
        }

        /// <summary>
        /// 读取excel文件内容获取行数 列数 方便保存
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="columnNum">行数</param>
        /// <param name="rowNum">列数</param>
        /// <returns></returns>
        public static DataRowCollection ReadExcel(string filePath, ref int columnNum, ref int rowNum)
        {
            FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);

            DataSet result = excelReader.AsDataSet();
            //Tables[0] 下标0表示excel文件中第一张表的数据
            columnNum = result.Tables[0].Columns.Count;
            rowNum = result.Tables[0].Rows.Count;
            return result.Tables[0].Rows;
        }
        public static object ToValue(string fieldType, string s)
        {
            object o = null;

            if ("uint".Equals(fieldType))
            {
                uint value = 0;
                uint.TryParse(s, out value);
                o = value;
            }
            else if ("string".Equals(fieldType))
            {
                o = s;
            }
            else if ("bool".Equals(fieldType))
            {
                o = s.Equals("1");
            }
            else if ("int".Equals(fieldType))
            {
                int value = 0;
                int.TryParse(s, out value);
                o = value;
            }
            else if ("float".Equals(fieldType))
            {
                float value = 0f;
                float.TryParse(s, out value);
                o = value;
            }

            return o;
        }
    }
}

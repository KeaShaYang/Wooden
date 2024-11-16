using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class UIAtlasBuildTool : EditorWindow
{
    private string dirlist = "";
    private int padding = 1;
    private static string tpexepath = @"E:\SiChen\app\CodeAndWeb\TexturePacker\bin\";
    [MenuItem("AMLD/UI工具/导出UI图集")]
    static void Init()
    {
        var window = GetWindow<UIAtlasBuildTool>(true, "导出UI图集");
        window.Show();
    }

    private void OnGUI()
    {
        tpexepath = EditorGUILayout.TextField("TP工具路径：", tpexepath);
        dirlist = EditorGUILayout.TextField("图集名称(;分割)：", dirlist);
        padding = EditorGUILayout.IntField("图标间隔：", padding);
        if (GUILayout.Button("导出"))
        {
            if (!string.IsNullOrEmpty(dirlist))
            {
                try
                {
                    HashSet<string> selectedSantuFolders = new HashSet<string>();
                    string numpattern = "[0-9]+";
                    string[] dirs = dirlist.Split(';');
                    string santupath = Application.dataPath.Replace("\\", "/") + "/Art/UI/santu/";
                    string atlaspath = Application.dataPath.Replace("\\", "/") + "/Art/UI/Atlas/";
                    string tptoolpath = tpexepath + "TexturePacker.exe";
                    for (int i = 0; i < dirs.Length; i++)
                    {
                        string dirname = dirs[i];
                        string tempsantupath = santupath + dirname;
                        dirname = dirname.ToLower();
                        if (Directory.Exists(tempsantupath))
                        {
                            if (UISantuSyncWindow.needProcessSantuFolder.Contains(dirname))
                            {
                                if (!selectedSantuFolders.Contains(dirname))
                                {
                                    selectedSantuFolders.Add(dirname);
                                }
                            }
                            else if(Regex.IsMatch(dirname,numpattern))
                            {
                                string tempname = Regex.Replace(dirname, numpattern, "");
                                if (UISantuSyncWindow.needProcessSantuFolder.Contains(tempname))
                                {
                                    if (!selectedSantuFolders.Contains(dirname))
                                    {
                                        selectedSantuFolders.Add(dirname);
                                    }
                                }
                            }
                            if (dirname == "soulwake")
                            {
                                dirname = "soulawake";
                            }
                            string tempatlasname = dirname;
                            if (Regex.IsMatch(tempatlasname, numpattern))
                            {
                                tempatlasname = Regex.Replace(tempatlasname, numpattern, "");
                            }
                            string tempatlaspath = atlaspath + tempatlasname;
                            if (!Directory.Exists(tempatlaspath))
                            {
                                Directory.CreateDirectory(tempatlaspath);
                                i--;
                            }
                            Process cmd = new Process();
                            cmd.StartInfo.FileName = tptoolpath;
                            cmd.StartInfo.CreateNoWindow = false;
                            cmd.StartInfo.UseShellExecute = false;
                            cmd.StartInfo.RedirectStandardInput = true;
                            cmd.StartInfo.RedirectStandardOutput = true;
                            cmd.StartInfo.RedirectStandardError = true;
                            string mulstr = "";
                            string nstr = "";
                            if (dirname == "skills")
                            {
                                dirname = "skill";
                                nstr = "{n}";
                                mulstr = " --multipack";
                            }
                            cmd.StartInfo.Arguments =
                                $"{tempsantupath} --sheet {tempatlaspath}/{dirname}{nstr}.png --data {tempatlaspath}/{dirname}{nstr}.txt --format unity --max-size 2048 --border-padding 0 --shape-padding {padding} --extrude 0 --allow-free-size --disable-rotation --no-trim{mulstr}";
                            cmd.Start();
                            cmd.WaitForExit();
                            if (cmd.ExitCode != 0)
                            {
                                var output = cmd.StandardError.ReadToEnd();
                                cmd.Close();
                                throw new Exception(output.ToString());
                            }
                            cmd.Close();
                            AssetDatabase.Refresh();
                            if (dirname == "skill")
                            {
                                for (int j = 0; j <= 5; j++)
                                {
                                    string texPath = (tempatlaspath + "/" + dirname + j).Replace(Application.dataPath,"Assets") + ".png";
                                    UITools.GenerateSpritesByTexPath(texPath,null);
                                }
                            }
                            else
                            {
                                string texPath = (tempatlaspath + "/" + dirname).Replace(Application.dataPath,"Assets") + ".png";
                                UITools.GenerateSpritesByTexPath(texPath,null);
                            }
                        }
                    }
                    if (selectedSantuFolders.Count > 0)
                    {
                        UISantuSyncWindow.SyncSantus(selectedSantuFolders);
                        AssetDatabase.Refresh();
                    }
                    EditorUtility.DisplayDialog("", "导出完毕", "ok");
                }
                catch (Exception e)
                {
                    EditorUtility.DisplayDialog("", "导出失败", "ok");
                    Debug.LogError(e.Message);
                }
            }
        }
        
        GUILayout.Space(10);
        if (GUILayout.Button("同步所有动态图集散图（慎点）"))
        {
            UISantuSyncWindow.ForceSyncAllSantu();
        }
    }
}

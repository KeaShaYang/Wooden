using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;


public class BatchRenameTool : EditorWindow
{
    [MenuItem("GameObject/批量重命名选中物体", priority = 20)]
    public static void OpenWinBatchRename()
    {
        Rect wr = new Rect(0, 0, 200, 85);
        BatchRenameTool win = EditorWindow.GetWindowWithRect<BatchRenameTool>(wr, true, "批量命名工具(GameObject版)", true);
        win.IsAsset = false;
        win.titleContent = new GUIContent("批量命名工具(GameObject版)");
    }

    [MenuItem("Assets/批量重命名选中物体", priority = 900)]
    public static void OpenAssetWinBatchRename()
    {
        Rect wr = new Rect(0, 0, 200, 85);
        BatchRenameTool win = EditorWindow.GetWindowWithRect<BatchRenameTool>(wr, true, "批量命名工具(Asset资源版)", true);
        win.IsAsset = true;
        win.titleContent = new GUIContent("批量命名工具(Asset资源版)");
    }


    private bool m_InitPos = false;
    private string m_NewName = "";
    private int m_StartIdx = 1;

    public bool IsAsset = false;

    private void Update()
    {
        if (focusedWindow != this)
        {
            if(Selection.activeObject != null)
            {
                if (EditorUtility.IsPersistent(Selection.activeObject))
                {
                    this.IsAsset = true;
                    this.titleContent = new GUIContent("批量命名工具(Asset资源版)");
                }
                else
                {
                    this.IsAsset = false;
                    this.titleContent = new GUIContent("批量命名工具(GameObject版)");
                }
            }
            Repaint();
        }

    }

    private void OnGUI()
    {
        if (!m_InitPos)
        {
            Vector2 mousePos = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
            Rect wr = this.position;
            wr.x = mousePos.x;
            wr.y = mousePos.y;
            this.position = wr;
            m_InitPos = true;
        }

        if (IsAsset)
        {
            EditorGUILayout.LabelField("当前选中资源数量: " + Selection.objects.Length);
        }
        else
        {
            EditorGUILayout.LabelField("当前选中物体数量: " + Selection.gameObjects.Length);
        }

        EditorGUIUtility.labelWidth = 50;
        m_NewName = EditorGUILayout.TextField("新命名", m_NewName);
        EditorGUILayout.BeginHorizontal();
        m_StartIdx = EditorGUILayout.IntField("开始索引", m_StartIdx, GUILayout.Width(100));
        if (GUILayout.Button("批量重命名"))
        {
            if (!string.IsNullOrEmpty(m_NewName))
            {

                if (IsAsset)
                {
                    List<Object> orderObjs = new List<Object>(Selection.objects);
                    orderObjs.Sort((obj1, obj2) => obj1.name.CompareTo(obj2.name));
                    int nameIndex = m_StartIdx;
                    foreach (var obj in orderObjs)
                    {
                        string oldPath = AssetDatabase.GetAssetPath(obj);
                        string newName = m_NewName + "_" + nameIndex.ToString("00") + Path.GetExtension(oldPath);
                        string info = AssetDatabase.RenameAsset(oldPath, newName);
                        if (info.Contains("already exist"))
                        {
                            do
                            {
                                nameIndex++;
                                newName = m_NewName + "_" + nameIndex.ToString("00") + Path.GetExtension(oldPath);
                                info = AssetDatabase.RenameAsset(oldPath, newName);
                            }
                            while (info.Contains("already exist"));
                        }
                        else if (!string.IsNullOrEmpty(info))
                        {
                            Debug.LogWarning(info);
                            break;
                        }
                        nameIndex++;
                    }
                }
                else
                {
                    List<GameObject> orderObjs = new List<GameObject>(Selection.gameObjects);
                    orderObjs.Sort((go1, go2) => go1.transform.GetSiblingIndex().CompareTo(go2.transform.GetSiblingIndex()));
                    int nameIndex = m_StartIdx;
                    Undo.RecordObjects(Selection.gameObjects, "BatchRename");
                    foreach (var go in orderObjs)
                    {
                        go.name = m_NewName + "_" + nameIndex.ToString("00");
                        nameIndex++;
                    }
                }

            }
        }
        EditorGUILayout.EndHorizontal();
        if (!string.IsNullOrEmpty(m_NewName))
        {
            EditorGUILayout.LabelField("预览：" + m_NewName + "_" + m_StartIdx.ToString("00"));
        }
        else
        {
            EditorGUILayout.LabelField("预览：");
        }
    }

}

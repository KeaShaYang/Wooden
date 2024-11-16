using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class UIPanelMerger : EditorWindow
{
    class MergeData
    {
        public Graphic target;
        public bool syncPosition;
    }

    private GameObject panelGo;
    private GameObject artPanelGo;
    private GameObject leftObj;
    private Dictionary<Graphic, MergeData> graphicsMap = new Dictionary<Graphic, MergeData>();
    private List<Graphic> graphics = new List<Graphic>();
    private Vector2 scrollVec;

    private Graphic selGraphic = null;
    private Graphic leftGraphic = null;
    private bool alwaysPos = false;
    private bool fastClear = false;

    [MenuItem("AMLD/UI工具/UI面板合并工具")]
    static void Init()
    {
        var window = GetWindowWithRect<UIPanelMerger>(new Rect(0, 0, 350, 600), false, "UI面板合并工具");
        window.minSize = new Vector2(350, 300);
        window.maxSize = new Vector2(1920, 1080);
        window.Show();
    }

    private string m_FilterStr = string.Empty;
    private void OnGUI()
    {
        GUILayout.Label("拖入已经制作好的临时UI面板");
        GUILayout.Label("【快捷键】S-可以显示/隐藏临时UI面板 D-半透明 A-可高亮选中的UI");
        GUILayout.Label("         1-使UI进入匹配状态  2-设置匹配的UI 3-一次性合并(");
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("         3-一次性合并(");
        float oldWidth = EditorGUIUtility.labelWidth;
        EditorGUIUtility.labelWidth = 48;
        alwaysPos = EditorGUILayout.Toggle("同步位置", alwaysPos);
        EditorGUIUtility.labelWidth = 38;
        fastClear = EditorGUILayout.Toggle("带清空", fastClear);
        EditorGUIUtility.labelWidth = oldWidth;
        GUILayout.Label(")");
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        m_FilterStr = EditorGUILayout.TextField("过滤控件", m_FilterStr);

        GUILayout.Label("我们的面板      <--      美术的面板");
        EditorGUILayout.BeginHorizontal();
        GameObject obj = EditorGUILayout.ObjectField(panelGo, typeof(GameObject), true) as GameObject;
        if (obj && obj != panelGo)
        {
            //            if (!PrefabUtility.IsAnyPrefabInstanceRoot(obj))
            //            {
            //                EditorUtility.DisplayDialog("提示", "选择的面板不是预制体", "ok");
            //                return;
            //            }

            panelGo = obj;
            panelGo.GetComponentsInChildren(true, graphics);
            graphicsMap.Clear();
            foreach (var graph in graphics)
            {
                MergeData data = new MergeData();
                data.target = null;
                graphicsMap[graph] = data;
            }
        }
        if (obj != null)
        {
            if (GUILayout.Button("刷新"))
            {
                Dictionary<Graphic, MergeData> tempDict = new Dictionary<Graphic, MergeData>();
                obj.GetComponentsInChildren(true, graphics);
                foreach (var graph in graphics)
                {
                    if (graphicsMap.ContainsKey(graph))
                    {
                        tempDict[graph] = graphicsMap[graph];
                    }
                    else
                    {
                        MergeData data = new MergeData();
                        data.target = null;
                        tempDict[graph] = data;
                    }
                }
                graphicsMap.Clear();
                graphicsMap = tempDict;
            }
            if (GUILayout.Button("清空"))
            {
                graphicsMap.Clear();
                obj.GetComponentsInChildren(true, graphics);
                foreach (var graph in graphics)
                {
                    MergeData data = new MergeData();
                    data.target = null;
                    graphicsMap[graph] = data;
                }
            }
        }
        artPanelGo = EditorGUILayout.ObjectField(artPanelGo, typeof(GameObject), true) as GameObject;
        EditorGUILayout.EndHorizontal();

        scrollVec = GUILayout.BeginScrollView(scrollVec);
        GUILayout.BeginHorizontal();

        GUILayout.BeginVertical();
        GUILayout.Label("临时UI面板元素");
        foreach (var graph in graphics)
        {
            if (!string.IsNullOrEmpty(m_FilterStr) && !graph.name.ToLower().Contains(m_FilterStr.ToLower()))
            {
                continue;
            }
            if ((selGraphic != null && selGraphic == graph) || (leftGraphic != null && leftGraphic == graph))
            {
                Color defaultColor = GUI.color;
                if(selGraphic != null)
                {
                    GUI.color = Color.red;
                }else if(leftGraphic != null)
                {
                    GUI.color = Color.green;
                }

                EditorGUILayout.ObjectField(graph, typeof(Graphic), false);
                GUI.color = defaultColor;
            }
            else
            {
                EditorGUILayout.ObjectField(graph, typeof(Graphic), false);
            }
        }
        GUILayout.EndVertical();

        GUILayout.BeginVertical();
        GUILayout.Label("<--");
        foreach (var graph in graphics)
        {
            GUILayout.Label("<--");
        }
        GUILayout.EndVertical();

        GUILayout.BeginVertical();
        GUILayout.Label("拖入对应美术生成面板元素");
        foreach (var graph in graphics)
        {
            if (!string.IsNullOrEmpty(m_FilterStr) && !graph.name.ToLower().Contains(m_FilterStr.ToLower()))
            {
                continue;
            }
            if (selGraphic != null && selGraphic == graphicsMap[graph].target)
            {
                Color defaultColor = GUI.color;
                GUI.color = Color.red;
                graphicsMap[graph].target = EditorGUILayout.ObjectField(graphicsMap[graph].target, typeof(Graphic), true) as Graphic;
                GUI.color = defaultColor;
            }
            else
            {
                graphicsMap[graph].target = EditorGUILayout.ObjectField(graphicsMap[graph].target, typeof(Graphic), true) as Graphic;
            }
        }
        GUILayout.EndVertical();

        GUILayout.BeginVertical();
        GUILayout.Label("是否同步位置");
        foreach (var graph in graphics)
        {
            if (!string.IsNullOrEmpty(m_FilterStr) && !graph.name.ToLower().Contains(m_FilterStr.ToLower()))
            {
                continue;
            }
            graphicsMap[graph].syncPosition = EditorGUILayout.Toggle(graphicsMap[graph].syncPosition);
        }
        GUILayout.EndVertical();

        GUILayout.EndHorizontal();
        GUILayout.EndScrollView();

        if (GUILayout.Button("一键合并", GUILayout.Height(20)))
        {
            Merge();

            leftGraphic = null;
        }
    }


    private void Merge(bool alwaysSyncPosition = false)
    {
        foreach (var graph in graphics)
        {
            MergeData data = null;
            if (graphicsMap.TryGetValue(graph, out data))
            {
                if (data!=null && data.target)
                {
                    bool raycastTarget = graph.raycastTarget;
                    if (ComponentUtility.CopyComponent(data.target)) ComponentUtility.PasteComponentValues(graph);
                    graph.raycastTarget = raycastTarget;
                    RectTransform sourceRect = graph.GetComponent<RectTransform>();
                    RectTransform targetRect = data.target.GetComponent<RectTransform>();
                    if (data.syncPosition || alwaysSyncPosition)
                    {
                        sourceRect.position = targetRect.position;
                        sourceRect.rotation = targetRect.rotation;
                        sourceRect.localScale = targetRect.localScale;
                    }
                    sourceRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, targetRect.rect.size.x);
                    sourceRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, targetRect.rect.size.y);
                    if (data.target is Text && graph is Text)
                    {
                        Shadow[] shadowLiners = graph.GetComponents<Shadow>();
                        foreach (var shadowLiner in shadowLiners)
                        {
                            if (shadowLiner is Shadow) DestroyImmediate(shadowLiner);
                        }
                        ShadowGamma[] shadows = data.target.GetComponents<ShadowGamma>();
                        ShadowGamma[] oldShadows = graph.GetComponents<ShadowGamma>();
                        foreach (var shadow in shadows)
                        {
                            if (shadow && ComponentUtility.CopyComponent(shadow))
                            {
                                bool find = false;
                                foreach (var oldShadow in oldShadows)
                                {
                                    if (oldShadow && oldShadow.GetType() == shadow.GetType())
                                    {
                                        ComponentUtility.PasteComponentValues(oldShadow);
                                        find = true;
                                        break;
                                    }
                                }
                                if (!find) ComponentUtility.PasteComponentAsNew(graph.gameObject);
                            }
                        }
                    }
                }
            }
        }
    }

    private void OnEnable()
    {
        SceneView.onSceneGUIDelegate += OnSceneGUI;
    }

    private void OnDisable()
    {
        SceneView.onSceneGUIDelegate -= OnSceneGUI;
    }

    void OnSceneGUI(SceneView sceneView)
    {
        Event e = Event.current;
        if (e != null && e.type == EventType.KeyDown && !e.control && !e.shift && !e.alt)
        {
            if (e.keyCode == KeyCode.S)
            {
                if (panelGo != null) panelGo.SetActive(!panelGo.activeInHierarchy);
                e.Use();
            }
            else if (e.keyCode == KeyCode.A)
            {
                GameObject selObj = Selection.activeGameObject;
                selGraphic = selObj.GetComponent<Graphic>();
                //if (selGraphic == null)
                //{
                //    selGraphic = selObj.GetComponentInChildren<Graphic>();
                //}
                if (leftGraphic == selGraphic)
                {
                    leftGraphic = null;
                }
                Repaint();
                e.Use();
            }
            else if (e.keyCode == KeyCode.D)
            {
                if (panelGo != null)
                {
                    CanvasGroup cg = panelGo.GetComponent<CanvasGroup>();
                    if (cg != null)
                    {
                        DestroyImmediate(cg);
                    }
                    else
                    {
                        cg = panelGo.AddComponent<CanvasGroup>();
                        cg.alpha = 0.5f;
                    }
                }
                e.Use();
            }
            else if (e.keyCode == KeyCode.Alpha1)
            {
                GameObject selObj = Selection.activeGameObject;
                leftGraphic = selObj.GetComponent<Graphic>();
                if (leftGraphic == selGraphic)
                {
                    selGraphic = null;
                }
                Repaint();
                e.Use();
            }
            else if (e.keyCode == KeyCode.Alpha2)
            {
                GameObject selObj = Selection.activeGameObject;
                Graphic rightGraphic = selObj.GetComponent<Graphic>();
                if (leftGraphic != null && rightGraphic != null && !graphics.Contains(rightGraphic) && graphicsMap.ContainsKey(leftGraphic))
                {
                    graphicsMap[leftGraphic].target = rightGraphic;
                }
                Repaint();
                e.Use();
            }
            else if (e.keyCode == KeyCode.Alpha3)
            {
                Merge(alwaysPos);
                selGraphic = null;
                leftGraphic = null;
                if (fastClear)
                {
                    foreach (var graph in graphics)
                    {
                        graphicsMap[graph].target = null;
                        graphicsMap[graph].syncPosition = false;
                    }
                }
                Repaint();
                e.Use();
            }

        }

        if (e != null && e.type == EventType.MouseDown && e.control)
        {
            if (artPanelGo)
            {
                Canvas cs = artPanelGo.GetComponent<Canvas>();
                if (cs)
                {
                    List<Graphic> results = new List<Graphic>();
                    Graphic[] graphicsForCanvas = cs.GetComponentsInChildren<Graphic>(true);
                    int count1 = graphicsForCanvas.Length;
                    Vector2 mousePosition = e.mousePosition;
                    //固定会有这么点偏移
                    mousePosition.y = -mousePosition.y + sceneView.position.height - 16f;
                    mousePosition.x = mousePosition.x + 1.8f;
                    for (int index = 0; index < count1; ++index)
                    {
                        Vector2 localPoint = new Vector2();
                        Graphic foundGraphic = graphicsForCanvas[index];
                        RectTransformUtility.ScreenPointToLocalPointInRectangle(foundGraphic.rectTransform,
                            mousePosition, sceneView.camera, out localPoint);
                        if (foundGraphic.rectTransform.rect.Contains(localPoint))
                        {
                            results.Add(foundGraphic);
                        }
                    }

                    if (results.Count > 0)
                    {
                        leftObj = Selection.activeObject as GameObject;
                        GraphicsSelector.Open();
                        GraphicsSelector win = focusedWindow as GraphicsSelector;
                        if (win)
                        {
                            win.Refresh(results, OnSelectGraphic);
                        }
                    }
                }
            }
        }
    }

    void OnSelectGraphic(Graphic graphic)
    {
        if (!leftObj) return;
        Graphic left = leftObj.GetComponent<Graphic>();
        if (left)
        {
            MergeData data = null;
            if (graphicsMap.TryGetValue(left, out data))
            {
                data.target = graphic;
            }
            else
            {
                data = new MergeData();
                data.target = graphic;
                graphicsMap[left] = data;
            }
        }
        selGraphic = graphic;
        Repaint();
    }

    private void OnFocus()
    {
        selGraphic = null;
    }
}
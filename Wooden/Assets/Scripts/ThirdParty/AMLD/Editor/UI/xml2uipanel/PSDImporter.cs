using UnityEditor;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using Unity.EditorCoroutines.Editor;
using UnityEngine.UI;
using Object = System.Object;

//------------------------------------------------------------------------------
// class definition
//------------------------------------------------------------------------------
public class PSDUI
{
	public Layer[] layers;
    public Size psdSize;

	public enum LayerType { Normal, Scroll, Grid, Button, Lable}
	public class Layer
	{
		public string name;
		public LayerType type;

        //行数
        //列数
        //render width
        //render height
        //水平间距
        //垂直间距
        //滑动方向
        public string[] arguments;   

		public Layer[] layers;
		public Image image;
        public ScrollViewData scrollview;
        public LabelData label;
        public ButtonData button;
        public Position position;
		public Size size;
        public TabData tab;
    }

	public class Position
	{
		public float x;
		public float y;
	}

	public class Size
	{
		public float width;
		public float height;
	}

    public class LabelExtraData
    {
        public float paddingX;//字间距
        public float paddingY;//行间距
        public bool isBold;//是否粗体
        public bool isItalic;//是否斜体
        public string effectColor;//描边颜色
        public string effectSize;//描边大小
        public float effectAlpha;//描边透明度
    }

	public enum ImageType { Image, Texture, Label, SliceImage }; 
	public enum ImageSource { Common, Custom };

	public class Image
	{
		public ImageType imageType;
		public ImageSource imageSource;
		public string name;
		public Position position;
		public Size size;

        // Label color.rgb.hexValue font size  content
        // SliceImage left right bottom top
		public string[] arguments;    
	}

    public class ScrollViewData
    {
        public string name;
        public Position position;
        public Size size;
    }

    public class LabelData
    {
        public string name;
        public Position position;
        public Size size;
        //颜色 字体 字体大小 内容
        public string[] arguments;
        public LabelExtraData labelExtraData;
    }

    public class ButtonData
    {
        public string name;
        public Layer[] layerArr;
    }

    public class TabData
    {
        public string name;
        public Layer[] btnArr;
    }
}


public class CommonPSDImporter : UnityEditor.Editor
{
    private static int curDepth ;
    private static List<GameObject> spriteList;
    private static List<GameObject> textureList;
    private static List<int> spriteWidthList;
    private static List<int> spriteHeightList;
    private static List<int> textureWidthList;
    private static List<int> textureHeightList;
    private static Transform panelTrans;
    
    [MenuItem("Assets/UI工具/XML生成界面",false,priority =40)]
    static public void ImportHogSceneMenuItem ()
    {
        if (Selection.activeObject == null)
            return;

        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (string.IsNullOrEmpty(path))
            return;

        path = Application.dataPath + "/" + path.Replace("Assets/", "");
        //Debug.LogError("show path:" + path);
        byte[] filedata = File.ReadAllBytes(path);
        string fileContent = System.Text.Encoding.GetEncoding("GB18030").GetString(filedata);
        //Debug.LogError(fileContent);

        try {
            ImportPSDUI(fileContent);
        }
        catch(XmlException e)
        {
            string fileContent2 = System.Text.Encoding.UTF8.GetString(filedata);
            //Debug.LogError(fileContent2);
            ImportPSDUI(fileContent2);
        }

        
    }
    
    

    static private void ImportPSDUI (string content)
    {
        content = PreprocessContent(content);
        spriteList = new List<GameObject>();
        textureList = new List<GameObject>();
        spriteWidthList = new List<int>();
        spriteHeightList = new List<int>();
        textureWidthList = new List<int>();
        textureHeightList = new List<int>();
        // before we do anything else, try to deserialize the input file and be sure it's actually the right kind of file
        PSDUI psdUI = (PSDUI)DeserializeXml (content, typeof(PSDUI));
        createGoByPSD(psdUI);
    }

    /// <summary>
    /// 预防解析问题
    /// </summary>
    static private string PreprocessContent(string content)
    {
        Regex regex = new Regex("<string><.*></string>");
//        Match match = regex.Match(content);
//        Debug.Log(match.Value);
        while (regex.IsMatch(content))
        {
            content = regex.Replace(content, (match) =>
            {
                return "<string>《？《" + match.Value.Substring(9, match.Value.Length - 19) + "》？》</string>";
            });
        }
        return content;
    }

    private static GameObject panelRoot;
    static private void createGoByPSD(PSDUI ui)
    {
        if (null == ui)
        {
            Debug.LogError("确认文件是否为utf8无bom格式！！！");
        }
        GameObject go = UGUITool.GetOrCreateCanvasGameObject();
        if (go != null)
        {
            go.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            GameObject panel = new GameObject(Selection.activeObject.name, typeof(RectTransform), typeof(Canvas), typeof(GraphicRaycaster));
            RectTransform rectTran = panel.GetComponent<RectTransform>();
            rectTran.SetParent(go.transform, false);
            rectTran.anchorMin = Vector2.zero;
            rectTran.anchorMax = Vector2.one;
            rectTran.sizeDelta = Vector2.zero;
            Selection.activeGameObject = panel;
            GraphicRaycaster gr = panel.GetComponent<GraphicRaycaster>();
            gr.ignoreReversedGraphics = false;
            
            panelRoot = new GameObject("root", typeof(RectTransform));
            RectTransform panelRootTran = panelRoot.GetComponent<RectTransform>();
            panelRootTran.SetParent(rectTran, false);
            panelRootTran.anchorMin = new Vector2(0, 1);
            panelRootTran.anchorMax = new Vector2(0, 1);
            panelRootTran.anchoredPosition = Vector2.zero;
            
            for (int i = 0; i < ui.layers.Length; i++)
            {
                createLayer(panelRoot, ui.layers[i], ui.psdSize);
            }

            Graphic[] graphics = panel.GetComponentsInChildren<Graphic>();
            for (int i = 0; i < graphics.Length; i++)
            {
                graphics[i].raycastTarget = false;
            }

            ChangeLayer(panel.transform, LayerMask.NameToLayer("UI"));
            //ProcessNode(panel.transform);
            ProcessMirrorImage(panel.transform);
            ProcessText(panel.transform);
            panelTrans = panel.transform;
            EditorCoroutineUtility.StartCoroutine(ExecuteRoutineWithWaitForSeconds(),panelTrans);
        }
//        CheckTurnState(go);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        // yield return null;
    }
    
    // public IEnumerator ThrowingCoroutine_DoesNotHandleExitGUIException() //prefixed test with Z in order to ensure it is last
    // {
    //     EditorCoroutineUtility.StartCoroutineOwnerless(ExecuteRoutineWithWaitForSeconds());
    //     yield return null;
    // }

    static IEnumerator ExecuteRoutineWithWaitForSeconds()
    {
        yield return null;
        if (panelTrans != null)
        {
            //PrefabVariableTools.AdaptationByObj(panelTrans);
            panelTrans = null;
        }
    }

    static void ChangeLayer(Transform root, int layer)
    {
        root.gameObject.layer = layer;
        int count = root.childCount;
        for (int i = 0; i < count; i++)
        {
            ChangeLayer(root.GetChild(i), layer);
        }
    }

    static void ProcessNode(Transform root)
    {
        if (root.GetComponents<Component>().Length == 1)
        {
            Graphic graphic;
            if (ChildrenHasComponent(root, out graphic))
            {
                GameObject obj = new GameObject(root.name, typeof(RectTransform));
                obj.transform.parent = root;
                obj.GetComponent<RectTransform>().anchoredPosition = graphic.GetComponent<RectTransform>().anchoredPosition;
                obj.transform.parent = root.parent;
                obj.transform.SetSiblingIndex(root.GetSiblingIndex());
                while (root.childCount > 0)
                {
                    root.GetChild(0).parent = obj.transform;
                }
                DestroyImmediate(root.gameObject);
                root = obj.transform;
            }
        }
        int count = root.childCount;
        for (int i = 0; i < count; i++)
        {
            ProcessNode(root.GetChild(i));
        }  
    }

    /// <summary>
    /// 处理所有文本全部都用居中显示
    /// </summary>
    static void ProcessText(Transform root)
    {
        Text[] texts = root.GetComponentsInChildren<Text>(true);
        foreach (var text in texts)
        {
            Vector2 curSize = text.rectTransform.sizeDelta;
            text.horizontalOverflow = HorizontalWrapMode.Overflow;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            var setting = text.GetGenerationSettings(text.rectTransform.rect.size);
            float height = text.cachedTextGenerator.GetPreferredHeight(text.text, setting);
            float width = text.cachedTextGenerator.GetPreferredWidth(text.text, setting);
            Vector2 newSize = new Vector2(width / text.pixelsPerUnit, height / text.pixelsPerUnit);
            Vector3 pos = text.rectTransform.localPosition;
            pos.x -= (curSize.x - newSize.x) * 0.5f;
            pos.y += (curSize.y - newSize.y) * 0.5f;
            pos.x = Mathf.Ceil(pos.x);
            pos.y = Mathf.Ceil(pos.y);
            text.rectTransform.localPosition = pos;
            text.rectTransform.sizeDelta = newSize;
            text.alignment = TextAnchor.MiddleCenter;
        }
    }

    static bool ChildrenHasComponent<T>(Transform root, out T comp) where T : Component
    {
        int count = root.childCount;
        for (int i = 0; i < count; i++)
        {
            comp = root.GetChild(i).GetComponent<T>();
            if (comp) return true;
        }

        comp = null;
        return false;
    }

    static Dictionary<string, Transform> mirrorNodeDic = new Dictionary<string, Transform>();
    
    static Dictionary<string, string[]> mirrorDirDic = new Dictionary<string, string[]>()
    {
        {"@zuo", new []{"@you"}},
        {"@you", new []{"@zuo"}},
        {"@shang", new []{"@xia"}},
        {"@xia", new []{"@shang"}},
        {"@zuoshang", new []{"@zuoxia", "@youshang", "@youxia"}},
        {"@zuoxia", new []{"@zuoshang", "@youshang", "@youxia"}},
        {"@youshang", new []{"@zuoxia", "@zuoshang", "@youxia"}},
        {"@youxia", new []{"@zuoxia", "@youshang", "@zuoshang"}}
    };

    static void ProcessMirrorImage(Transform root)
    {
        List<Transform> childList = new List<Transform>();
        int count = root.childCount;
        for (int i = 0; i < count; i++)
        {
            childList.Add(root.GetChild(i));
        }
        for (int i = 0; i < childList.Count; i++)
        {
            Transform tran = childList[i];
            if (!tran) continue;
            string tranName = tran.name;
            if (!tranName.Contains("_jx_")) continue;
            int index = tranName.IndexOf("@", StringComparison.Ordinal);
            if (index < 0) continue;
            string profix = tranName.Substring(0, index);
            string suffix = tranName.Substring(index);
            foreach (var key in mirrorDirDic.Keys)
            {
                if (suffix == key)
                {
                    var list = mirrorDirDic[key];
                    bool hasAll = true;
                    for (int j = 0; j < list.Length; j++)
                    {
                        if (!mirrorNodeDic.ContainsKey(profix + list[j]))
                        {
                            hasAll = false;
                            break;
                        }
                    }
                    if (hasAll)
                    {
                        // 其他镜像部分的Image都找到
                        List<Transform> otherTranList = new List<Transform>();
                        // 判断rect范围，看是否是镜像图
                        bool isMirror = true;
                        Rect rect = tran.GetComponent<RectTransform>().rect;
                        // 上下左右向外阔一个像素，然后判断是否重叠，以此判定是否是镜像
                        rect.x = rect.x - 1 + tran.localPosition.x;
                        rect.y = rect.y - 1 + tran.localPosition.y;
                        rect.width = rect.width + 2;
                        rect.height = rect.height + 2;
                        for (int j = 0; j < list.Length; j++)
                        {
                            Transform otherTran = mirrorNodeDic[profix + list[j]];
                            mirrorNodeDic.Remove(profix + list[j]);
                            otherTranList.Add(otherTran);
                            Rect otherRect = otherTran.GetComponent<RectTransform>().rect;
                            if (!rect.Contains(otherRect.min + (Vector2)otherTran.localPosition) && !rect.Contains(otherRect.max + (Vector2)otherTran.localPosition))
                            {
                                isMirror = false;
                            }
                        }
                        if (isMirror)
                        {
                            // 合成一张镜像Image
                            Vector3 middlePos = Vector3.zero;
                            for (int j = 0; j < otherTranList.Count; j++)
                            {
                                Transform otherTran = otherTranList[j];
                                middlePos += otherTran.localPosition;
                                DestroyImmediate(otherTran.gameObject);
                            }
                            middlePos += tran.localPosition;
                            middlePos /= list.Length + 1;
                            tran.localPosition = middlePos;
                            tran.name = profix;
                            UIMirror mirrorComp = tran.gameObject.AddComponent<UIMirror>();
                            if (list.Length == 3)
                            {
                                mirrorComp.MirrorType = UIMirror.UIMirrorType.Quarter;
                                RectTransform rectTran = tran.GetComponent<RectTransform>();
                                var oldSize = rectTran.rect.size;
                                rectTran.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, oldSize.x * 2);
                                rectTran.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, oldSize.y * 2);
                            }
                            else
                            {
                                if (list[0] == "@zuo" || list[0] == "@you")
                                {
                                    mirrorComp.MirrorType = UIMirror.UIMirrorType.Horizontal;
                                    RectTransform rectTran = tran.GetComponent<RectTransform>();
                                    var oldSize = rectTran.rect.width;
                                    rectTran.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, oldSize * 2);
                                }
                                else
                                {
                                    mirrorComp.MirrorType = UIMirror.UIMirrorType.Vertical;
                                    RectTransform rectTran = tran.GetComponent<RectTransform>();
                                    var oldSize = rectTran.rect.height;
                                    rectTran.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, oldSize * 2);
                                }
                            }
//                        mirrorComp.SetNativeSize();
                        }
                    }
                    else
                    {
                        //其他部分没找到，先缓存起来
                        if (!mirrorNodeDic.ContainsKey(tranName)) mirrorNodeDic.Add(tranName, tran);
                    }
                    break;
                }
            }
        }
        for (int i = 0; i < childList.Count; i++)
        {
            Transform tran = childList[i];
            if (!tran) continue;
            ProcessMirrorImage(tran);
        }
    }

//    private static void CheckTurnState(GameObject fatherGo)
//    {
//        Image[] curSpriteList = fatherGo.GetComponentsInChildren<Image>();
//        RawImage[] curTextureList = fatherGo.GetComponentsInChildren<RawImage>();
//        int i = 0;
//        int j = 0;
//        string curName = "";
//        string[] strArr;
//        UIAtlas atlas = null;
//        string atlasName;
//        UISpriteData spriteData;
//        int blankWidth = 0;
//        int blankHeight = 0;
//        if (curSpriteList != null && spriteList != null)
//        {            
//            for(i=0;i< spriteList.Count; i++)
//            {
//                strArr = StringProxy.Split(spriteList[i].name, "@");
//                blankWidth = 0;
//                blankHeight = 0;
//                if (strArr != null && strArr.Length == 2)
//                {
//                    curName = strArr[0];
//                    for(j=0;j< curSpriteList.Length; j++)
//                    {
//                        if(curSpriteList[j].name == curName)
//                        {
//                            atlasName = GetUIAtlas(curName);                           
//                            if (atlasName != "")
//                            {
//                                atlas = AddMissAtlasWindow.GetAtlasByName(atlasName);
//                                if (atlas != null)
//                                {
//                                    spriteData = atlas.GetSprite(curName);
//                                    bool isSplit = IsChangeSplit(atlas, curName);
//                                    if (spriteData != null && !isSplit)
//                                    {
//                                        if (isHasPadVal(spriteData))
//                                        {
//                                            blankWidth = Math.Abs(spriteData.width - spriteWidthList[i]);
//                                            blankHeight = Math.Abs(spriteData.height - spriteHeightList[i]);
//                                        }
//                                        if (IsOkComponent(curSpriteList[j], spriteList[i], strArr[1], blankWidth, blankHeight, spriteWidthList[i], spriteHeightList[i]))
//                                        {
//                                            break;
//                                        }
//                                    }
//                                }
//                            }                           
//                        }
//                    }
//                }
//            }
//        }
//
//        if (curTextureList != null && textureList != null)
//        {
//            for (i = 0; i < textureList.Count; i++)
//            {
//                strArr = StringProxy.Split(textureList[i].name, "@");
//                if (strArr != null && strArr.Length == 2)
//                {
//                    curName = strArr[0];
//                    blankWidth = 0;
//                    blankHeight = 0;
//                    for (j = 0; j < curTextureList.Length; j++)
//                    {
//                        if (curTextureList[j].name == curName)
//                        {
//                            blankWidth = Math.Abs(curTextureList[j].width - textureWidthList[i]);
//                            blankHeight = Math.Abs(curTextureList[j].height - textureHeightList[i]);
//                            if (IsOkComponent(curTextureList[j], textureList[i], strArr[1], blankWidth, blankHeight, textureWidthList[i], textureHeightList[i]))
//                            {
//                                break;
//                            }
//                        }
//                    }
//                }
//            }
//        }
//    } 

//    private static bool IsOkComponent(UIBasicSprite baseSprite,GameObject destroyGo,string typeStr,int blankWidth,int blankHeight,int trueWidth,int trueHeight)
//    {
//        if (typeStr == "you")
//        {
//            if (Math.Abs(baseSprite.transform.localPosition.x - destroyGo.transform.localPosition.x) == trueWidth)
//            {
//                baseSprite.SpriteType = eUISpriteType.eMirror;
//                baseSprite.SetBorder(new Vector4(0, 0, blankWidth, 0));
//                baseSprite.transform.localPosition = new Vector3(baseSprite.transform.localPosition.x + (trueWidth - baseSprite.width / 2), baseSprite.transform.localPosition.y, baseSprite.transform.localPosition.z);
//                baseSprite.width = trueWidth * 2;
//                DestroyImmediate(destroyGo);
//                return true;
//            }
//        }
//        else if (typeStr == "xia")
//        {
//            if (Math.Abs(baseSprite.transform.localPosition.y - destroyGo.transform.localPosition.y) == trueHeight)
//            {
//                baseSprite.SpriteType = eUISpriteType.eMirror;
//                baseSprite.SetBorder(new Vector4(0, blankHeight, 0, 0));
//                baseSprite.transform.localPosition = new Vector3(baseSprite.transform.localPosition.x, baseSprite.transform.localPosition.y - (trueHeight - baseSprite.height / 2), baseSprite.transform.localPosition.z);
//                baseSprite.height = trueHeight * 2;
//                DestroyImmediate(destroyGo);
//                return true;
//            }
//        }
//        else if (typeStr == "youxia")
//        {
//            if (Math.Abs(baseSprite.transform.localPosition.x - destroyGo.transform.localPosition.x) == trueWidth && Math.Abs(baseSprite.transform.localPosition.y - destroyGo.transform.localPosition.y) == trueHeight)
//            {
//                baseSprite.SpriteType = eUISpriteType.eMirror;
//                baseSprite.SetBorder(new Vector4(0, blankHeight, blankWidth, 0));
//                baseSprite.transform.localPosition = new Vector3(baseSprite.transform.localPosition.x + (trueWidth - baseSprite.width / 2), baseSprite.transform.localPosition.y - (trueHeight - baseSprite.height / 2), baseSprite.transform.localPosition.z);
//                baseSprite.width = trueWidth * 2;
//                baseSprite.height = trueHeight * 2;
//                DestroyImmediate(destroyGo);
//                return true;
//            }
//        }
//        return false;
//    }


    static private void createLayer(GameObject parent, PSDUI.Layer layer, PSDUI.Size psdSize)
    {        
        if (layer.scrollview != null)
        {
            createScrollView(parent, layer, psdSize);
        }
        else if (layer.tab != null)
        {
            createTab(parent, layer, psdSize);
        }
        else if(layer.image != null)
        {
            createUISprite(parent, layer, psdSize);
        }
        else if (layer.label != null)
        {
            createLabel(parent, layer, psdSize);
        }
        else if (layer.button != null)
        {
            createButton(parent, layer, psdSize);
        }        
        else if (layer.layers != null)
        {
            GameObject go = new GameObject("", typeof(RectTransform));
            go.name = GetOkNodeName(layer.name);
            go.transform.parent = parent.transform;
            go.transform.localScale = new Vector3(1, 1, 1);
            go.transform.localPosition = Vector3.zero;
            for (int i = 0; i < layer.layers.Length; i++)
            {
                createLayer(go, layer.layers[i], psdSize);
            }
        }
    }

    private static ToggleGroup createTab(GameObject parent, PSDUI.Layer layer, PSDUI.Size psdSize)
    {
        GameObject go = new GameObject(GetOkNodeName(layer.tab.name), typeof(RectTransform), typeof(ToggleGroup));
        go.transform.SetParent(parent.transform, false);
        go.transform.localScale = Vector3.one;
        ToggleGroup tab = go.GetComponent<ToggleGroup>();
        if (layer.tab.btnArr==null)
        {
            Debugger.Log("页签中无按钮.");
            return null;
        }
        for (int i=0;i<layer.tab.btnArr.Length;i++)
        {
            if (layer.tab.btnArr[i].button==null)
            {
                Debugger.Log("页签中存在不是按钮的元素.");
                return null;
            }
        }
        SetTabBtn(tab, layer.tab.btnArr, psdSize,true);
        return tab;
//        GameObject go = new GameObject();
//        go.transform.parent = root.transform;
//        go.name = GetOkNodeName(layer.tab.name);
//        go.transform.localScale = Vector3.one;
//        MyUITab tab = go.AddComponent<MyUITab>();
//        tab.isSetPos = false;
//        if (layer.tab.btnArr==null)
//        {
//            Debug.Log("页签中无按钮.");
//            return null;
//        }
//        for (int i=0;i<layer.tab.btnArr.Length;i++)
//        {
//            if (layer.tab.btnArr[i].button==null)
//            {
//                Debug.Log("页签中存在不是按钮的元素.");
//                return null;
//            }
//        }
//        SetTabBtn(tab, layer.tab.btnArr, psdSize, canvas, depth,true);
//        return tab;
    }

    private static void SetTabBtn(ToggleGroup tab, PSDUI.Layer[] btnArr, PSDUI.Size psdSize, bool isTabBtn=false)
    {
        for (int i = 0; i < btnArr.Length; i++)
        {
            Toggle tog = createToggle(tab.gameObject, btnArr[i], psdSize);
            tab.RegisterToggle(tog);
        }
    }
    
    private static Toggle createToggle(GameObject parent, PSDUI.Layer layer, PSDUI.Size psdSize)
    {
        GameObject go = new GameObject(GetOkNodeName(layer.button.name), typeof(RectTransform), typeof(Toggle));
        go.transform.SetParent(parent.transform, false);
        go.transform.localScale = Vector3.one;
        Toggle tog = go.GetComponent<Toggle>();
        List<PSDUI.Layer> curImage = new List<PSDUI.Layer>();
        List<PSDUI.Layer> curLabel=new List<PSDUI.Layer>();
        if(layer.button.layerArr==null)
        {
            Debug.LogError("按钮生成失败，请检查:" + layer.button.name);
            return go.AddComponent<Toggle>();
        }
        for (int i=0;i<layer.button.layerArr.Length;i++)
        {
            if (layer.button.layerArr[i].image != null)
            {
                curImage.Add(layer.button.layerArr[i]);
            }
            else if (layer.button.layerArr[i].label != null)
            {
                curLabel.Add(layer.button.layerArr[i]);
            }
        }
        tog.transition = Selectable.Transition.None;
        
        SetTogSprite(curImage, tog, psdSize);
        SetTogLabel(curLabel, tog, psdSize);
        return tog;
    }
    
    static private void SetTogSprite(List<PSDUI.Layer> layerArr, Toggle tog, PSDUI.Size psdSize)
    {
        for (int i=0;i<layerArr.Count;i++)
        {
            Graphic img = createUISprite(tog.gameObject, layerArr[i], psdSize, true);
            if (i == 0)
            {
                tog.graphic = img;
            }
        }
    }
    
    static private void SetTogLabel(List<PSDUI.Layer> layerArr, Toggle tog, PSDUI.Size psdSize)
    {
        for (int i = 0; i < layerArr.Count; i++)
        {
            createLabel(tog.gameObject, layerArr[i], psdSize, true, false);
        }
    }

    static private Graphic createUISprite(GameObject parent, PSDUI.Layer layer, PSDUI.Size psdSize, bool isGetPos = true,string btnSpriteName = "")
    {
        if (layer.image.name==null)
        {
            return null;
        }
        if(layer.image.name.IndexOf("@kongbai", StringComparison.Ordinal) != -1)
        {
            GameObject empty = new GameObject("kongbai", typeof(EmptyImage), typeof(RectTransform));
            RectTransform tran = empty.GetComponent<RectTransform>();
            tran.SetParent(parent.transform, false);
            tran.sizeDelta = new Vector2(layer.image.size.width, layer.image.size.height);
            return empty.GetComponent<EmptyImage>();
        }
        string spriteName = layer.image.name;
        if(btnSpriteName != "")
        {
            spriteName = btnSpriteName;
        }
        string nextSpriteName = spriteName;
        if (spriteName.IndexOf("@", StringComparison.Ordinal) != -1)
        {
            spriteName = spriteName.Substring(0, spriteName.IndexOf("@", StringComparison.Ordinal));
        }
        string prefix = spriteName.Substring(0, 1);
        int type;
        if (int.TryParse(prefix, out type))
        {
            spriteName = spriteName.Substring(1);
        }
        string atlasName = GetUIAtlas(spriteName);
        Sprite sprite = GetSprite(atlasName, spriteName);
        if (isGetPos)
        {
            if (!sprite)
            {
                return createTexture(parent, layer, psdSize, spriteName);
            }
        }

        GameObject go = new GameObject("", typeof(Image));
        Image img = go.GetComponent<Image>();
        go.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
        go.transform.parent = parent.transform;
        go.name = GetOkNodeName(nextSpriteName);
        if(nextSpriteName.IndexOf("@", StringComparison.Ordinal) != -1)
        {
            GetResultRot(go, nextSpriteName, (int)(layer.image.size.width), (int)(layer.image.size.height));
        }

        if (sprite)
        {
            bool isSplit = sprite.border.sqrMagnitude > 0;
            if (isSplit)
            {
                img.type = Image.Type.Sliced;
            }
            img.sprite = sprite;
        }
        Vector3 curPos = GetMidPos(parent, layer.image.position, layer.image.size, psdSize, isGetPos);
        go.transform.localPosition = curPos;

        RectTransform recttran = go.GetComponent<RectTransform>(); 
        recttran.sizeDelta = new Vector2(layer.image.size.width,layer.image.size.height );
        return img;
    }

    private static Sprite GetSprite(string atlasName, string spriteName)
    {
        List<UnityEngine.Object> objs = GetAllSprites(atlasName);
        if (null == objs || objs.Count == 0)
        {
            Debug.LogError("Sprite资源不存在：" + getSpritePath(atlasName));
            return null;
        }
        for (int i = 0; i < objs.Count; i++)
        {
            Sprite sp = objs[i] as Sprite;
            if (sp.name == spriteName)
            {
                return sp;
            }
        }
        if (atlasName == "bag")
        {
            atlasName = "skill";
            return GetSprite(atlasName, spriteName);
        }
        
        return null;
    }
    
    private const string ATLAS_SUFFIX = ".png";
    
    static string getSpritePath(string texName)
    {
        if (texName == "skill")
        {
            return "Assets/Art/UI/Atlas/" + texName + "s/" + texName + ATLAS_SUFFIX;
        }
        return "Assets/Art/UI/Atlas/" + texName + "/" + texName + ATLAS_SUFFIX;
    }

    static List<UnityEngine.Object> GetAllSprites(string atlasName)
    {
        string atlasPath;
        if (atlasName == "skill")
        {
            atlasPath = "Assets/Art/UI/Atlas/" + atlasName + "s";
        }
        else
        {
            atlasPath = "Assets/Art/UI/Atlas/" + atlasName;
        }
        if (!Directory.Exists(atlasPath)) return null;
        
        string[] atlas = Directory.GetFiles(atlasPath, "*.png", SearchOption.AllDirectories);
        List<UnityEngine.Object> allSprites = new List<UnityEngine.Object>();
        for (int j = 0; j < atlas.Length; j++)
        {
            UnityEngine.Object[] objs = AssetDatabase.LoadAllAssetRepresentationsAtPath(atlas[j]);
            if (objs != null && objs.Length > 0)
            {
                allSprites.AddRange(objs);
            }
        }
        return allSprites;
    }

    private static void GetResultRot(GameObject go,string name,int width,int height,bool isSprite = true)
    {
        if (isSprite)
        {
            spriteList.Add(go);
            spriteWidthList.Add(width);
            spriteHeightList.Add(height);
        }
        else
        {
            textureList.Add(go);
            textureWidthList.Add(width);
            textureHeightList.Add(height);
        }
        if (name.IndexOf("@you", StringComparison.Ordinal) != -1)
        {
            go.transform.localRotation = Quaternion.Euler(0, 180, 0);
        }
        else if (name.IndexOf("@xia", StringComparison.Ordinal) != -1)
        {
            go.transform.localRotation = Quaternion.Euler(180, 0, 0);
        }
        else if (name.IndexOf("@youxia", StringComparison.Ordinal) != -1)
        {
            go.transform.localRotation = Quaternion.Euler(180, 180, 0);
        }
    }

    static private bool isHasPadVal(RectTransform tran)
    {
        if(tran != null)
        {
            if (tran.pivot.sqrMagnitude > 0)
            {
                return true;
            }
        }
        return false;
    }

    static private Graphic createTexture(GameObject parent, PSDUI.Layer layer, PSDUI.Size psdSize, string okSpriteName)
    {
        if (layer.image.name == null)
        {
            return null;
        }
        string spriteName = layer.image.name;
        RawImage tex = UGUITool.CreateRawImage(parent.transform);
        string[] strArr = okSpriteName.Split('_');
        int width = (int)layer.image.size.width;
        int height = (int)layer.image.size.height;
        
        if (strArr != null && strArr.Length > 0)
        {
            string startPath = "Assets/Resources/Texture/" + strArr[0].Substring(0, 1).ToUpper() + strArr[0].Substring(1) + "/";
            string path = startPath + okSpriteName + ".png";
//            string rgbPath = startPath + okSpriteName + "_rgb.png";
//            string alphaPath = startPath + okSpriteName + "_alpha.png";
            Texture texture = AssetDatabase.LoadMainAssetAtPath(path) as Texture;
            if (!texture)
            {
                startPath = "Assets/Art/UI/Atlas/" + strArr[0] + "/";
                path = startPath + okSpriteName + ".png";
                texture = AssetDatabase.LoadMainAssetAtPath(path) as Texture;
            }
            if(texture != null)
            {
                tex.texture = texture;
//                layer.image.size.width = texture.width;
//                layer.image.size.height = texture.height;
            }
            else
            {
                Debug.LogError("该图片在本项目不存在：" + okSpriteName);
            }
        }       
        GameObject go = tex.gameObject;
        go.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
        go.transform.parent = parent.transform;
        go.name = GetOkNodeName(spriteName);

        if (spriteName.IndexOf("@", StringComparison.Ordinal) != -1)
        {
            GetResultRot(go, spriteName, width, height, false);
        }
        go.transform.localPosition = GetMidPos(parent, layer.image.position, layer.image.size, psdSize);
        RectTransform rectTran = go.GetComponent<RectTransform>();
        rectTran.sizeDelta = new Vector2(layer.image.size.width, layer.image.size.height);
        return tex;
    }

    static private void createScrollView(GameObject parent,PSDUI.Layer layer, PSDUI.Size psdSize)
    {
        ScrollRect panel = UGUITool.CreateScrollView(parent.transform);
        GameObject go = panel.gameObject;
        go.transform.parent = parent.transform;
        go.name = GetOkNodeName(layer.scrollview.name);
        PSDUI.Size size = layer.scrollview.size;
        panel.GetComponent<RectTransform>().sizeDelta = new Vector2(size.width, size.height);
        panel.horizontal = false;
        Vector3 getPos = GetMidPos(parent, layer.scrollview.position, layer.scrollview.size, psdSize);
        Vector3 endPos = new Vector3(getPos.x, getPos.y, 0);
        go.transform.localPosition = endPos;
        Transform gridTran = panel.transform.Find("Viewport");
        gridTran.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
        DestroyImmediate(panel.transform.Find("Scrollbar Horizontal").gameObject);
        DestroyImmediate(panel.transform.Find("Scrollbar Vertical").gameObject);
        var list = go.GetComponentsInChildren<Graphic>();
        foreach (var graphic in list)
        {
            DestroyImmediate(graphic);
        }
        var list1 = go.GetComponentsInChildren<CanvasRenderer>();
        foreach (var graphic in list1)
        {
            DestroyImmediate(graphic);
        }
    }

    static private Text createLabel(GameObject root, PSDUI.Layer layer, PSDUI.Size psdSize, bool isGetPos=true,bool isSetCenter = false)
    {
        Text lab = UGUITool.CreateTextGamma(root.transform);
        GameObject go = lab.gameObject;
        go.transform.parent = root.transform;
        go.name = GetOkNodeName(layer.label.name);

        Color col = Color.white;
        try
        {
            col = GetColorValue(layer.label.arguments[0]);
        }
        catch (Exception e)
        {
            Debug.LogError("获取颜色值失败，xml有问题");
        }
        lab.color = col;
        float fontsize;
        float.TryParse(layer.label.arguments[2], out fontsize);
        lab.fontSize = Mathf.RoundToInt(fontsize);
        RectTransform rectTran = go.GetComponent<RectTransform>();
        
        lab.text = layer.label.arguments[3].Replace("《？《", "<").Replace("》？》", ">");
        rectTran.sizeDelta = new Vector2(layer.label.size.width, Mathf.Max(lab.preferredHeight + 1, layer.label.size.height));
        if(isSetCenter)
        {
            lab.verticalOverflow = VerticalWrapMode.Overflow;
            lab.horizontalOverflow = HorizontalWrapMode.Overflow;
        }
        else
        {
            lab.verticalOverflow = VerticalWrapMode.Truncate;
            lab.horizontalOverflow = HorizontalWrapMode.Wrap;
        }        
        lab.supportRichText = true;
        
        Vector3 curPos = GetLabelPos(root, layer.label.position, layer.label.size, psdSize, isGetPos);
//        curPos.x += Mathf.Floor(rectTran.sizeDelta.x * 0.5f);
//        curPos.y -= Mathf.Ceil(rectTran.sizeDelta.y * 0.5f);
//        if (isSetCenter)
//        {
//            lab.alignment= TextAnchor.MiddleCenter;
//        }
//        else
//        {
            lab.alignment = TextAnchor.UpperLeft;
//        }
//        rectTran.pivot = new Vector2(0f, 1f);
//        go.transform.localPosition = curPos;
        if (layer.label.arguments[1].IndexOf("FZCS", StringComparison.Ordinal) != -1)
        {
            lab.font = AssetDatabase.LoadAssetAtPath<Font>("Assets/Resources/Font/FZCSK.TTF");
//            if (lab.fontSize > 11)
//            {
//                if(lab.fontSize <35)
//                {
//                    go.transform.localPosition = new Vector3(curPos.x, curPos.y - 1, curPos.z);
//                }
//                else if(lab.fontSize < 45)
//                {
//                    go.transform.localPosition = new Vector3(curPos.x, curPos.y - 2, curPos.z);
//                }  
//                else
//                {
//                    go.transform.localPosition = new Vector3(curPos.x, curPos.y - 3, curPos.z);
//                }  
//            }    
            rectTran.sizeDelta = new Vector2(rectTran.sizeDelta.x, rectTran.sizeDelta.y + 2);   
        }
        else
        {
            lab.font = AssetDatabase.LoadAssetAtPath<Font>("Assets/Resources/Font/FZHTJW.TTF");
//            lab.trueTypeFont = MusUICheckUtil.GetFont("FZHTJW");
        }
        rectTran.sizeDelta = new Vector2(rectTran.sizeDelta.x + 1, rectTran.sizeDelta.y);
        Vector3 size = rectTran.sizeDelta;
        if (size.x % 2 > 0) size.x += 1;
        if (size.y % 2 > 0) size.y += 1;
        rectTran.sizeDelta = size;
        curPos.x += rectTran.sizeDelta.x * 0.5f;
        curPos.y -= rectTran.sizeDelta.y * 0.5f;
        rectTran.pivot = new Vector2(0.5f, 0.5f);
        go.transform.localPosition = curPos;

//        lab.applyGradient = false;
        lab.lineSpacing = 1;
//        lab.floatSpacingX = 0;
//        lab.floatSpacingY = 5;
        lab.fontStyle = FontStyle.Normal;

        float activeY = 0;

        if (layer.label.labelExtraData != null)
        {
            //行距离
//            if (layer.label.labelExtraData.paddingY != 0)
//            {
//                int offset = Mathf.RoundToInt(layer.label.labelExtraData.paddingY);
//                lab.lineSpacing = ((float)offset / lab.fontSize) * 0.837f;
////                activeY = lab.lineSpacing;
//            }
            //增加行距需要的高度
            //if (activeY > 0)
            //{
            //    lab.height += (int)(UnityEngine.Mathf.CeilToInt(lab.height / lab.fontSize) * activeY) + UnityEngine.Mathf.CeilToInt(lab.fontSize * 0.1f);
            //}

//            int activeX = 0;
//            if (layer.label.labelExtraData.paddingX != 0)
//            {
//                activeX = Mathf.RoundToInt((int)(layer.label.labelExtraData.paddingX) * 0.001f * lab.fontSize);
//                lab.spacingX = activeX;
//                //lab.width += activeX * lab.text.Length + UnityEngine.Mathf.CeilToInt(lab.fontSize * 0.1f);//会出现显示不全的问题
//            }

            bool isBold = layer.label.labelExtraData.isBold;
            bool isItalic = layer.label.labelExtraData.isItalic;
            if (isBold && isItalic)
            {
                MakeFontBoldAndItalic(lab);
            }
            else if (isBold)
            {
                MakeFontBold(lab);
            }
            else if (isItalic)
            {
                MakeFontItalic(lab);
            }

            //描边
//            if (!string.IsNullOrEmpty(layer.label.labelExtraData.effectColor))
//            {
//                if (!string.IsNullOrEmpty(layer.label.labelExtraData.effectSize) && layer.label.labelExtraData.effectSize != "0")
//                {
//                    OutlineBetter outline = lab.gameObject.AddComponent<OutlineBetter>();
//                    outline.effectColor = GetColorValue(layer.label.labelExtraData.effectColor, layer.label.labelExtraData.effectAlpha);
//                    float effectSize = float.Parse(layer.label.labelExtraData.effectSize);
//                    outline.effectDistance = new Vector2(effectSize, effectSize);
//                }
//            }

            //判断是否单行
//            List<Vector3> tempVerts = new List<Vector3>();
//            List<int> tempIndices = new List<int>();
//            NGUIText.PrintApproximateCharacterPositions(lab.text, tempVerts, tempIndices);
//            if (tempVerts.Count > 1)
//            {
//                if (tempVerts[0].y == tempVerts[tempVerts.Count - 1].y)
//                {
//                    if (lab.spacingY != 0)
//                    {
//                        lab.spacingY = 0;
//                    }
//                    if (lab.floatSpacingY != 0f)
//                    {
//                        lab.floatSpacingY = 0;
//                    }
//                }
//            }
        }


        //放在最后面
//        lab.verticalOverflow = VerticalWrapMode.Overflow;
//        lab.horizontalOverflow = HorizontalWrapMode.Overflow;

        return lab;
    }

    private static void MakeFontBold(Text lab)
    {
        lab.fontStyle = FontStyle.Bold;
//        lab.text = lab.text.Insert(0, "[b]");
//        lab.text = lab.text.Insert(lab.text.Length, "[-]");
    }

    private static void MakeFontItalic(Text lab)
    {
        lab.fontStyle = FontStyle.Italic;
//        lab.text = lab.text.Insert(0, "[i]");
//        lab.text = lab.text.Insert(lab.text.Length, "[-]");
    }

    private static void MakeFontBoldAndItalic(Text lab)
    {
        lab.fontStyle = FontStyle.BoldAndItalic;
    }

    static private void createButton(GameObject parent, PSDUI.Layer layer, PSDUI.Size psdSize, bool isTabBtn=false)
    {
        GameObject go = new GameObject("", typeof(RectTransform));
        go.transform.SetParent(parent.transform, false);
        go.name = GetOkNodeName(layer.button.name);
        go.transform.localScale = Vector3.one;
        if(layer.button.layerArr == null)
        {
            Debug.LogError(layer.button.name + "此按钮下无按钮元素");
            return;
        }
        List<PSDUI.Layer> curImage = new List<PSDUI.Layer>();
        List<PSDUI.Layer> curLabel =new List<PSDUI.Layer>();
        for (int i=0;i<layer.button.layerArr.Length;i++)
        {
            if (layer.button.layerArr[i].image != null)
            {
                curImage.Add(layer.button.layerArr[i]);
            }
            else if (layer.button.layerArr[i].label != null)
            {
                curLabel.Add(layer.button.layerArr[i]);
            }
        }
//        Button myBtn = go.AddComponent<Button>();
//        myBtn.transition = Selectable.Transition.None;
//        myBtn.duration = 0;
//        myBtn.hover = Color.white;
//        myBtn.pressed = Color.white;
//        myBtn.disabledColor = Color.white;
        SetBtnSprite(curImage, go, psdSize);
        SetBtnLabel(curLabel, go, psdSize, isTabBtn);
//        UIButtonScale btnScale = myBtn.GetComponent<UIButtonScale>();
//        if (myBtn.normalSprite != myBtn.disabledSprite && btnScale == null)
//        {
//            btnScale = myBtn.gameObject.AddComponent<UIButtonScale>();
//            btnScale.hover = Vector3.one;
//            btnScale.pressed = new Vector3(1.1f, 1.1f, 1.1f);
//            btnScale.tweenTarget = myBtn.transform;
//        }
//        TweenScale tweenScale = myBtn.GetComponent<TweenScale>();
//        if (tweenScale == null)
//        {
//            tweenScale = myBtn.gameObject.AddComponent<TweenScale>();
//        }
        //if(isTabBtn && myBtn.disabledChange == false)
        //{
        //    myBtn.disabledChange = true;
        //    myBtn.disabledColor = new Color(172 / 255f, 171 / 255f, 171 / 255f);
        //}       
//        return myBtn;
    }

    static private void SetBtnSprite(List<PSDUI.Layer> layerArr,GameObject btn, PSDUI.Size psdSize)
    {
        for (int i = 0; i < layerArr.Count; i++)
        {
            int type = 0;
            int.TryParse(layerArr[i].image.name.Substring(0, 1), out type);
            if (type != 1 && type != 2 && type != 3 && type != 4)
            { 
                createUISprite(btn, layerArr[i], psdSize);
            }
            else
            {
                var spriteName = layerArr[i].image.name.Substring(1);
                createUISprite(btn, layerArr[i], psdSize, true, spriteName);
            }
        }
    }

    static private void SetBtnLabel(List<PSDUI.Layer> layerArr, GameObject btn, PSDUI.Size psdSize, bool isTabBtn=false)
    {
        Text text = btn.gameObject.GetComponentInChildren<Text>();
        if (text) DestroyImmediate(text.gameObject);
        bool isFirst = true;
        for (int i = 0; i < layerArr.Count; i++)
        {
            if(isFirst)
            {
                isFirst = false;
                createLabel(btn.gameObject, layerArr[i], psdSize, true, true);
            }
        }
    }

    static private int GetMaxValue(int val1,int val2)
    {
        if (val1>=val2)
        {
            return val1;
        }
        return val2;
    }

    static private Color SetLabelEffectColor(PSDUI.LabelData lab)
    {
        Color curColor = Color.white;
        if (lab.name.Contains("@miaobian"))
        {
            int index = lab.name.IndexOf("@", StringComparison.Ordinal) + 9;
            string miaobianColor = lab.name.Substring(index);
            miaobianColor = miaobianColor.Trim();
            curColor = GetColorValue(miaobianColor);
            curColor = new Color(curColor.r, curColor.g, curColor.g, 128 / 255f);
        }
        return curColor;
    }

    static private Color GetColorValue(string str, float alpha = 100)
    {
        if (str=="")
        {
            return new Color(38 / 255f, 6 / 255f, 6 / 255f, 128 / 255f);
        }
        List<string> colorStr = new List<string>();
        for (int i = 0; i < 3; i++)
        {
            colorStr.Add(str.Substring(i * 2, 2));
        }
        int num = 0;
        num = Int32.Parse(colorStr[0], System.Globalization.NumberStyles.HexNumber);
        float r = num / 255.0f;
        num = Int32.Parse(colorStr[1], System.Globalization.NumberStyles.HexNumber);
        float g = num / 255.0f; 
        num = Int32.Parse(colorStr[2], System.Globalization.NumberStyles.HexNumber);
        float b = num / 255.0f;

        float a = alpha / 100f;

        Color color = new Color(r, g, b, a);

        return color;
    }

    static private object DeserializeXml (string content, System.Type type)
    {
        object instance = null;
        if (content != null)
        {
            string xml = content.Replace(" px","");
            xml = xml.Replace(" py",""); 
            if ((xml != null) && (xml.ToString () != ""))
            { 
                XmlSerializer xs = new XmlSerializer (type); 
                UTF8Encoding encoding = new UTF8Encoding (); 
                byte[] byteArray = encoding.GetBytes (xml); 
                MemoryStream memoryStream = new MemoryStream (byteArray); 
                XmlTextWriter xmlTextWriter = new XmlTextWriter (memoryStream, Encoding.UTF8);
                if (xmlTextWriter != null)
                {
                    instance = xs.Deserialize (memoryStream);
                }
            }
        }
        return instance;
    }

    static private Vector3 GetMidPos(GameObject parent, PSDUI.Position pos,PSDUI.Size size, PSDUI.Size psdSize, bool isGetPos=true)
    {
        if (isGetPos==false)
        {
            return Vector3.zero;
        }
        if (panelRoot==null||parent==null||pos==null||size==null|| psdSize== null)
        {
            return Vector3.zero;
        }
        Vector3 curPos = new Vector3(pos.x, pos.y, 0);
        curPos.x += size.width * 0.5f;
        curPos.y -= size.height * 0.5f;
        Vector3 posW = panelRoot.transform.localToWorldMatrix.MultiplyPoint3x4(curPos);
        Vector3 posS = parent.transform.worldToLocalMatrix.MultiplyPoint3x4(posW);
        return posS;
    }

    static private Vector3 GetLabelPos(GameObject parent, PSDUI.Position pos, PSDUI.Size size, PSDUI.Size psdSize, bool isGetPos = true)
    {
        if (isGetPos == false)
        {
            return Vector3.zero;
        }
        if (panelRoot == null || parent == null || pos == null || size == null || psdSize == null)
        {
            return Vector3.zero;
        }         
        Vector3 curPos = new Vector3(pos.x, pos.y, 0);
        Vector3 posW = panelRoot.transform.localToWorldMatrix.MultiplyPoint3x4(curPos);
        Vector3 posS = parent.transform.worldToLocalMatrix.MultiplyPoint3x4(posW);
//        posS = new Vector3(Mathf.RoundToInt(posS.x), Mathf.RoundToInt(posS.y), Mathf.RoundToInt(posS.z));
        return posS;
    }

//    static private int GetUIAnchor(GameObject go)
//    {
//        UIWidget widget = go.GetComponent<UIWidget>();
//        int curPivot = -1;
//        if (widget != null)
//        {
//            curPivot = (int)(widget.pivot);
//        }
//        else
//        {
//            UIWidgetContainer widgetContainer = go.GetComponent<UIWidgetContainer>();
//            if (widgetContainer != null)
//            {
//                curPivot = 4;
//            }
//            else
//            {
//                curPivot = 4;
//            }
//            
//        }
//        if (curPivot != -1)
//        {
//            return curPivot;
//        }
//        return -1;
//    }

    static private string GetUIAtlas(string name)
    {
        string atlasName = "";
        int startIndex = -1;
        //int endIndex = -1;
        //int length = 0;
        //if (name.Contains("atlas"))
        //{
        //    startIndex = name.IndexOf("_");
        //    endIndex = name.LastIndexOf("_");
        //    length = endIndex - startIndex-1;
        //    atlasName = name.Substring(startIndex+1, length);
        //    if (name.Contains("common"))
        //    {
        //        atlasName = "comm";
        //    }
        //    return atlasName;
        //}

        //if (name.Contains("common"))
        //{
        //    atlasName = "comm";
        //    return atlasName;
        //}
        int result = 0;
        //if (name.StartsWith("i") || int.TryParse(name.Substring(1), out result))
        //{
        //    //背包图集
        //    atlasName = "bag";
        //    return atlasName;
        //}

        if (name.StartsWith("card_quality") || name.StartsWith("card_type") || name.StartsWith("card_select") || name.StartsWith("star"))
        {
            return "comm";
        }

        if (name.StartsWith("mainui_action") || name.StartsWith("mainui_function"))
        {
            return "npcfunctionicon";
        }
        startIndex = name.IndexOf("_", StringComparison.Ordinal);
        if (startIndex > 0)
        {
            atlasName = name.Substring(0, startIndex);
        }
        

        return atlasName;
    }

    //static private string GetSpriteName(string name)
    //{
    //    string spriteName = "";
    //    if (name.Contains("atlas"))
    //    {
    //        return name;
    //    }
    //    if (name.Contains("common"))
    //    {
    //        spriteName = "atlas_" + name;
    //        return spriteName;
    //    }
    //    if (name.Contains("copy"))
    //    {
    //        spriteName = "atlas_" + name;
    //        return spriteName;
    //    }

    //    return spriteName;
    //}

//    static public bool IsChangeSplit(UIAtlas atlas,string spriteName)
//    {
//        if(atlas == null)
//        {
//            return false;
//        }
//        UISpriteData data = atlas.GetSprite(spriteName);
//        if (data==null)
//        {
//            return false;
//        }
//        if (data.borderTop!=0||data.borderBottom!=0||data.borderLeft!=0||data.borderRight!=0)
//        {
//            return true;
//        }
//        return false;
//    }
    private static int LayerCount;
    private static Regex CnReg = new Regex("[\u4e00-\u9fa5]");
    private static Regex EnReg = new Regex("[A-Z]");

    static public string GetOkNodeName(string nodeName)
    {
        if (string.IsNullOrEmpty(nodeName)) return nodeName;
        LayerCount++;

        //包含中文
        if (CnReg.IsMatch(nodeName))
        {
            nodeName = "layer-auto" + LayerCount;
        }
        //包含大写字母
        if (EnReg.IsMatch(nodeName))
        {
            nodeName = nodeName.ToLower();
        }

        return nodeName;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Text;

public class WinAtlasViewer : EditorWindow
{
    GameObject win;
    GameObject oldwin;
    Vector2 scroll = new Vector2();
    List<Image> winimages = new List<Image>();
    List<string> useatlas = new List<string>();
    string selectatlas = "全部";
    int select = 0;
    Sprite checkSprite = null;
    Texture2D checkAtlas = null;
    string checkDirectoryPath = "";
    List<string> functionname = new List<string>();
    int selectfunction = 0;
    Dictionary<GameObject, List<string>> checkSpriteData = new Dictionary<GameObject, List<string>>();
    List<GameObject> checkAtlasData = new List<GameObject>();
    List<GameObject> winlist = new List<GameObject>();
    List<GameObject> checkdirectorywinlist = new List<GameObject>();
    Dictionary<Sprite, List<GameObject>> checkSpriteWinData = new Dictionary<Sprite, List<GameObject>>();
    Dictionary<GameObject,int> checkSpriteWinDataTime = new Dictionary<GameObject, int>();
    string numselect = "";
    string searchStr = "";
    private string dircheckresult = "";
    private string dircheckfliteratlas = "comm;comm2;comm3;comm4;mainui;mainui2;mainui3;bag0;bag1;bag2;bag3;bag4;bag5;bag6;bag7;bag8;bag9;bag10;head0;head1;head2;head3;skill0;skill1;skill2;skill3;huanicon;hunhuanicon;shenzhuangicon;hunguicon";

    [MenuItem("AMLD/UI工具/查看界面所用图集列表")]
    static void Init()
    {
        var window = GetWindow<WinAtlasViewer>(true, "界面所用图集列表");
        window.Show();
    }

    private void OnGUI()
    {
        if(functionname.Count<=0)
        {
            functionname.Add("界面所用图集列表");
            functionname.Add("查看文件夹下界面用的图集次数");
            functionname.Add("查找引用了某个图集的界面");
            functionname.Add("查找引用图集里图片的界面");
            functionname.Add("查找图集里图片引用的次数");
        }
        selectfunction = GUILayout.Toolbar(selectfunction, functionname.ToArray());
        if (selectfunction==0)
        {
            GUILayout.BeginHorizontal();
            win = EditorGUILayout.ObjectField(win, typeof(GameObject), true) as GameObject;
            if (GUILayout.Button("刷新", GUILayout.Width(100)))
            {
                oldwin = null;
            }
            GUILayout.EndHorizontal();
            if (oldwin != win)
            {
                selectatlas = "全部";
                winimages.Clear();
                useatlas.Clear();
                if (win != null)
                {
                    useatlas.Add("全部");
                    select = 0;
                    Image image = win.GetComponent<Image>();
                    if (image != null)
                    {
                        winimages.Add(image);
                        if (image.sprite != null)
                        {
                            if (!useatlas.Contains(image.sprite.texture.name))
                            {
                                useatlas.Add(image.sprite.texture.name);
                            }
                        }
                        else
                        {
                            if (!useatlas.Contains("空"))
                            {
                                useatlas.Add("空");
                            }
                        }
                    }
                    Image[] images = win.GetComponentsInChildren<Image>(true);
                    if (images != null && images.Length > 0)
                    {
                        for (int i = 0; i < images.Length; i++)
                        {
                            Image temp = images[i];
                            winimages.Add(temp);
                            if (temp.sprite != null)
                            {
                                if (!useatlas.Contains(temp.sprite.texture.name))
                                {
                                    useatlas.Add(temp.sprite.texture.name);
                                }
                            }
                            else
                            {
                                if (!useatlas.Contains("空"))
                                {
                                    useatlas.Add("空");
                                }
                            }
                        }
                    }
                }
                oldwin = win;
            }
            if (win != null)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("图集筛选", GUILayout.Width(200));
                GUILayout.Label("图片名称搜索：", GUILayout.Width(80));
                searchStr = GUILayout.TextField(searchStr, GUILayout.Width(200));
                GUILayout.EndHorizontal();
                select = GUILayout.Toolbar(select, useatlas.ToArray());
                selectatlas = useatlas[select];
                GUILayout.BeginHorizontal();
                GUILayout.Label("物体", GUILayout.Width(200));
                GUILayout.Label("图集名称", GUILayout.Width(150));
                GUILayout.Label("图片名称", GUILayout.Width(150));
                GUILayout.EndHorizontal();
                scroll = GUILayout.BeginScrollView(scroll);
                if (winimages.Count > 0)
                {
                    for (int i = 0; i < winimages.Count; i++)
                    {
                        Image temp = winimages[i];
                        GUILayout.BeginHorizontal();
                        if (temp.sprite != null && (selectatlas == "全部" || selectatlas == temp.sprite.texture.name) &&
                            (searchStr == "" || temp.sprite.name.Contains(searchStr) ))
                        {
                            EditorGUILayout.ObjectField(temp, typeof(Image), true, GUILayout.Width(200));
                            EditorGUILayout.ObjectField(temp.sprite.texture, typeof(Texture2D), true, GUILayout.Width(150));
                            //GUILayout.Label(temp.sprite.texture.name, GUILayout.Width(150));
                            GUILayout.Label(temp.sprite.name, GUILayout.Width(150));
                        }
                        else if (temp.sprite == null && (selectatlas == "全部" || selectatlas == "空"))
                        {
                            EditorGUILayout.ObjectField(temp, typeof(Image), true, GUILayout.Width(200));
                            GUILayout.Label("空", GUILayout.Width(150));
                            GUILayout.Label("空", GUILayout.Width(150));
                        }
                        GUILayout.EndHorizontal();
                    }
                }
                GUILayout.EndScrollView();
            }
        }
        else if (selectfunction == 1)
        {
            GUILayout.BeginHorizontal();
            if (mouseOverWindow == this)
            {
                if (Event.current.type == EventType.DragUpdated)
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
                }
                else if (Event.current.type == EventType.DragExited)
                {
                    Focus();
                    if(DragAndDrop.paths!=null)
                    {
                        for (int i = 0; i < DragAndDrop.paths.Length; i++)
                        {
                            checkDirectoryPath = DragAndDrop.paths[i];
                            break;
                        }
                    }
                }
            }
            checkDirectoryPath = EditorGUILayout.TextField("拖动文件夹到窗口：",checkDirectoryPath,GUILayout.Width(600));
            if (GUILayout.Button("刷新", GUILayout.Width(200)))
            {
                if (!Directory.Exists(checkDirectoryPath.Replace("Assets", Application.dataPath)))
                {
                    return;
                }
                checkdirectorywinlist.Clear();
                List<string> filter = new List<string>(dircheckfliteratlas.Split(';'));
                string[] paths = Directory.GetFiles(checkDirectoryPath.Replace("Assets",Application.dataPath), "*.prefab", SearchOption.AllDirectories);
                for (int i = 0; i < paths.Length; i++)
                {
                    string path = paths[i].Replace("\\", "/").Replace(Application.dataPath, "Assets");
                    GameObject win = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
                    checkdirectorywinlist.Add(win);
                }
                Dictionary<string, List<string>> useatlas = new Dictionary<string, List<string>>();
                for (int i = 0; i < checkdirectorywinlist.Count; i++)
                {
                    var win = checkdirectorywinlist[i];
                    if (win != null)
                    {
                        useatlas.Add(win.name,new List<string>());
                        Image image = win.GetComponent<Image>();
                        if (image != null && image.sprite != null)
                        {
                            if (!useatlas[win.name].Contains(image.sprite.texture.name)&&!filter.Contains(image.sprite.texture.name))
                            {
                                useatlas[win.name].Add(image.sprite.texture.name);
                            }
                        }
                        Image[] images = win.GetComponentsInChildren<Image>(true);
                        if (images != null && images.Length > 0)
                        {
                            for (int j = 0; j < images.Length; j++)
                            {
                                if (images[j] != null && images[j].sprite != null)
                                {
                                    if (!useatlas[win.name].Contains(images[j].sprite.texture.name)&&!filter.Contains(images[j].sprite.texture.name))
                                    {
                                        useatlas[win.name].Add(images[j].sprite.texture.name);
                                    }
                                }
                            }
                        }
                        useatlas[win.name].Sort();
                    }
                }
                checkdirectorywinlist.Clear();
                StringBuilder sb = new StringBuilder();
                var e = useatlas.GetEnumerator();
                while (e.MoveNext())
                {
                    sb.Append(e.Current.Key + "：");
                    for (int i = 0; i < e.Current.Value.Count; i++)
                    {
                        sb.Append(e.Current.Value[i] + "，");
                    }
                    sb.Append("\n");
                }
                e.Dispose();
                dircheckresult = sb.ToString();
            }
            GUILayout.EndHorizontal();
            GUILayout.Label("过滤的图集名称，;分隔：");
            dircheckfliteratlas = GUILayout.TextField(dircheckfliteratlas);
            GUILayout.Label("引用图集情况");
            scroll = GUILayout.BeginScrollView(scroll);
            GUILayout.TextField(dircheckresult);
            GUILayout.EndScrollView();
        }
        else if(selectfunction==2)
        {
            GUILayout.BeginHorizontal();
            checkAtlas = (Texture2D)EditorGUILayout.ObjectField(checkAtlas, typeof(Texture2D), true, GUILayout.Width(300));
            if (GUILayout.Button("刷新", GUILayout.Width(200)))
            {
                if (checkAtlas != null)
                {
                    checkAtlasData.Clear();
                    if (winlist.Count <= 0)
                    {
                        string[] paths = Directory.GetFiles(Application.dataPath + "/Resources/UI", "*.prefab", SearchOption.AllDirectories);
                        for (int i = 0; i < paths.Length; i++)
                        {
                            string path = paths[i].Replace("\\", "/").Replace(Application.dataPath, "Assets");
                            GameObject win = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
                            winlist.Add(win);
                        }
                    }
                    for (int i = 0; i < winlist.Count; i++)
                    {
                        var win = winlist[i];
                        if (win != null)
                        {
                            MaskableGraphic image = win.GetComponent<MaskableGraphic>();
                            if (image != null && image.mainTexture == checkAtlas)
                            {
                                checkAtlasData.Add(win);
                                continue;
                            }
                            MaskableGraphic[] images = win.GetComponentsInChildren<MaskableGraphic>(true);
                            if (images != null && images.Length > 0)
                            {
                                for (int j = 0; j < images.Length; j++)
                                {
                                    if (images[j] != null && images[j].mainTexture == checkAtlas)
                                    {
                                        checkAtlasData.Add(win);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            GUILayout.EndHorizontal();
            scroll = GUILayout.BeginScrollView(scroll);
            if (checkAtlasData.Count > 0)
            {
                GUILayout.Label("使用此图集的界面");
                for(int i =0;i<checkAtlasData.Count;i++)
                {
                    EditorGUILayout.ObjectField(checkAtlasData[i], typeof(GameObject), true);
                }
            }
            GUILayout.EndScrollView();
        }
        else if(selectfunction==3)
        {
            GUILayout.BeginHorizontal();
            checkSprite = (Sprite)EditorGUILayout.ObjectField(checkSprite, typeof(Sprite), true, GUILayout.Width(300));
            if(GUILayout.Button("刷新",GUILayout.Width(200)))
            {
                if(checkSprite!=null)
                {
                    checkSpriteData.Clear();
                    if (winlist.Count <= 0)
                    {
                        string[] paths = Directory.GetFiles(Application.dataPath + "/Resources/UI", "*.prefab", SearchOption.AllDirectories);
                        for (int i = 0; i < paths.Length; i++)
                        {
                            string path = paths[i].Replace("\\", "/").Replace(Application.dataPath, "Assets");
                            GameObject win = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
                            winlist.Add(win);
                        }
                    }
                    for (int i = 0; i < winlist.Count; i++)
                    {
                        var temp = winlist[i];
                        if (temp != null)
                        {
                            Image image = temp.GetComponent<Image>();
                            if (image != null && image.sprite != null && image.sprite == checkSprite)
                            {
                                if (!checkSpriteData.ContainsKey(temp))
                                {
                                    checkSpriteData.Add(temp, new List<string>());
                                }
                                checkSpriteData[temp].Add(image.transform.name);
                            }
                            Image[] images = temp.GetComponentsInChildren<Image>(true);
                            if (images != null && images.Length > 0)
                            {
                                for (int j = 0; j < images.Length; j++)
                                {
                                    if (images[j] != null && images[j].sprite != null && images[j].sprite == checkSprite)
                                    {
                                        if (!checkSpriteData.ContainsKey(temp))
                                        {
                                            checkSpriteData.Add(temp, new List<string>());
                                        }
                                        checkSpriteData[temp].Add(images[j].transform.name);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            GUILayout.EndHorizontal();
            scroll = GUILayout.BeginScrollView(scroll);
            if(checkSpriteData.Count>0)
            {
                var e = checkSpriteData.GetEnumerator();
                while(e.MoveNext())
                {
                    GUILayout.Label("----------------------");
                    EditorGUILayout.ObjectField(e.Current.Key, typeof(GameObject), true);
                    GUILayout.Label("使用此图集图片的子物体");
                    for(int i=0;i<e.Current.Value.Count;i++)
                    {
                        if(i%5==0)
                        {
                            GUILayout.BeginHorizontal();
                        }
                        GUILayout.TextField(e.Current.Value[i]);
                        if(i==e.Current.Value.Count-1||i%5==4)
                        {
                            GUILayout.EndHorizontal();
                        }
                    }
                }
                e.Dispose();
            }
            GUILayout.EndScrollView();
        }
        else if(selectfunction==4)
        {
            GUILayout.BeginHorizontal();
            checkAtlas = (Texture2D)EditorGUILayout.ObjectField(checkAtlas, typeof(Texture2D), true, GUILayout.Width(300));
            if (GUILayout.Button("刷新", GUILayout.Width(200)))
            {
                if (checkAtlas != null)
                {
                    checkSpriteWinData.Clear();
                    if (winlist.Count <= 0)
                    {
                        string[] paths = Directory.GetFiles(Application.dataPath + "/Resources/UI", "*.prefab", SearchOption.AllDirectories);
                        for (int i = 0; i < paths.Length; i++)
                        {
                            string path = paths[i].Replace("\\", "/").Replace(Application.dataPath, "Assets");
                            GameObject win = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
                            winlist.Add(win);
                        }
                    }
                    string atlaspath = AssetDatabase.GetAssetPath(checkAtlas);
                    Object[] objs = AssetDatabase.LoadAllAssetRepresentationsAtPath(atlaspath);
                    if(objs.Length>0)
                    {
                        for (int i = 0; i < objs.Length; i++)
                        {
                            Sprite sp = objs[i] as Sprite;
                            checkSpriteWinData.Add(sp, new List<GameObject>());
                        }
                    }
                    for (int i = 0; i < winlist.Count; i++)
                    {
                        var win = winlist[i];
                        if (win != null)
                        {
                            Image image = win.GetComponent<Image>();
                            if (image != null && image.sprite != null)
                            {
                                if (checkSpriteWinData.ContainsKey(image.sprite))
                                {
                                    checkSpriteWinData[image.sprite].Add(win);
                                }
                            }
                            Image[] images = win.GetComponentsInChildren<Image>(true);
                            if (images != null && images.Length > 0)
                            {
                                for (int j = 0; j < images.Length; j++)
                                {
                                    if (images[j] != null && images[j].sprite != null)
                                    {
                                        if (checkSpriteWinData.ContainsKey(images[j].sprite))
                                        {
                                            checkSpriteWinData[images[j].sprite].Add(win);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            GUILayout.Label("数量筛选,输入如(>3,=7,<10)");
            numselect = GUILayout.TextField(numselect);
            GUILayout.EndHorizontal();
            scroll = GUILayout.BeginScrollView(scroll);
            if (checkSpriteWinData.Count > 0)
            {
                int type = 0;
                int compare = 0;
                if(numselect.StartsWith(">"))
                {
                    string temp = numselect.Remove(0,1);
                    if(int.TryParse(temp,out compare))
                    {
                        type = 1;
                    }
                }
                else if(numselect.StartsWith("="))
                {
                    string temp = numselect.Remove(0,1);
                    if (int.TryParse(temp, out compare))
                    {
                        type = 2;
                    }
                }
                else if (numselect.StartsWith("<"))
                {
                    string temp = numselect.Remove(0,1);
                    if (int.TryParse(temp, out compare))
                    {
                        type = 3;
                    }
                }
                var e = checkSpriteWinData.GetEnumerator();
                while (e.MoveNext())
                {
                    if(type==0||type==1&&e.Current.Value.Count>compare||type==2&&e.Current.Value.Count==compare||type==3&&e.Current.Value.Count<compare)
                    {
                        GUILayout.Label("----------------------");
                        EditorGUILayout.ObjectField(e.Current.Key, typeof(Sprite), true);
                        GUILayout.Label("总引用次数:" + e.Current.Value.Count);
                        GUILayout.Label("使用此图集图片的界面");
                        checkSpriteWinDataTime.Clear();
                        for (int i = 0; i < e.Current.Value.Count; i++)
                        {
                            if (!checkSpriteWinDataTime.ContainsKey(e.Current.Value[i]))
                            {
                                checkSpriteWinDataTime.Add(e.Current.Value[i], 1);
                            }
                            else
                            {
                                checkSpriteWinDataTime[e.Current.Value[i]]++;
                            }
                        }
                        int index = 0;
                        var f = checkSpriteWinDataTime.GetEnumerator();
                        while (f.MoveNext())
                        {
                            if (index % 5 == 0)
                            {
                                GUILayout.BeginHorizontal();
                            }
                            EditorGUILayout.ObjectField(f.Current.Key, typeof(GameObject), true);
                            GUILayout.Label("次数:" + f.Current.Value);
                            if (index == checkSpriteWinDataTime.Count - 1 || index % 5 == 4)
                            {
                                GUILayout.EndHorizontal();
                            }
                            index++;
                        }
                        f.Dispose();
                    }
                }
                e.Dispose();
            }
            GUILayout.EndScrollView();
        }
    }
}

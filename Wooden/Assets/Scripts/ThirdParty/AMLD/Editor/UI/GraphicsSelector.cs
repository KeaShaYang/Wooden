using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class GraphicsSelector : EditorWindow
{
    private List<Graphic> showList;
    private Action<Graphic> callback;
    private Vector2 scrollVec;
    
    static readonly GUIStyle m_LODBlackBox = "LODBlackBox";
    
    public static void Open()
    {
        var window = GetWindowWithRect<GraphicsSelector>(new Rect(0, 0, 600, 350), false, "");
        window.minSize = new Vector2(350, 300);
        window.maxSize = new Vector2(1920, 1080);
        window.Show();
    }

    public void Refresh(List<Graphic> list, Action<Graphic> selectCallback)
    {
        showList = list;
        callback = selectCallback;
    }


    void OnGUI()
    {
        if (showList != null)
        {
            scrollVec = GUILayout.BeginScrollView(scrollVec);
            for (int i = 0; i < showList.Count; i++)
            {
                DrawGraphicButton(showList[i], i);
            }
            GUILayout.EndScrollView();
        }
    }

    void DrawGraphicButton(Graphic graphic, int index)
    {
        int col = (int)position.width / 110;
        int x = index % col;
        int y = index / col;
        Rect rect = new Rect(x * 110, y * 110, 100, 100);
        Event current = Event.current;
        switch (current.type)
        {
            case EventType.MouseDown:
                if (rect.Contains(current.mousePosition))
                {
                    if (callback != null) callback(graphic);
                    Close();
                }
                break;
            case EventType.Repaint:
                if (graphic is Text)
                {
                    Text text = graphic as Text;
                    m_LODBlackBox.Draw(rect, new GUIContent(text.text), true, true, true, false);
                }
                else if (graphic is Image)
                {
                    Image img = graphic as Image;
                    m_LODBlackBox.Draw(rect, new GUIContent(AssetPreview.GetAssetPreview(img.sprite), img.sprite.name), true, true, true, false);
                }
                else if (graphic is RawImage)
                {
                    RawImage rawImage = graphic as RawImage;
                    m_LODBlackBox.Draw(rect, new GUIContent(AssetPreview.GetAssetPreview(rawImage.mainTexture), rawImage.mainTexture.name), true, true, true, false);
                }
                break;
        }
    }
}
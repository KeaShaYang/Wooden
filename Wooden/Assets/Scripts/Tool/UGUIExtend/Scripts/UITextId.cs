using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 给Text加个id用于拓展
/// </summary>
[RequireComponent(typeof(Text))]
public class UITextId : MonoBehaviour
{
    public string V_ID = "";
    private bool m_Started = false;
    private bool m_CanLayout = true;

    /// <summary>
    /// 根据预设获取id
    /// </summary>
    public static string GetVIDByTextInPrefab(Text textInPrefab)
    {
        string assetGuid = "";
        long localId = 0;

#if UNITY_EDITOR
        if (textInPrefab != null)
        {
            AssetDatabase.TryGetGUIDAndLocalFileIdentifier(textInPrefab, out assetGuid, out localId);
        }
#endif

        return string.Format("{0}-->{1}", assetGuid, localId.ToString());
    }

#if AMLD_HW
    private void Awake()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying) return;
#endif
        m_Started = true;
        OnLayout();
    }

    private void OnEnable()
    {
#if UNITY_EDITOR
        if (Application.isPlaying) return;
        if (!m_Started) OnLayout();
#endif
    }

#if AMLD_HW
    public void SetCanLayout(bool b)
    {
        m_CanLayout = b;
    }
#endif

    private void OnLayout()
    {
        if(!m_CanLayout) return;

        UITextId uiTextId = GetComponent<UITextId>();
        if (uiTextId != null)
        {
            List<TextLayoutVo> list = HWTextLayout.GetTextLayoutList(uiTextId.V_ID);
            Text text = GetComponent<Text>();
            string[] arr = null;
            TextGamma textGamma = null;
            if (list != null && text != null)
            {
                for (int i = 0, len = list.Count; i < len; i++)
                {
                    try
                    {
                        var vo = list[i];
                        switch (vo.key)
                        {
                            case "Text":
                                text.text = vo.value;
                                break;

                            case "FontStyle":
                                if (Enum.TryParse(vo.value, out FontStyle fontStyle))
                                {
                                    text.fontStyle = fontStyle;
                                }

                                break;
                            case "FontSize":
                                text.fontSize = int.Parse(vo.value);
                                break;
                            case "LineSpacing":
                                text.lineSpacing = float.Parse(vo.value);
                                break;
                            case "RichText":
                                text.supportRichText = bool.Parse(vo.value);
                                break;
                            case "Alignment":
                                if (Enum.TryParse(vo.value, out TextAnchor alignment))
                                {
                                    text.alignment = alignment;
                                }

                                break;
                            case "AlignByGeometry":
                                text.alignByGeometry = bool.Parse(vo.value);
                                break;
                            case "HorizontalOverflow":
                                if (Enum.TryParse(vo.value, out HorizontalWrapMode horizontalOverflow))
                                {
                                    text.horizontalOverflow = horizontalOverflow;
                                }

                                break;
                            case "VerticalOverflow":
                                if (Enum.TryParse(vo.value, out VerticalWrapMode verticalOverflow))
                                {
                                    text.verticalOverflow = verticalOverflow;
                                }

                                break;
                            case "BestFit":
                                arr = vo.GetValueArr();
                                if (arr != null)
                                {
                                    if (arr.Length == 1)
                                    {
                                        text.resizeTextForBestFit = bool.Parse(arr[0]);
                                    }
                                    else if (arr.Length == 4)
                                    {
                                        text.resizeTextForBestFit = bool.Parse(arr[0]);
                                        text.resizeTextMinSize = int.Parse(arr[1]);
                                        text.resizeTextMaxSize = int.Parse(arr[2]);
                                        textGamma = GetComponent<TextGamma>();
                                        if (textGamma)
                                        {
                                            textGamma.SetUseShrinkBestFit(bool.Parse(arr[3]));
                                        }
                                    }
                                }

                                break;
                            case "IsNeedDealHeadChar":
                                textGamma = GetComponent<TextGamma>();
                                if (textGamma)
                                {
                                    textGamma.SetIsNeedDealHeadChar(bool.Parse(vo.value));
                                }

                                break;

                            case "Position":
                                arr = vo.GetValueArr();
                                if (arr != null && arr.Length == 3)
                                {
                                    text.rectTransform.position = new Vector3(float.Parse(arr[0]), float.Parse(arr[1]), float.Parse(arr[2]));
                                }

                                break;
                            case "Size":
                                arr = vo.GetValueArr();
                                if (arr != null && arr.Length == 2)
                                {
                                    text.rectTransform.sizeDelta = new Vector2(float.Parse(arr[0]), float.Parse(arr[1]));
                                }

                                break;
                            case "AnchoredPosition":
                                arr = vo.GetValueArr();
                                if (arr != null && arr.Length == 2)
                                {
                                    text.rectTransform.anchoredPosition = new Vector2(float.Parse(arr[0]), float.Parse(arr[1]));
                                }

                                break;

                            case "AnchorMin":
                                arr = vo.GetValueArr();
                                if (arr != null && arr.Length == 2)
                                {
                                    text.rectTransform.anchorMin = new Vector2(float.Parse(arr[0]), float.Parse(arr[1]));
                                }

                                break;
                            case "AnchorMax":
                                arr = vo.GetValueArr();
                                if (arr != null && arr.Length == 2)
                                {
                                    text.rectTransform.anchorMax = new Vector2(float.Parse(arr[0]), float.Parse(arr[1]));
                                }

                                break;
                            case "Pivot":
                                arr = vo.GetValueArr();
                                if (arr != null && arr.Length == 2)
                                {
                                    text.rectTransform.pivot = new Vector2(float.Parse(arr[0]), float.Parse(arr[1]));
                                }

                                break;
                            case "Rotation":
                                arr = vo.GetValueArr();
                                if (arr != null && arr.Length == 3)
                                {
                                    text.rectTransform.rotation = Quaternion.Euler(float.Parse(arr[0]), float.Parse(arr[1]), float.Parse(arr[2]));
                                }

                                break;
                            case "Scale":
                                arr = vo.GetValueArr();
                                if (arr != null && arr.Length == 3)
                                {
                                    text.rectTransform.localScale = new Vector3(float.Parse(arr[0]), float.Parse(arr[1]), float.Parse(arr[2]));
                                }

                                break;
                        }
                    }
                    catch (Exception e)
                    {
                        Debuger.Log("TextLayout Error, TextId : " + uiTextId.V_ID + " , error : " + e.Message);
                    }
                }
            }
        }
    }
#endif
}
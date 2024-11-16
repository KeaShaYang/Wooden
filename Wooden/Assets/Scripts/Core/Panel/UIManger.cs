using Assets.Scripts.Define;

using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 存储所有UI信息，并可以创建或者销毁UI
/// </summary>
public class UIManager : Singleton<UIManager>
{
    // Start is called before the first frame update
    /// <summary>
    /// 存储所有UI信息的字典，每一个UI信息都会对应一个GameObject
    /// </summary>
    private Dictionary<EM_WinType, BaseWindow> dicUI;
    private GameObject m_UIRoot;
    public Dictionary<EM_WinType, string> m_pathDic;
    public int V_OrderIndex = 40;
    public UIManager()
    {
        dicUI = new Dictionary<EM_WinType, BaseWindow>();
        m_pathDic = new Dictionary<EM_WinType, string>();
        m_pathDic.Add(EM_WinType.WinLoading, "UI/WinLoading");
        m_pathDic.Add(EM_WinType.WinMain, "UI/Level/WinMain");
        m_pathDic.Add(EM_WinType.WinLevel, "UI/Level/WinLevel");
        m_pathDic.Add(EM_WinType.WinLevelResult, "UI/WinLevelResult");
    }
    public void F_AddWin(EM_WinType winType,BaseWindow win)
    {
        if (!dicUI.ContainsKey(winType))
            dicUI.Add(winType, win);
        else
        {
            DestroyUI(winType);
            dicUI.Add(winType, win);
        }
    }
    /// <summary>
    /// 获取一个UI对象
    /// </summary>
    /// <param name="type">UI信息</param>
    /// <returns></returns>
    public BaseWindow GetSingleUI(EM_WinType winType)
    {
        if (null == m_UIRoot)
            m_UIRoot = GameObject.Find("UIRoot");
        if (null == m_UIRoot)
        {
            Debug.LogError("Canvas不存在，请仔细查找有无这个对象");
            return null;
        }
        if (dicUI.ContainsKey(winType))
            return dicUI[winType];
        if (m_pathDic.ContainsKey(winType))
        {
            BaseWindow win = Resources.Load<BaseWindow>(m_pathDic[winType]);
            if (null != win)
            {
                BaseWindow ui = GameObject.Instantiate(win, m_UIRoot.transform);//此处是在Canvas下生成预制体，函数使用方法见上面解析
                dicUI.Add(winType, ui);
                V_OrderIndex++;
                return ui;
            }
            else
                Debug.Log(winType + "加载失败");
        }
        else
            Debug.Log(winType + "没配路径");
       
            
        return null;
    }

    /// <summary>
    /// 销毁一个UI对象
    /// </summary>
    /// <param name="type">UI信息</param>
    public void DestroyUI(EM_WinType winType)
    {
        if (dicUI.ContainsKey(winType))
        {
            GameObject.Destroy(dicUI[winType].gameObject);
            dicUI.Remove(winType);
        }
    }

}

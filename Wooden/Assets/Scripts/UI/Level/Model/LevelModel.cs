using Assets.Scripts.Define;
using System;
using UnityEngine;

public class LevelModel : Model
{
    public LevelModel()
    {

    }

    bool m_canClick = false;
    /// <summary>
    /// 是否可以开始拆
    /// </summary>
    public bool V_CanClick { get { return m_canClick; } }
    private int m_currentLevel = 1;
    public int V_CurrentLevel { get { return m_currentLevel; } }
    public int m_stingCount = 12;

    internal int F_GetColor(int i)
    {
        //todo：记录和生成钉子颜色，保证颜色在规定范围，且能找到对应的钉子
    }

    public int m_removeSting = 0;
    public override void Destroy()
    {
        base.Destroy();
    }
    public void F_InitLevel(int level)
    {
        m_currentLevel = level;
        m_canClick = true;
    }
    public void F_RemoveSting(Sting sting)
    {
        m_removeSting++;
        //通知winlevel，已移除某位置的螺丝
        BaseWindow win = UIManager.GetInstance().GetSingleUI(EM_WinType.WinLevel);
        if (null != win)
        {
            WinLevel winLevel = win as WinLevel;
            winLevel.F_OnStingRemove(sting);
        }
        if (m_stingCount - m_removeSting <= 0)
        {
            //成功界面
            m_canClick = false;
            UIManager.GetInstance().GetSingleUI(Assets.Scripts.Define.EM_WinType.WinLevelResult);
        }
    }
    public Color GetStingColor()
    {
        return Color.white;
    }

    internal q_levelExcelItem F_GetLevelCfg()
    {
        int level = m_currentLevel;
        return   ExcelManager.GetInstance().GetExcelItem<q_level, q_levelExcelItem>(level);
    }
}

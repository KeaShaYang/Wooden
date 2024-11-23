using Assets.Scripts.Battle;
using Assets.Scripts.Define;
using DG.Tweening;
using System;
using System.Collections.Generic;
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
    private int m_currentLevel = 0;
    public int V_CurrentLevel { get { return m_currentLevel; } }

    private int m_stingCount = 12;
    public int V_StingCount { get => m_stingCount; }
    private int m_unlockLockNum = 2;
    public int V_unlockLockNum { get => m_unlockLockNum; }
    private int m_lockNum = 4;
    public int V_lockNum { get => m_lockNum; }

    internal bool F_isLastLevel()
    {
        q_levelExcelItem nextCfg = F_GetLevelCfg(m_currentLevel + 1);
        if (nextCfg == null)
            return true;
        return false;
    }

    /// <summary>
    /// 每种颜色有几个钉子
    /// </summary>
    private Dictionary<int, int> m_ColorDic = new Dictionary<int, int>();
    public Stack<int> m_lockColors = new Stack<int>();
    private LockGo[] m_lockGos;
    private Wooden m_wooden;
    private Slot[] m_slotList;
    /// <summary>
    /// 所有钉子的颜色，到时候有道具会提前移除钉子
    /// </summary>
    private int[] m_stingColors;
    public int m_removeSting = 0;
    /// <summary>
    /// 所有钉子的颜色，到时候有道具会提前移除钉子
    /// </summary>
    private int[] m_removeStingArr;
    public q_levelExcelItem V_LevelCfg = null;
    public int m_unlockSlotNum = 4;

    public override void Destroy()
    {
        base.Destroy();
    }
    #region 关卡数据生成
    /// <summary>
    /// 根据关卡初始化颜色数据
    /// </summary>
    /// <param name="wooden">之后要支持多个木块、分层</param>
    public void F_InitLevel(LockGo[] lockGos, Slot[] slots)
    {
        m_lockGos = lockGos;
        m_slotList = slots;
        //todo:改成读玩家数据
        F_ChangeLevel(1);
    }

    /// <summary>
    /// 切换关卡
    /// </summary>
    /// <param name="level">关卡</param>
    /// <param name="reset">默认非强制重开</param>
    public void F_ChangeLevel(int level, bool reset = false)
    {
        if (level != m_currentLevel)
        {
            m_removeSting = 0;
            m_currentLevel = level;
            m_canClick = true;
            V_LevelCfg = F_GetLevelCfg(m_currentLevel);
            if (null != m_wooden)
                DisplayManager.GetInstance().F_DeleteEitity(m_wooden);
            if (null != V_LevelCfg)
            {
                m_stingCount = V_LevelCfg.stingNum;
                if (V_LevelCfg.stingNum % V_LevelCfg.colorNum > 0)
                    Debugger.LogError("level表配置错误，钉子总数要为颜色的倍数");
                else
                {
                    GenerateColorTypes(V_LevelCfg.stingNum, V_LevelCfg.colorNum);
                    for (int i = 0; i < m_lockGos.Length; i++)
                    {
                        int lockColorType = i < V_unlockLockNum ? F_GetLockColor() : 0;
                        m_lockGos[i].F_Init(lockColorType, i < V_unlockLockNum);
                    }
                }
                for (int i = 0; i < m_slotList.Length; i++)
                {
                    m_slotList[i].F_Init(0, null, i < m_unlockSlotNum);
                }
            }
            WoodenData data = new WoodenData();
            data.F_InitData(V_LevelCfg.id);
            m_wooden = DisplayManager.GetInstance().F_AddEitity<Wooden>(data);
            m_wooden.transform.position = Vector3.zero;
            //通知winlevel，已移除某位置的螺丝
            RasiseEvent("LevelChange");
        }
    }

    /// <summary>
    /// 生成所有钉子颜色列表
    /// </summary>
    /// <param name="n">钉子数量</param>
    /// <param name="colorNum">颜色数量</param>
    /// <returns></returns>
    private void GenerateColorTypes(int n, int colorNum)
    {
        m_stingColors = new int[n];
        //缓存当前钉子不同颜色个数，计算锁颜色顺序
        for (int i = 1; i <= colorNum; i++)
        {
            m_ColorDic[i] = 0;
        }
        int totalColorCount = 0;
        m_lockColors.Clear();
        while (totalColorCount < n)
        {
            List<int> availableColors = new List<int>();
            foreach (var kvp in m_ColorDic)
            {
                if (kvp.Value < 3)
                {
                    availableColors.Add(kvp.Key);
                }
            }

            if (availableColors.Count == 0)
            {
                availableColors = new List<int>(m_ColorDic.Keys);
            }

            int randomColor = availableColors[new System.Random().Next(availableColors.Count)];

            m_stingColors[totalColorCount++] = randomColor;
            m_ColorDic[randomColor]++;
            if (randomColor > 0 && m_ColorDic[randomColor] % 3 == 0)
            {
                m_lockColors.Push(randomColor);
            }
        }
        m_removeStingArr = new int[m_stingColors.Length];
    }
    /// <summary>
    /// 计算锁的颜色，还要排除已有的颜色
    /// </summary>
    /// <param name="lockCount">需要生成颜色的锁的总数</param>
    /// <returns>颜色类型列表</returns>
    private int F_GetLockColor()
    {
        if (m_lockColors.Count > 0)
        {
            return m_lockColors.Pop(); ;
        }
        return 0;
    }
    public int F_GetColor(int stingIndex)
    {
        return m_stingColors[stingIndex];
    }
    #endregion

    #region 移除相关逻辑
    /// <summary>
    /// 移除锁
    /// </summary>
    /// <param name="lockGo">移除</param>
    public void F_RemoveLock(LockGo lockGo)
    {
        if (null != lockGo && lockGo.V_UnLock && lockGo.V_Full)
        {
            //播放上移动画加alpha 1-> 0，移动完再让点
            m_canClick = false;
            Vector3 pos = lockGo.transform.localPosition;
            lockGo.transform.DOLocalMoveY(pos.y + 300f, 0.2f)  // 2 秒内向上移动 100 单位
             .SetEase(Ease.Linear).OnComplete(() =>
             {
                 lockGo.F_ClearStings();
                 // 动画完成后的处理逻辑
                 int colorType = F_GetLockColor();
                 Debugger.LogError("colorType" + colorType);
                 if (colorType > 0)
                 {
                     lockGo.transform.localPosition = pos - new Vector3(300, 0, 0);
                     // 2 秒内向右移动 100 单位
                     lockGo.transform.DOLocalMoveX(pos.x, 0.2f).SetEase(Ease.Linear).OnComplete(() =>
                     {
                         lockGo.F_Init(colorType, lockGo.V_UnLock);
                         m_canClick = true;
                         if (lockGo.V_UnLock)
                         {
                             for (int i = 0; i < m_slotList.Length; i++)
                             {
                                 //检查是否有同色的孔位里的钉子，可以移动到新的锁里
                                 if (!lockGo.V_Full && m_slotList[i].V_IsFull && m_slotList[i].V_ColorType == colorType)
                                 {
                                     F_RemoveSting(m_slotList[i].V_Sting, true);
                                 }
                             }
                         }
                     });
                 }
                 else
                 {
                     m_canClick = true;
                     lockGo.F_Init(colorType, lockGo.V_UnLock);
                 }
             });
        }
    }
    /// <summary>
    /// 判断钉子是否可以移动
    /// </summary>
    /// <param name="sting">钉子对象</param>
    /// <param name="targetPosition">移动的目标位置</param>
    /// <returns>是否可以移动</returns>
    public bool F_CanMoveSting(Sting sting, out Vector3 targetPosition)
    {
        targetPosition = sting.transform.position;
        RaycastHit hit;
        if (Physics.Raycast(sting.transform.position, sting.transform.up, out hit))
        {
            if (hit.distance > 0.3f)
            {
                targetPosition = sting.transform.position + new Vector3(0, 1, 0);
                return true;
            }
            else
            {
                float time = hit.distance / 2;
                targetPosition = hit.point;
                return false;
            }
        }
        else
        {
            targetPosition = sting.transform.position + new Vector3(0, 1, 0);
            return true;
        }
    }
    public void F_RemoveSting(Sting sting, bool fromSlot = false)
    {
        if (!fromSlot)
        {
            m_removeSting++;
            m_removeStingArr[sting.V_Index] = 1;
        }
        //通知winlevel，已移除某位置的螺丝
        WinLevel win = UIManager.GetInstance().GetSingleUI(EM_WinType.WinLevel) as WinLevel;
        if (null != win)
        {
            //找到该颜色的螺丝可以移动到的孔位，并移动、旋转到目标位置
            //先检查当前锁的颜色是否一致，然后找到还没active的锁的孔位
            Slot slot = GetEmptySlot(sting.V_ColorType);
            if (null != slot && null != m_wooden)
            {
                slot.F_InjectSting(sting);
                sting.transform.SetParent(DisplayManager.GetInstance().V_EntityRoot.transform);
                RectTransform rect = slot.GetComponent<RectTransform>();

                // 获取目标UI位置的世界坐标
                Vector3 targetPosition = rect.position;
                targetPosition.z = 94; // 保持与钉子当前的深度一致

                // 获取屏幕方向（相机的 forward 方向）
                Vector3 screenDirection = Camera.main.transform.forward;

                // 调用Sting类的方法进行旋转和移动
                sting.F_RotateToScreen(screenDirection);
                sting.F_MoveToSlot(targetPosition, rect, () =>
                {
                    slot.F_CheckLock();
                    RasiseEvent(EM_LevelEvent.LockChange.ToString());
                });
            }
            else
            {
                WinLevelResult winres = UIManager.GetInstance().GetSingleUI(EM_WinType.WinLevelResult) as WinLevelResult;
                winres.F_Init(false, F_Reset);
            }
        }
        if (V_StingCount - m_removeSting <= 0)
        {
            //成功界面
            m_canClick = false;
            TimerMgr.GetInstance().Schedule(() =>
            {
                WinLevelResult winRes = UIManager.GetInstance().GetSingleUI(EM_WinType.WinLevelResult) as WinLevelResult;
                if (null != winRes)
                    winRes.F_Init(true, RefreshWinLevel);
            }, 1, 1, 1);
        }
    }
    /// <summary>
    /// 重启当前关卡。
    /// </summary>
    /// <remarks>
    /// 此方法将当前关卡重置为初始状态，并调用F_ChangeLevel方法切换到第一个关卡。
    /// </remarks>
    public void F_Reset()
    {
        F_ChangeLevel(m_currentLevel, true);
    }

    private void RefreshWinLevel()
    {
        RasiseEvent(EM_LevelEvent.LevelChange.ToString());
    }
    private Slot GetEmptySlot(int colorType)
    {
        Slot emptySlot = null;
        for (int i = 0; i < m_lockGos.Length; i++)
        {
            LockGo lockItem = m_lockGos[i];
            if (colorType == lockItem.V_ColorType && lockItem.V_UnLock)
            {
                //找到空的孔位
                for (int j = 0; j < lockItem.V_SlotArr.Length; j++)
                {
                    Slot slot = lockItem.V_SlotArr[j];
                    if (slot && !slot.V_IsFull && slot.V_isUnlock)
                    {
                        emptySlot = slot;
                        break;
                    }
                }
                if (null != emptySlot)
                    break;
            }
        }
        if (null == emptySlot)
        {
            //找到空的孔位
            for (int j = 0; j < m_slotList.Length; j++)
            {
                Slot slot = m_slotList[j];
                if (slot && !slot.V_IsFull && slot.V_isUnlock)
                {
                    emptySlot = slot;
                    break;
                }
            }
        }
        return emptySlot;
    }
    #endregion
    public q_levelExcelItem F_GetLevelCfg(int level)
    {
        return ExcelManager.GetInstance().GetExcelItem<q_level, q_levelExcelItem>(level);
    }
    public Wooden F_GetCurrentWooden()
    {
        return m_wooden;
    }

    internal void F_Exit()
    {
        UIManager.GetInstance().GetSingleUI(EM_WinType.WinMain);
        UIManager.GetInstance().DestroyUI(EM_WinType.WinLevel);
        UIManager.GetInstance().DestroyUI(EM_WinType.WinLevelResult);
    }
}

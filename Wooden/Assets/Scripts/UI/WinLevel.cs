using Assets.Scripts.Define;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class WinLevel : BaseWindow
{
    public RawImage img;
    public int unlockLockNum = 2;
    public int maxColor = 3;
    public int m_StingNum = 12;
    public Transform LockListGo;
    public Transform SlotListGo;
    public Text m_text_lastStingNum;
    public Text m_text_Progress;
    [SerializeField]
    private LockGo[] m_LockList;
    [SerializeField]
    private Slot[] m_slotList;
    Wooden m_wooden;
    public override EM_WinType F_GetWinType()
    {
        return EM_WinType.WinLevel;
    }

    protected override void Awake()
    {
        m_LockList = LockListGo.GetComponentsInChildren<LockGo>();
        m_slotList = SlotListGo.GetComponentsInChildren<Slot>();
    }

    internal void F_Init(RenderTexture render, Wooden wooden)
    {
        img.texture = render;
        m_wooden = wooden;
        //for (int i = 0; i < m_Stings.Length; i++)
        //{
        //    m_Stings[i].F_Init(i, LevelMgr.GetInstance().V_Model.F_GetColor(i));
        //}
        LevelMgr.GetInstance().V_Model.F_InitLevel(m_wooden,m_LockList,m_slotList);
        m_StingNum = LevelMgr.GetInstance().V_Model.V_StingCount;
       
    }
    public void F_Refresh()
    {
        q_levelExcelItem cfg = LevelMgr.GetInstance().V_Model.F_GetLevelCfg(LevelMgr.GetInstance().V_Model.V_CurrentLevel);
        m_text_Progress.text = "当前关卡" + LevelMgr.GetInstance().V_Model.V_CurrentLevel;
        m_text_lastStingNum.text = "剩余：" + (cfg.stingNum - LevelMgr.GetInstance().V_Model.V_StingCount);

    }

}

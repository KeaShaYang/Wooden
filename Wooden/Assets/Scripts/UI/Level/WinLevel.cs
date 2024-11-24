using Assets.Scripts.Define;
using UnityEngine;
using UnityEngine.UI;

public class WinLevel : BaseWindow
{
    public RawImage img;
    public Transform LockListGo;
    public Transform SlotListGo;
    public Text m_text_lastStingNum;
    public Text m_text_Progress;
    [SerializeField]
    private LockGo[] m_LockList;
    [SerializeField]
    private Slot[] m_slotList;
    public HorizontalLayoutGroup m_LayoutGroup;
    public override EM_WinType F_GetWinType()
    {
        return EM_WinType.WinLevel;
    }

    protected override void Awake()
    {
        m_LockList = LockListGo.GetComponentsInChildren<LockGo>();
        m_slotList = SlotListGo.GetComponentsInChildren<Slot>();
        Init(LevelMgr.GetInstance().V_Model);
        BindModel(EM_LevelEvent.LevelChange, F_Refresh);
        BindModel(EM_LevelEvent.LockChange, F_Refresh);
    }
    void Start()
    {
        LevelMgr.GetInstance().V_Model.F_InitLevel(m_LockList, m_slotList);
        F_Refresh(null);
    }
    void F_Refresh(object[] arg1)
    {
        q_levelExcelItem cfg = LevelMgr.GetInstance().V_Model.F_GetLevelCfg(LevelMgr.GetInstance().V_Model.V_CurrentLevel);
        m_text_Progress.text = "当前关卡" + LevelMgr.GetInstance().V_Model.V_CurrentLevel;
        m_text_lastStingNum.text = "剩余：" + (LevelMgr.GetInstance().V_Model.V_StingCount - LevelMgr.GetInstance().V_Model.m_removeSting);
        RefreshLock(arg1);
    }
    void RefreshLock(object[] arg1)
    {
        if ((arg1!=null && arg1.Length > 0 ) || LevelMgr.GetInstance().V_Model.V_unlockLockNum > LevelMgr.GetInstance().V_Model.m_lockColors.Count)
        {
            m_LayoutGroup.enabled = true;
            m_LayoutGroup.SetLayoutHorizontal();

            TimerMgr.GetInstance().Schedule(() =>
            {
                if (m_LayoutGroup)
                    m_LayoutGroup.enabled = false;
            }, 1, 1, 1);
        }
    }
}

using Assets.Scripts.Define;
using DG.Tweening;
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
    }

    internal void F_Init(RenderTexture render)
    {
        img.texture = render;
        LevelMgr.GetInstance().V_Model.F_InitLevel(m_LockList,m_slotList);
        F_Refresh();
    }
    public void F_Refresh(bool needReposition = false)
    {
        q_levelExcelItem cfg = LevelMgr.GetInstance().V_Model.F_GetLevelCfg(LevelMgr.GetInstance().V_Model.V_CurrentLevel);
        m_text_Progress.text = "当前关卡" + LevelMgr.GetInstance().V_Model.V_CurrentLevel;
        m_text_lastStingNum.text = "剩余：" + (LevelMgr.GetInstance().V_Model.V_StingCount - LevelMgr.GetInstance().V_Model.m_removeSting);
        if (needReposition || LevelMgr.GetInstance().V_Model.V_unlockLockNum > LevelMgr.GetInstance().V_Model.m_lockColors.Count)
        { 
            m_LayoutGroup.enabled = true;
            m_LayoutGroup.SetLayoutHorizontal();
           
            TimerMgr.GetInstance().Schedule(() => {
                if (m_LayoutGroup)
                    m_LayoutGroup.enabled = false;
            }, 1, 1, 1);
        }
    }

}

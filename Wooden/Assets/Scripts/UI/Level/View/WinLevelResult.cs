using Assets.Scripts.Define;
using Assets.Scripts.UI;
using UnityEngine.UI;

public class WinLevelResult : BaseWindow
{
    public override EM_WinType F_GetWinType()
    {
        return EM_WinType.WinLevelResult;
    }
    public Text m_textDes;
    public CommonButton m_button;
    private bool m_isSuccess = false;
    public CommonButton m_button2;
    public override void ClickClose(ClickUIObj obj)
    {
        if (m_isSuccess)
            LevelMgr.GetInstance().V_Model.F_ChangeLevel(LevelMgr.GetInstance().V_Model.V_CurrentLevel + 1);
        else
            UIManager.GetInstance().GetSingleUI(EM_WinType.WinMain);
        UIManager.GetInstance().DestroyUI(F_GetWinType());
    }
    protected override void Awake()
    {
        m_button2.gameObject.SetActive(false);
    }
   
    public void F_Init(bool isSuccess, System.Action refreshWinLevel)
    {
        m_isSuccess = isSuccess;
        m_button.gameObject.SetActive(true);
        if (isSuccess)
        {
            if (LevelMgr.GetInstance().V_Model.F_isLastLevel())
            {
                m_textDes.text = "真厉害，已经全部通关啦！";
                m_button.F_Refresh("返回主界面");
                m_button.F_SetClickCall((button)=>{
                    ClickClose(m_CloseBtn);
                });
            }
            else
            {
                m_textDes.text = "恭喜获得新道具！";
                m_button.F_Refresh("下一关");
                m_button.F_SetClickCall((button) => {
                    ClickClose(m_CloseBtn);
                });
            }
           
        }
        else
        {
            m_textDes.text = "孔位满了，是否重新开始？";
            m_button.F_Refresh("确定");
            m_button.F_SetClickCall((button) => {
                LevelMgr.GetInstance().V_Model.F_Reset();
            });
            m_button2.gameObject.SetActive(true);
            m_button2.F_Refresh("返回主界面");
            m_button2.F_SetClickCall((button) => {
                LevelMgr.GetInstance().V_Model.F_Exit();
                UIManager.GetInstance().DestroyUI(F_GetWinType());
            });
        }
    }
}

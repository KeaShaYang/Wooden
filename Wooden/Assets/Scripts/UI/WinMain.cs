using Assets.Scripts.Define;
using Assets.Scripts.UI;

public class WinMain : BaseWindow
{
    public ClickUIObj m_BtnSetting;
    public ClickUIObj m_BtnMore;
    public ClickUIObj m_BtnTask;
    public ClickUIObj m_BtnSign;
    public ClickUIObj m_BtnRank;
    public ClickUIObj m_BtnShare;
    public ClickUIObj m_BtnSkin;
    public ClickUIObj m_BtnStart;

    public override EM_WinType F_GetWinType()
    {
        return EM_WinType.WinMain;
    }

    // Start is called before the first frame update
    void Start()
    {
        m_BtnStart.F_SetClickCall((arg)=> { 
            UIManager.GetInstance().GetSingleUI(EM_WinType.WinLevel);
            ClickClose(m_CloseBtn);
        });
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
    }
}

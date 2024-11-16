using Assets.Scripts.Define;
using Assets.Scripts.UI;

using UnityEngine;

[RequireComponent(typeof(Canvas))]
public abstract class BaseWindow : View
{
    public ClickUIObj m_CloseBtn;

    public abstract EM_WinType F_GetWinType();

    private void Start()
    {
        Canvas can = GetComponent<Canvas>();
        if (F_GetWinType() != EM_WinType.WinMain)
            can.sortingOrder = 40;
        else
            can.sortingOrder = UIManager.GetInstance().V_OrderIndex;
        if (null != m_CloseBtn)
        {
            m_CloseBtn.F_SetClickCall(ClickClose);
        }
    }
    
    public virtual void ClickClose(ClickUIObj obj)
    {
        UIManager.GetInstance().DestroyUI(F_GetWinType());
    }

    public virtual void OnEnter() { }

    public virtual void OnPause() { }
    /// <summary>
    /// UI继续时的操作
    /// </summary>
    public virtual void OnResume() { }

    public virtual void OnExit() { }

}

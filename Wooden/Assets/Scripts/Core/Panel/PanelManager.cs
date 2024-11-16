using Assets.Scripts.Define;

using System.Collections.Generic;

/// <summary>
/// 面板管理器 用栈来存储UI
/// </summary>
public class PanelManager : Singleton<PanelManager>
{
    /// <summary>
    /// 存储UI面板的栈
    /// </summary>
    private Stack<BaseWindow> stackPanel;
    private BaseWindow panel;

    public void F_Init()
    {
        stackPanel = new Stack<BaseWindow>();
    }
    /// <summary>
    /// UI的入栈操作，此操作会显示一个面板
    /// </summary>
    /// <param name="nextPanel">要显示的面板</param>
    public T Push<T>(EM_WinType winType) where T : BaseWindow
    {
        if (stackPanel.Count > 0)//如果当前没有面板，则不需要执行暂停面板的操作
        {
            panel = stackPanel.Peek();//获取栈顶
            panel.OnPause();
        }
        BaseWindow panelGo = UIManager.GetInstance().GetSingleUI(winType);
        stackPanel.Push(panelGo);
        return panelGo as T;
    }

    /// <summary>
    /// 执行面板的出栈操作，此操作会执行面版的OnExit方法
    /// </summary>
    public void Pop()
    {
        if (stackPanel.Count > 0)
        {
            BaseWindow win = stackPanel.Peek();
            win.OnExit();
            UIManager.GetInstance().DestroyUI(win.F_GetWinType());
            stackPanel.Pop();
        }
        if (stackPanel.Count > 0)
            stackPanel.Peek().OnResume();

    }
}


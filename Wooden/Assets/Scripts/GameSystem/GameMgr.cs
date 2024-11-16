using UnityEngine;
/// <summary>
/// 游戏总管理器
/// </summary>
class GameMgr : Singleton<GameMgr>
{
    public GameModel V_Model;
    public GameMgr()
    {
        V_Model = new GameModel();
    }
    public void F_InitManager()
    {
        TimerMgr.GetInstance().Init();
        DisplayManager.GetInstance().F_Init();
        PanelManager.GetInstance().F_Init();
    }

    ///gamemgr管理游戏进度，如游戏暂停，引导系统，战斗系统，UI系统等。其他系统不得互相调用
    ///监听
    public void F_GameStart()
    {
        V_Model.F_Init();
        GameObjectPool.GetInstance().F_Init();
    }
    public void F_BattleChange(bool result)
    {
        //暂停游戏
        //检查是否要显示引导
        //显示结算界面弹窗
    }
    public void OnDestroy()
    {
        TimerMgr.GetInstance().F_Destry();
    }
}


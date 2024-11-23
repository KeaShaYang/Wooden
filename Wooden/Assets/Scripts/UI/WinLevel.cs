using System;
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
        Init(LevelMgr.GetInstance().V_Model);
        BindModel(EM_LevelEvent.LevelChange, F_Refresh);
        BindModel(EM_LevelEvent.LockChange, F_Refresh);
    }
    void Destroy()
    {
        UnBindModel(EM_LevelEvent.LevelChange, F_Refresh);
        UnBindModel(EM_LevelEvent.LockChange, F_Refresh);
    }
    internal void F_Init(Camera photoCamera)
    {
        //RenderTexture renderTexture = new RenderTexture(512, 512, 24);
        //photoCamera.targetTexture = renderTexture;
        //// 让相机渲染一帧以捕获图像
        //photoCamera.Render();
        //// 从RenderTexture获取纹理
        //Texture2D texture = new Texture2D(512, 512, TextureFormat.ARGB32, false);
        //img.texture= texture;
        //RenderTexture.active = renderTexture;
        //texture.ReadPixels(new Rect(0, 0, 512, 512), 0, 0);
        //texture.Apply();
        //// 清理资源，将相机的目标纹理设置为null
        //photoCamera.targetTexture = null;
        //// 可以在这里将纹理保存为文件或者用于其他用途，例如显示在UI上
        //// 释放RenderTexture资源
        //RenderTexture.ReleaseTemporary(renderTexture);
        LevelMgr.GetInstance().V_Model.F_InitLevel(m_LockList, m_slotList);
        F_Refresh(null);
    }
    void F_Refresh(object[] arg1)
    {
        q_levelExcelItem cfg = LevelMgr.GetInstance().V_Model.F_GetLevelCfg(LevelMgr.GetInstance().V_Model.V_CurrentLevel);
        m_text_Progress.text = "当前关卡" + LevelMgr.GetInstance().V_Model.V_CurrentLevel;
        m_text_lastStingNum.text = "剩余：" + (LevelMgr.GetInstance().V_Model.V_StingCount - LevelMgr.GetInstance().V_Model.m_removeSting);
        RefreshLock(null);
    }
    void RefreshLock(object[] arg1)
    {
        if (LevelMgr.GetInstance().V_Model.V_unlockLockNum > LevelMgr.GetInstance().V_Model.m_lockColors.Count)
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

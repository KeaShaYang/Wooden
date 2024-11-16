using Assets.Scripts.Define;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class WinLevel : BaseWindow
{
    public RawImage img;
    public int unlockLockNum = 2;
    public int unlockSlotNum = 4;
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

    private void Start()
    {
        m_LockList = LockListGo.GetComponentsInChildren<LockGo>();
        m_slotList = SlotListGo.GetComponentsInChildren<Slot>();
        for (int i = 0; i < m_LockList.Length; i++)
        {
            int color = Random.Range(1, 4);
            m_LockList[i].F_Init(color, i < unlockLockNum);
        }
        for (int i = 0; i < m_slotList.Length; i++)
        {
            m_slotList[i].F_Init(0,null, i < unlockSlotNum);
        }
    }

    internal void F_Init(RenderTexture render, Wooden wooden)
    {
        img.texture = render;
        m_wooden = wooden;
        q_levelExcelItem cfg = LevelMgr.GetInstance().V_Model.F_GetLevelCfg();
        if (null != cfg)
        {
            this.m_StingNum = cfg.stingNum;

            //初始化木块
            m_wooden.F_Init();
        }
    }
    internal void F_CheckLock(Slot slot)
    {
        //
    }
    internal void F_OnStingRemove(Sting sting)
    {
        //找到该颜色的螺丝可以移动到的孔位，并移动、旋转到目标位置
        //先检查当前锁的颜色是否一致，然后找到还没active的锁的孔位
        Slot slot = GetEmptySlot(sting.V_ColorType);
        if (null != slot && null != m_wooden)
        {
            slot.F_InjectSting(sting.V_ColorType);
            sting.transform.SetParent(DisplayManager.GetInstance().V_EntityRoot.transform);
            RectTransform rect = slot.GetComponent<RectTransform>();
            // 获取目标UI位置的世界坐标
            Vector3 targetPosition = rect.position;
            targetPosition.z = 94; // 保持与钉子当前的深度一致

            // 使用DOTween移动钉子到目标位置
            sting.transform.DOMove(targetPosition, 1f);
            // 获取屏幕方向（相机的 forward 方向）
            Vector3 screenDirection = Camera.main.transform.forward;
            // 计算钉子需要旋转的四元数，使up方向朝向屏幕
            Quaternion targetRotation = Quaternion.FromToRotation(sting.transform.up, screenDirection) * transform.rotation;
            // 使用DOTween旋转钉子到目标旋转状态
            sting.transform.DORotateQuaternion(targetRotation, 0.5f);
            sting.transform.DOScale(new Vector3(2, 2, 2), 1);

        }
    }
    private Slot GetEmptySlot(int colorType)
    {
        Slot emptySlot = null;
        for (int i = 0; i < m_LockList.Length; i++)
        {
            var lockItem = m_LockList[i];
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
}

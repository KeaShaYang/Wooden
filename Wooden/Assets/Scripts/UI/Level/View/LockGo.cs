using Assets.Scripts.UI;
using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LockGo : BaseMonoBehaviour
{
    private bool m_isFull = false;
    public bool V_Full { get { return m_isFull; } }
    private bool m_unLock;
    /// <summary>
    /// 是否已解锁
    /// </summary>
    public bool V_UnLock { get { return m_unLock; } }
    private int m_colorType;
    public int V_ColorType{ get { return m_colorType; } }
    private Slot[] m_SlotArr;
    public Slot[] V_SlotArr { get { return m_SlotArr; } }
    private Image m_Image;
    private int m_slotLength = 0;
    public ClickUIObj m_LockBtn;
    public List<Sting> m_haveStings = new List<Sting>();
    protected override void Awake()
    {
        m_Image = GetComponent<Image>();
        m_SlotArr = GetComponentsInChildren<Slot>();
    }



    // Start is called before the first frame update
    void Start()
    {
       
    }
    public void F_Init(int color,bool isUnlock)
    {
        m_isFull = false;
        F_SetColor(color);
        m_unLock = isUnlock;
        m_slotLength = m_SlotArr.Length;
        m_LockBtn.gameObject.SetActive(!isUnlock);
    }
    private void F_SetColor(int colorType)
    {
        m_colorType = colorType;
        m_isFull = false;
        q_colorExcelItem cfg = ExcelManager.GetInstance().GetExcelItem<q_color, q_colorExcelItem>(colorType);
        if (null != cfg)
        // 修改颜色为红色
        {
            F_SetActive(true);
            Color color = new Color();
            if (ColorUtility.TryParseHtmlString(cfg.valueType, out color))
            {
                m_Image.color = color;
            }
            for (int i = 0; i < m_SlotArr.Length; i++)
            {
                m_SlotArr[i].F_Init(colorType, this);
            }
        }
        else
            F_SetActive(!V_UnLock);
    }
    /// <summary>
    /// 孔位装入钉子时回调
    /// </summary>
    public void F_CheckLock()
    {
        m_slotLength--;
        if (m_slotLength <= 0)
        {
            //说明孔位已满，通知winlevel生成一个新的锁并播放向左的动画，然后重新生成颜色
            if (V_SlotArr[V_SlotArr.Length - 1].V_IsFull)
            {
                m_isFull = true;
                LevelMgr.GetInstance().V_Model.F_RemoveLock(this);
            }
        }
    }
    public void F_AddSting(Sting sting)
    {
        //记录这个锁持有的螺丝
        //方便重置锁的时候把旧的螺丝销毁
        m_haveStings.Add(sting);
    }
    public void F_ClearStings()
    {
        for (int i = 0; i < m_haveStings.Count; i++)
        {
            if (null != m_haveStings)
            {
                Destroy(m_haveStings[i].gameObject);
                m_haveStings[i] = null;
            }
        }
        //记录这个锁持有的螺丝
        //方便重置锁的时候把旧的螺丝销毁
        m_haveStings.Clear();
    }
    public void F_SetActive(bool active)
    {
        gameObject.SetActive(active); 
    }
    public void F_RemoveAndReinit(Action callBack)
    {
        if (V_UnLock && V_Full)
        {
            // 播放上移动画加alpha 1-> 0，移动完再让点
            Vector3 pos = transform.localPosition;
            transform.DOLocalMoveY(pos.y + 300f, 0.2f)  // 2 秒内向上移动 100 单位
             .SetEase(Ease.Linear).OnComplete(() =>
             {
                 F_ClearStings();
                 // 动画完成后的处理逻辑
                 int colorType = LevelMgr.GetInstance().V_Model.F_GetLockColor();
                 if (colorType > 0)
                 {
                     transform.localPosition = pos - new Vector3(300, 0, 0);
                     F_Init(colorType, V_UnLock);
                     // 2 秒内向右移动 100 单位
                     transform.DOLocalMoveX(pos.x, 0.2f).SetEase(Ease.Linear).OnComplete(() =>
                     {
                         if (V_UnLock)
                         {
                             callBack();
                         }
                     });
                 }
                 else
                 {
                     F_Init(colorType, V_UnLock);
                 }
             });
        }
    }
}

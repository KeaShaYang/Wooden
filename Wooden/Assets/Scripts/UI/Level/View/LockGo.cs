using Assets.Scripts.UI;
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
    protected override void Awake()
    {
        m_Image = GetComponent<Image>();
        m_SlotArr = GetComponentsInChildren<Slot>();
        m_slotLength = m_SlotArr.Length;
    }

    // Start is called before the first frame update
    void Start()
    {
       
    }
    public void F_Init(int color,bool isUnlock)
    {
        F_SetColor(color);
        m_unLock = isUnlock;
        m_LockBtn.gameObject.SetActive(!isUnlock);
    }
    private void F_SetColor(int colorType)
    {
        m_colorType = colorType;
        if (colorType == 0)
            m_isFull = false;
        else
            m_isFull = true;
        q_colorExcelItem cfg = ExcelManager.GetInstance().GetExcelItem<q_color, q_colorExcelItem>(colorType);
        // 修改颜色为红色
        Color color = new Color();
        if (ColorUtility.TryParseHtmlString(cfg.valueType,out color))
        {
            m_Image.color = color;
        }
        for (int i = 0; i < m_SlotArr.Length; i++)
        {
            m_SlotArr[i].F_Init(colorType,this);
        }
       
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
        }
    }
}

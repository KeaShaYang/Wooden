using UnityEngine;

public class Slot : BaseMonoBehaviour
{

    private bool m_isFull = false;
    private int m_colorType;
    public int V_ColorType{ get { return m_colorType; } }

    private bool m_isUnlock = false;
    public bool V_isUnlock { get { return m_isUnlock; } }
    private LockGo m_parentLock;
    private Sting m_sting;
    public Sting V_Sting { get { return m_sting; } }
    public bool V_IsFull
    {
        get
        {
            return m_isFull;
        }
        set
        {
            m_isFull = value;

        }
    }
    /// <summary>
    /// 钉子移动到该孔位
    /// </summary>
    /// <param name="sting">钉子</param>
    public void F_InjectSting(Sting sting)
    {
        m_isFull = true;
        m_colorType = sting.V_ColorType;
        if (null != m_parentLock)
        {
            m_parentLock.F_AddSting(sting);
        }
        else
        {
            m_sting = sting;
        }
    }
    public void F_CheckLock()
    {
        if (null != m_parentLock)
        {
            //检查锁是否孔位全满
            m_parentLock.F_CheckLock();
        }
    }
    public void F_Init(int colorType,LockGo parentLock = null,bool isUnlock = true)
    {
        m_isFull = false;
        m_colorType = colorType;
        m_isUnlock = isUnlock;
        m_parentLock = parentLock;
        if (!isUnlock)
            gameObject.SetActive(false);
    }
  
}

using UnityEngine;

public class Slot : BaseMonoBehaviour
{

    private bool m_isFull = false;
    private int m_colorType;
    public int V_ColorType{ get { return m_colorType; } }

    private bool m_isUnlock = false;
    public bool V_isUnlock { get { return m_isUnlock; } }
    private LockGo m_parentLock;

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
    public void F_InjectSting(int colorType)
    {
        m_isFull = true;
        m_colorType = colorType;
        if (null != m_parentLock)
        {
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

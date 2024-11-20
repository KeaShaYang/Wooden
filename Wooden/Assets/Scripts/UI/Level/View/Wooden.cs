
public class Wooden : EntityView
{
    public Sting[] m_Stings;
    public override void F_AfterAddRes()
    {
        if (null != m_ResGo)
        {
            m_Stings = m_ResGo.GetComponentsInChildren<Sting>();
            for (int i = 0; i < m_Stings.Length; i++)
            {
                m_Stings[i].F_Init(i, LevelMgr.GetInstance().V_Model.F_GetColor(i));
            }
        }
    }   
}

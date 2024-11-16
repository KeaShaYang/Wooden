using Assets.Scripts.Define;
using UnityEngine;
public class Entity : Model
{
    internal int m_displayId;
    public int V_DisplayId { get { return m_displayId; } }
    private long m_id;
    public string V_Name = string.Empty;
    internal EM_EntityType m_enityType = EM_EntityType.None;
    public EM_EntityType V_EnityType { get { return m_enityType; } }
    public long V_id { get { return m_id; } set { m_id = value; } }
    private Vector3 m_GridPos = new Vector3(9999, 9999, 9999);
    public Vector3 GridPos { get => m_GridPos; set => m_GridPos = value; }
    private Vector3 m_Scale = Vector3.one;
    public Vector3 V_Scale { get => m_Scale; set => m_Scale = value; }

    public virtual void F_InitData(int cfgId)
    {
    }

}


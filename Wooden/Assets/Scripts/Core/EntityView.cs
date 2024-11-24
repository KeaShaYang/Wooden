using Assets.Scripts.Define;
using System.Collections.Generic;
using UnityEngine;

public class EntityView : View
{
    private int m_displayId;
    private q_displayExcelItem m_cfg;
    public q_displayExcelItem V_Cfg
    {
        get { return m_cfg; }
    }
    Entity m_Entity;
    public Entity V_Enity { get { return m_Entity; } set { m_Entity = value; } }
    private List<long> m_buffList = new List<long>();

    protected GameObject m_ResGo = null;
    public void F_Init(Entity entity)
    {
        //生成资源对象
        //拿到动画组件
        if (null != entity)
        {
            m_Entity = entity;
            m_displayId = entity.V_DisplayId;
            m_cfg = ExcelManager.GetInstance().GetExcelItem<q_display, q_displayExcelItem>(m_displayId);
            if (null != m_cfg)
            {
                entity.V_Scale = new Vector3(m_cfg.scale / 10000f, m_cfg.scaleY / 10000f, 1);
                transform.localPosition = entity.GridPos;
                transform.localScale = entity.V_Scale;
                V_PrefabPath = m_cfg.resPath;
                GameObject obj = Resources.Load<GameObject>("Prefabs/" + V_PrefabPath);
                if (null != obj)
                {
                    GameObject go = GameObjectPool.GetInstance().CreateObject(V_PrefabPath, obj, entity.GridPos, entity.V_Scale);
                    m_ResGo = go;
                    go.transform.SetParent(transform);
                    go.transform.localPosition = Vector3.zero;
                    go.transform.localScale = Vector3.one;
                    F_AfterAddRes();
                }
                else
                    Debug.LogError("资源加载失败请检查路径" + V_PrefabPath);
            }
        }
    }
    public virtual void F_AfterAddRes()
    {

    }
    protected override void Awake()
    {
        base.Awake();
    }
    public void F_RemoveBuff(long id)
    {
        if (m_buffList.Contains(id))
        {
            m_buffList.Remove(id);
        }
    }
    public void F_AddBuffLongId(long id)
    {
        if(!m_buffList.Contains(id))
        m_buffList.Add(id);
    }
    public virtual void F_Delete()
    {
        OnDestroy();
        GameObjectPool.GetInstance().CollectObject(m_ResGo);
        if (null != gameObject)
            Destroy(gameObject);
    }
    protected override void OnDestroy()
    {
        for (int i = 0; i < m_buffList.Count; i++)
        {
            DisplayManager.GetInstance().F_DeleteEntity(EM_EntityType.Buff, m_buffList[i]);
        }
       
    }
}


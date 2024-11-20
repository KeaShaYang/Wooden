using Assets.Scripts.Define;
using System.Collections.Generic;
using UnityEngine;
public class DisplayManager : Singleton<DisplayManager>
{
    GameObject m_EntityRoot;
    public GameObject V_EntityRoot { get { return m_EntityRoot; } }
    long m_playerId = 10000;
    Dictionary<EM_EntityType, Dictionary<long, EntityView>> m_EntityTypeDic = new Dictionary<EM_EntityType, Dictionary<long, EntityView>>();
    public void F_Init()
    {
        //todo:生成一个所有实体的总节点
        m_EntityRoot = GameObject.Find("EntityRoot");
    }
    public void F_DeleteEntityAll(EM_EntityType enType)
    {
        if (m_EntityTypeDic.ContainsKey(enType))
        {
            foreach (var item in m_EntityTypeDic[enType])
            {
                item.Value.F_Delete();
            }
            m_EntityTypeDic[enType].Clear();
            m_EntityTypeDic.Remove(enType);
           
        }
    }
    public void F_DeleteEntity(EM_EntityType enType,long id)
    {
        if (m_EntityTypeDic.ContainsKey(enType) && m_EntityTypeDic[enType].ContainsKey(id))
        {
            EntityView view = m_EntityTypeDic[enType][id];
            m_EntityTypeDic[enType].Remove(id);
            view.F_Delete();
        }
    }
    public void F_DeleteEitity(EntityView view)
    {
        if (null != view && null != view.V_Enity)
        {
            if (m_EntityTypeDic.ContainsKey(view.V_Enity.V_EnityType) && m_EntityTypeDic[view.V_Enity.V_EnityType].ContainsKey(view.V_Enity.V_id))
            {
                m_EntityTypeDic[view.V_Enity.V_EnityType].Remove(view.V_Enity.V_id);
                view.F_Delete();
            }
        }
    }
    public V F_AddEitity<V>(Entity entity) where V : EntityView
    {
        if (null == entity)
            return null;
        int displayId = entity.V_DisplayId;
        V newEntityView = null;
        q_displayExcelItem cfg = ExcelManager.GetInstance().GetExcelItem<q_display, q_displayExcelItem>(displayId);
        if (null != cfg && null != m_EntityRoot)
        {
            m_playerId++;
            entity.V_id = m_playerId;
            GameObject t = new GameObject(entity.V_EnityType.ToString() + m_playerId);
            if (null != t && null!= m_EntityRoot)
            {
                t.transform.SetParent(m_EntityRoot.transform);
                t.transform.localPosition = Vector3.zero;
                t.transform.localScale = Vector3.one;
                if (entity.V_EnityType == EM_EntityType.Wooden)
                {
                    newEntityView = t.AddComponent<Wooden>() as V;
                    newEntityView.F_Init(entity);
                }
                else
                {
                    EntityView view = t.AddComponent<EntityView>();
                    view.F_Init(entity);
                    newEntityView = view as V;
                }
                if (null != newEntityView)
                {
                    if (!m_EntityTypeDic.ContainsKey(entity.V_EnityType))
                        m_EntityTypeDic.Add(entity.V_EnityType, new Dictionary<long, EntityView>());
                    if (!m_EntityTypeDic[entity.V_EnityType].ContainsKey(m_playerId))
                        m_EntityTypeDic[entity.V_EnityType].Add(m_playerId, newEntityView);
                }

            }
        }
        return newEntityView;
    }
    public EntityView F_GetEnityView(EM_EntityType entityType, long id)
    {
        if (m_EntityTypeDic.ContainsKey(entityType) && m_EntityTypeDic[entityType].ContainsKey(m_playerId))
            return m_EntityTypeDic[entityType][m_playerId];
        return null;
    }
}




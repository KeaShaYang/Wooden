using System.Collections.Generic;
using UnityEngine;

public class Cube : BaseMonoBehaviour
{
    private Rigidbody m_rigidBody;
    public Sting[] m_StingList;
    private int lastStingNum = 0;
    private List<Sting> m_connetSting = new List<Sting>();
    private Material m_material;

    // Start is called before the first frame update
    protected override void Awake()
    {
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        m_material = renderer.material;
        m_rigidBody = gameObject.GetComponent<Rigidbody>();
        if (m_rigidBody == null)
            m_rigidBody = gameObject.AddComponent<Rigidbody>();
        m_rigidBody.useGravity = false;
        m_rigidBody.isKinematic = true;
        m_StingList = GetComponentsInChildren<Sting>();
        for (int i = 0; i < m_StingList.Length; i++)
        {
            m_StingList[i].F_SetParentCube(this,null);
        }
        lastStingNum = m_StingList.Length;
        int colorType = Random.Range(1, 9);
        q_colorExcelItem cfg = ExcelManager.GetInstance().GetExcelItem<q_color, q_colorExcelItem>(colorType);
        // 修改颜色为红色
        Color color = new Color();
        if (null != cfg && ColorUtility.TryParseHtmlString(cfg.valueType, out color))
        {
            m_material.color = color;
        }
    }
    /// <summary>
    /// 关联的sting特殊处理一下
    /// </summary>
    /// <param name="sting"></param>
    public void F_AddSting(Sting sting)
    {
        if (null != sting)
        {
            lastStingNum++;
            m_connetSting.Add(sting);
        }
    }
    public void F_RemoveSting(Sting sting)
    {
        if (null != sting)
        {
            lastStingNum--;
            if (lastStingNum == 0)
            {
                // 开启重力
                m_rigidBody.useGravity = true;
                m_rigidBody.isKinematic = false;
                TimerMgr.GetInstance().Schedule(()=> {
                    m_rigidBody.useGravity = false;
                    m_rigidBody.isKinematic = true;
                    gameObject.SetActive(false);
                }, 1, 1, 1);
                //延迟destroy自己
            }
        }
    }
}

using Assets.Scripts.Define;
using UnityEngine;
using DG.Tweening;

public class Sting : BaseMonoBehaviour
{
    EM_EntityType V_Type = EM_EntityType.Sting;
    private Vector3 m_currentPos;
    private Rigidbody m_rigidBody;
    [HideInInspector]
    public bool isMoved = false;
    private Cube m_Parent1;
    private Cube m_Parent2;
    private int m_colorType = 1;
    public int V_ColorType { get { return m_colorType; } }

    /// <summary>
    /// 已点击过，避免重复移动
    /// </summary>
    private bool m_isMoving = false;
    Material m_material;
    // Start is called before the first frame update
    void Start()
    {
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        m_material = renderer.material;
        m_rigidBody = gameObject.GetComponent<Rigidbody>();
        if (m_rigidBody == null)
            m_rigidBody = gameObject.AddComponent<Rigidbody>();
        m_rigidBody.useGravity = false;
        m_rigidBody.isKinematic = true;
    }
    public void F_Init(int index,int colorType)
    {
        gameObject.name = "sting" + index;
        m_colorType = colorType;
        q_colorExcelItem cfg = ExcelManager.GetInstance().GetExcelItem<q_color, q_colorExcelItem>(colorType);
        // 修改颜色为红色
        Color color = new Color();
        if (ColorUtility.TryParseHtmlString(cfg.valueType, out color))
        {
            m_material.color = color;
        }
    }
    public void F_SetParentCube(Cube c1, Cube c2)
    {
        m_Parent1 = c1;
        m_Parent2 = c2;
    }
    public void F_Move()
    {
        if (!m_isMoving)
        {
            m_isMoving = true;
            m_currentPos = transform.position;
            //拔钉子
            //返回是否可拔，播放动画
            //拔成功的话就增加重力
            //碰到物体（箱子或钉子或其他就失败）
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.up, out hit))
            {
                //如果移动距离大于钉子长度，说明拔出来了，否则要移动回去
                if (hit.distance > 1)
                {
                    F_RemoveSting();
                }
                else
                {
                    float time = hit.distance / 2;
                    //移动失败了，先移动到碰撞的位置然后挪获取
                    transform.DOMove(hit.point, time).SetSpeedBased(true).OnComplete(() =>
                    {
                        transform.DOMove(m_currentPos, time);
                    });
                }

            }
            else
            {
                F_RemoveSting();
            }
        }
    }
    private void F_RemoveSting()
    {
        transform.DOMove(m_currentPos + new Vector3(0, 1, 0), 2f).SetSpeedBased(true).OnComplete(() =>
        {
            isMoved = true;
            LevelMgr.GetInstance().V_Model.F_RemoveSting(this);
            // 移动到屏幕中央
            if (m_Parent1 != null)
            {
                m_Parent1.F_RemoveSting(this);
            }
            if (m_Parent2 != null)
            {
                m_Parent2.F_RemoveSting(this);
            }
        });
        
    }
}

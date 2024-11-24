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
    /// <summary>
    /// 钉子颜色
    /// </summary>
    private int m_colorType = 1;
    public int V_ColorType { get { return m_colorType; } }
    private int m_index = 0;
    /// <summary>
    /// 钉子序号
    /// </summary>
    public int V_Index { get { return m_index; } }
    /// <summary>
    /// 已点击过，避免重复移动
    /// </summary>
    private bool m_isMoving = false;
    Material m_material;
    private CapsuleCollider m_collider;
    private Tween _tween;

    // Start is called before the first frame update
    protected override void Awake()
    {
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        m_material = renderer.material;
        m_collider = GetComponent<CapsuleCollider>();
        m_rigidBody = gameObject.GetComponent<Rigidbody>();
        if (m_rigidBody == null)
            m_rigidBody = gameObject.AddComponent<Rigidbody>();
        m_rigidBody.useGravity = false;
        m_rigidBody.isKinematic = true;
    }
    public void F_Init(int index, int colorType)
    {
        gameObject.name = "sting" + index;
        m_colorType = colorType;
        q_colorExcelItem cfg = ExcelManager.GetInstance().GetExcelItem<q_color, q_colorExcelItem>(colorType);
        // 修改颜色为红色
        Color color = new Color();
        if (null != cfg && ColorUtility.TryParseHtmlString(cfg.valueType, out color))
        {
            if (null == color)
                Debugger.LogError("颜色错误");
            else
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
            Vector3 hitPos;
            bool canMove = LevelMgr.GetInstance().V_Model.F_CanMoveSting(this, out hitPos);
            if (canMove)
            {
                F_RemoveSting();
            }
            else
            {
                m_currentPos = transform.position;
                float time = Vector3.Distance(hitPos, transform.position) / 2;
                _tween = transform.DOMove(hitPos, time).OnComplete(() =>
                {
                    _tween = transform.DOMove(m_currentPos, time).OnComplete(() => { m_isMoving = false; });
                });
            }
        }
    }
    private void F_RemoveSting()
    {
        m_collider.enabled = false;
        Destroy(m_rigidBody);
        m_rigidBody = null;
        _tween = transform.DOLocalMoveY(transform.position.y+0.5f, 2f).SetSpeedBased(true).OnComplete(() =>
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
    public void F_RotateToScreen(Vector3 screenDirection)
    {
        // 计算钉子需要旋转的四元数，使up方向朝向屏幕
        Quaternion targetRotation = Quaternion.FromToRotation(transform.up, screenDirection) * transform.rotation;
        // 使用DOTween旋转钉子到目标旋转状态
        transform.DORotateQuaternion(targetRotation, 0.5f);
    }

    public void F_MoveToSlot(Vector3 targetPosition, RectTransform slotRect, System.Action onComplete)
    {
        // 使用DOTween缩放钉子到指定大小
        //transform.DOScale(new Vector3(2, 2, 2), 0.5f);
        // 使用DOTween移动钉子到目标位置
        _tween = transform.DOLocalMove(targetPosition, 0.5f).OnComplete(() =>
        {
            // 将字符串的父对象设置为插槽的变换组件
            //transform.SetParent(slotRect.transform);
            // 执行完成后的回调
            onComplete?.Invoke();
        });
    }
    private void OnDestroy()
    {
        if (_tween != null)
            _tween.Kill();
    }
}

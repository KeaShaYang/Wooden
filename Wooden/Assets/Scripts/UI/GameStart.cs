using UnityEngine;

public class GameStart : MonoBehaviour
{
    public RenderTexture m_Render;
    public Camera m_Camera;
    private Vector2 previousTouchPosition;
    private float rotationSpeed = 0.5f;
    private Wooden m_wooden;
    void Start()
    {
        GameMgr.GetInstance().F_InitManager();
        BaseWindow win = UIManager.GetInstance().GetSingleUI(Assets.Scripts.Define.EM_WinType.WinLevel);
        if (null != win)
        {
            WinLevel winLevel = win as WinLevel;
            //todo:obj就是每个关卡要拆的模型，后面要改成动态生成
            winLevel.F_Init(m_Render);
        }
    }

    private void Update()
    {
        TimerMgr.GetInstance().Update();
        m_wooden = LevelMgr.GetInstance().V_Model.F_GetCurrentWooden();
        if (null != m_wooden)
        {
            if (Input.GetMouseButtonDown(0))
            {
                previousTouchPosition = Input.mousePosition;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    Sting sting = hit.transform.GetComponent<Sting>();
                    if (null != sting)
                    {
                        sting.F_Move();
                    }
                }
            }
            else if (Input.GetMouseButton(0))
            {
                Vector2 currentMousePosition = Input.mousePosition;
                Vector2 delta = currentMousePosition - previousTouchPosition;

                // 绕世界Y轴旋转
                m_wooden.transform.Rotate(delta.y * rotationSpeed, -delta.x * rotationSpeed, 0, Space.World);

                previousTouchPosition = currentMousePosition;
            }
        }

    }
}

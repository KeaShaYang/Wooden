﻿using UnityEngine;

public class GameStart : BaseMonoBehaviour
{
    private Vector2 previousTouchPosition;
    private float rotationSpeed = 0.5f;
    private Wooden m_wooden;
    private AudioManager m_audioMgr;
    protected override void Awake()
    {
        DontDestroyOnLoad(this);
        GameMgr.GetInstance().F_InitManager();
        DontDestroyOnLoad(GameObjectPool.GetInstance().V_PoolRoot);
        m_audioMgr = GetComponent<AudioManager>();
        m_audioMgr.PlayBGMAudio("game_bgm_06");
    }
    void Start()
    {
        UIManager.GetInstance().GetSingleUI(Assets.Scripts.Define.EM_WinType.WinMain);
       
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

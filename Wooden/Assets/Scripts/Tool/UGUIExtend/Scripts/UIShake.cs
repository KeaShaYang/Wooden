using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UI震动效果
/// </summary>
public class UIShake : MonoBehaviour
{
    [SerializeField]
    RectTransform m_Target;
    /// <summary>
    /// 两方向变化曲线
    /// </summary>
    public AnimationCurve xCurve = new AnimationCurve();
    public AnimationCurve yCurve = new AnimationCurve();

    /// <summary>
    /// 震动最大偏移
    /// </summary>
    public float xMax = 0f;
    public float yMax = 0f;

    /// <summary>
    /// 持续时间
    /// </summary>
    public float Duration = 1f;

    private float m_Timer = 0f;
    private Vector2 startPos = Vector2.zero;
    private float tempx = 0f;
    private float tempy = 0f;
    private Vector2 tempVec = Vector2.zero;
    private Vector2 lastVec = Vector2.zero;
    private RectTransform tsform = null;
    private bool isStart = false;

    void Awake()
    {
        if(m_Target==null)
        {
            tsform = this.GetComponent<RectTransform>();
        }
        else
        {
            tsform = m_Target;
        }
    }

    public void F_Start()
    {
        if (isStart) F_Stop();
        startPos = tsform.anchoredPosition;
        m_Timer = 0f;
        isStart = true;
    }

    public void F_Stop()
    {
        if (isStart)
        {
            tsform.anchoredPosition = startPos;
            isStart = false;
            lastVec = Vector3.zero;
        }
    }

    /// <summary>
    /// 暂停震动
    /// </summary>
    public void Pause()
    {
        isStart = true;
    }

    /// <summary>
    /// 恢复震动
    /// </summary>
    public void Resume()
    {
        isStart = false;
    }

    void LateUpdate()
    {
        if (!this || !tsform)
            return;
        if (isStart)
        {
            if (m_Timer < Duration)
            {
                tempx = xCurve.Evaluate(m_Timer) * xMax;
                tempy = yCurve.Evaluate(m_Timer) * yMax;
                tempVec = new Vector2(tempx, tempy);
                tsform.anchoredPosition = tsform.anchoredPosition + tempVec - lastVec;
                lastVec = tempVec;
                m_Timer += Time.deltaTime;
            }
            else
            {
                tsform.anchoredPosition = startPos;
                isStart = false;
            }
        }
    }
}

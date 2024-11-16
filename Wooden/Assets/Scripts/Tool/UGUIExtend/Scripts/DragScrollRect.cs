using Tool.UGUIExtend.Scripts;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragScrollRect : MonoBehaviour, IInitializePotentialDragHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IScrollHandler
{
    [SerializeField] private MyScrollRect m_MyScrollRect;

    [SerializeField] private MyScrollRect m_SelfMyScrollRect;

    /// <summary>
    /// 传递 Y 滑动(竖向)
    /// </summary>
    [SerializeField] private bool m_TransY = true;

    /// <summary>
    /// 传递 X 滑动(横向)
    /// </summary>
    [SerializeField] private bool m_TransX = false;

    bool canTrans = false;

    public void OnInitializePotentialDrag(PointerEventData eventData)
    {
        if (m_MyScrollRect) m_MyScrollRect.OnInitializePotentialDrag(eventData);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        HandleEventData(eventData);
        if (m_MyScrollRect) m_MyScrollRect.OnBeginDrag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canTrans = false;
        if (m_MyScrollRect) m_MyScrollRect.OnEndDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        //eventData = HandleEventData(eventData);
        if (canTrans)
        {
            if (m_MyScrollRect) m_MyScrollRect.OnDrag(eventData);
        }
    }

    public void OnScroll(PointerEventData eventData)
    {
        if (canTrans)
        {
            if (m_MyScrollRect) m_MyScrollRect.OnScroll(eventData);
        }
    }

    void HandleEventData(PointerEventData eventData) {
        canTrans = true;
        if (m_TransY != m_TransX)
        {
            //应该不会同时false吧，不然就没意义了


            if (m_TransY)
            {
                //只传递Y
                if (Mathf.Abs(eventData.delta.y) <= Mathf.Abs(eventData.delta.x))
                {
                    //y的值少的话，就不传递了
                    //eventData.position = eventData.pressPosition;
                    canTrans = false;
                }
                else
                {
                    if (m_SelfMyScrollRect != null)
                    {
                        m_SelfMyScrollRect.OnEndDrag(eventData);
                    }
                }
            }

            if (m_TransX)
            {
                //只传递X
                if (Mathf.Abs(eventData.delta.x) <= Mathf.Abs(eventData.delta.y))
                {
                    //x的值少的话，就不传递了
                    //eventData.position = eventData.pressPosition;
                    canTrans = false;
                }
                else
                {
                    if (m_SelfMyScrollRect != null)
                    {
                        m_SelfMyScrollRect.OnEndDrag(eventData);
                    }
                }
            }
        }

        //Debug.LogError(canTrans + "  " + eventData.delta);
    }
}

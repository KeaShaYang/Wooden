using Tool.UGUIExtend.Scripts;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragScrollRect : MonoBehaviour, IInitializePotentialDragHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IScrollHandler
{
    [SerializeField] private MyScrollRect m_MyScrollRect;

    [SerializeField] private MyScrollRect m_SelfMyScrollRect;

    /// <summary>
    /// ���� Y ����(����)
    /// </summary>
    [SerializeField] private bool m_TransY = true;

    /// <summary>
    /// ���� X ����(����)
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
            //Ӧ�ò���ͬʱfalse�ɣ���Ȼ��û������


            if (m_TransY)
            {
                //ֻ����Y
                if (Mathf.Abs(eventData.delta.y) <= Mathf.Abs(eventData.delta.x))
                {
                    //y��ֵ�ٵĻ����Ͳ�������
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
                //ֻ����X
                if (Mathf.Abs(eventData.delta.x) <= Mathf.Abs(eventData.delta.y))
                {
                    //x��ֵ�ٵĻ����Ͳ�������
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

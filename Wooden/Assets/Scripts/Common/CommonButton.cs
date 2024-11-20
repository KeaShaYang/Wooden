using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class CommonButton : BaseMonoBehaviour,IPointerClickHandler
{
    public Text m_Text;
    public Text m_TextDis;
    public GameObject m_ActiveIcon;
    public GameObject m_disAciveIcon;
    private bool m_isSelect = false;
    private int m_index = 0;
    public int V_Index  { get { return m_index; } }
    Action<CommonButton> m_clickCall = null;
    public bool V_isSelect { get { return m_isSelect; } }
    void Start()
    {
        if (null != m_ActiveIcon)
            m_ActiveIcon.gameObject.SetActive(false);
        if (null != m_disAciveIcon)
            m_disAciveIcon.gameObject.SetActive(true);
    }
    public void F_ForceClick()
    {
        if (null != m_clickCall)
        {
            m_clickCall(this);
        }
        else
        {
            F_SetSelect(true);
        }
    }
    public void F_SetClickCall(Action<CommonButton> clickCall,int idx = 0)
    {
        m_clickCall = clickCall;
        m_index = idx;
    }
    public void F_Refresh(string name)
    {
        if (null != m_Text)
            m_Text.text = name;
        if (null != m_TextDis)
            m_TextDis.text = name;
    }
    public void F_SetSelect(bool select)
    {
        m_isSelect = select;
        m_ActiveIcon.SetActive(select);
        m_disAciveIcon.SetActive(!select);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (null != m_clickCall)
        {
            m_clickCall(this);
        }
        else
        {
            F_SetSelect(true);
        }
    }
}

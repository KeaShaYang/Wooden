using System;
using System.Collections.Generic;

public class Notifier
{
    public delegate void StandardDelegate(params object[] arg1);
    private Dictionary<string, StandardDelegate> m_eventMap = new Dictionary<string, StandardDelegate>();

    public void AddEventHandler(string eventName, StandardDelegate pFunc)
    {
        if (!m_eventMap.ContainsKey(eventName))
        {
            m_eventMap[eventName] = pFunc;
        }
        else
        {
            m_eventMap[eventName] += pFunc;
        }
    }
    public void RemoveEventHandler(string eventName, StandardDelegate pFunc)
    {
        if (m_eventMap.ContainsKey(eventName))
        {
            if (m_eventMap[eventName] != null)
            {
                m_eventMap[eventName] -= pFunc;
            }
        }

    }
    public void RasiseEvent(string eventName, params object[] e)
    {
        StandardDelegate fun = null;
        if (m_eventMap.TryGetValue(eventName, out fun))
        {
            if (null != fun)
            {
                fun(e);
            }
        }
    }
    public bool HasEvent(string eventName)
    {
        return m_eventMap.ContainsKey(eventName);
    }
    public void RemoveAllEventHandler(string eventName)
    {
        if (m_eventMap.ContainsKey(eventName))
        {
            if (m_eventMap[eventName] != null)
            {
                Delegate[] delegArr = m_eventMap[eventName].GetInvocationList();
                for (int i = 0; i < delegArr.Length; i++)
                {
                    StandardDelegate deleg = (StandardDelegate)delegArr[i];
                    RemoveEventHandler(eventName, deleg);
                }
            }
        }

    }
    public void ClearEventHandle()
    {
        if (m_eventMap.Count <= 0) return;
        List<string> DicKeys = new List<string>(m_eventMap.Keys);
        for (int i = 0; i < DicKeys.Count; i++)
        {
            RemoveAllEventHandler(DicKeys[i]);
        }
        m_eventMap.Clear();
    }
}

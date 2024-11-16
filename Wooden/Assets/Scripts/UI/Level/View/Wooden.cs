

using System;
using UnityEngine;

public class Wooden : EntityView
{
    private Sting[] m_Stings;
    // Start is called before the first frame update
    void Start()
    {
        m_Stings = GetComponentsInChildren<Sting>();
    }

    internal void F_Init()
    {

        for (int i = 0; i < m_Stings.Length; i++)
        {
            m_Stings[i].F_Init(i,LevelMgr.GetInstance().V_Model.F_GetColor(i));
        }
    }

    
}

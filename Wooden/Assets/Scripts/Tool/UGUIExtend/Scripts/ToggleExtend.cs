using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Tool.UGUIExtend.Scripts
{
    [RequireComponent(typeof(Toggle))]
    public class ToggleExtend : MonoBehaviour
    {
        /// <summary>
        /// 是否需要拓展点击区域
        /// </summary>
        [SerializeField] bool m_ExtendClickArea;
        /// <summary>
        /// 是否需要切换游戏物体
        /// </summary>
        [SerializeField] bool m_ToggleGameObject;

        [SerializeField] GameObject m_ObjOn;
        [SerializeField] GameObject m_ObjOff;

        Toggle m_Toggle;

        void Awake()
        {
            m_Toggle = GetComponent<Toggle>();
            m_Toggle.onValueChanged.AddListener(OnToggleValueChange);
        }

        private void OnEnable()
        {
            OnToggleValueChange(m_Toggle.isOn);
        }

        void OnToggleValueChange(bool value)
        {
            if (!m_ToggleGameObject) return;
            if (m_ObjOn != null)
            {
                m_ObjOn.gameObject.SetActive(value);
            }
            if (m_ObjOff != null)
            {
                m_ObjOff.gameObject.SetActive(!value);
            }
        }
    }
}
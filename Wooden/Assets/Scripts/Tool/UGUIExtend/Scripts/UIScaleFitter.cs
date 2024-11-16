using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("Layout/UI Scale Fitter (Custom)")]
[ExecuteInEditMode]
[RequireComponent(typeof(RectTransform))]
public class UIScaleFitter : MonoBehaviour
{
    [SerializeField]
    private RectTransform m_Parent;
    [SerializeField]
    private Vector2 m_OriSize;

    private void OnEnable()
    {
        if (m_Parent == null)
        {
            return;
        }

        Vector2 curSize = m_Parent.sizeDelta;
        transform.localScale = new Vector3(curSize.x / m_OriSize.x, curSize.y / m_OriSize.y, 1);
    }
}

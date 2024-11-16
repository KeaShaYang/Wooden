using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Tool.UGUIExtend.Scripts
{
    public class MyInputModule : StandaloneInputModule
    {
        public PointerEventData GetPointerEventData(int pointerId)
        {
            return GetLastPointerEventData(pointerId);
        }
    }
}
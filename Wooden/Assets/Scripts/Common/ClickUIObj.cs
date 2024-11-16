using System;
using UnityEngine.EventSystems;

namespace Assets.Scripts.UI
{
    public class ClickUIObj : BaseMonoBehaviour, IPointerClickHandler
    {
        public object otherArgs = null;
        private Action<ClickUIObj> m_click;
        public void F_SetClickCall(Action<ClickUIObj> clickCall )
        {
            m_click = clickCall;
        }
        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (m_click != null)
            {
                m_click(this);
            }
        }
    }
}

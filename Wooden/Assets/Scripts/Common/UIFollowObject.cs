using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Common
{
    public class UIFollowObject : MonoBehaviour
    {
        private GameObject m_FollowObj;
        private bool m_needUpdate = false;
        public void F_SetFollowObj(GameObject obj,float offsetY = 0,bool needUpdate = false)
        {
            m_FollowObj = obj;
            if (null != m_FollowObj)
            {
                Vector3 targetPos = m_FollowObj.transform.position;
                targetPos.y += 2;
                Vector3 pos = Camera.main.WorldToScreenPoint(targetPos);
                if (offsetY > 0)
                    pos.y += offsetY;
                transform.position = pos;
                //transform.DOMove(targetPos, 0.2f);
            }
        }
        public void Update()
        {
            if (m_needUpdate && null != m_FollowObj)
            {
                Vector3 pos = Camera.main.WorldToScreenPoint(m_FollowObj.transform.position);
                transform.position = pos;
            }
        }

        public static void FlyTo(Graphic graphic)
        {
            RectTransform rt = graphic.rectTransform;
            Color c = graphic.color;
            c.a = 0;
            graphic.color = c;
            Sequence mySequence = DOTween.Sequence();
            Tweener move1 = rt.DOMoveY(rt.position.y + 50, 0.5f);
            Tweener move2 = rt.DOMoveY(rt.position.y + 100, 0.5f);
            Tweener alpha1 = graphic.DOColor(new Color(c.r, c.g, c.b, 1), 0.5f);
            Tweener alpha2 = graphic.DOColor(new Color(c.r, c.g, c.b, 0), 0.5f);
            mySequence.Append(move1);
            mySequence.Join(alpha1);
            mySequence.AppendInterval(1);
            mySequence.Append(move2);
            mySequence.Join(alpha2);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tool.UGUIExtend.Scripts
{
    [RequireComponent(typeof(MyScrollRect))]
    [AddComponentMenu("UI/Tween/Tween ScrollRect")]
    public class TweenScrollRect : UITweener
    {
        public Vector2 from = Vector2.zero;
        public Vector2 to = Vector2.one;

        private MyScrollRect mScrollRect;
        
        public MyScrollRect cachedScrollRect { get { if (mScrollRect == null) mScrollRect = GetComponent<MyScrollRect>(); return mScrollRect; } }
        
        public Vector2 value { get { return cachedScrollRect.normalizedPosition; } set { cachedScrollRect.normalizedPosition = value; } }
        
        protected override void OnUpdate(float factor, bool isFinished)
        {
            value = from * (1f - factor) + to * factor;
        }
        
        public static TweenScrollRect Begin (GameObject go, float duration, Vector2 normalizedPosition)
        {
            TweenScrollRect comp = UITweener.Begin<TweenScrollRect>(go, duration);
            comp.from = comp.value;
            comp.to = normalizedPosition;

            if (duration <= 0f)
            {
                comp.Sample(1f, true);
                comp.enabled = false;
            }
            return comp;
        }
    }
}
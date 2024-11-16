using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Tool.UGUIExtend.Scripts
{
    [RequireComponent(typeof(RawImage))]
    public class TweenUVRect : UITweener
    {
        public Rect from;
        public Rect to;
        
        RawImage mRawImage;
        public RawImage cachedImage { get { if (mRawImage == null) mRawImage = GetComponent<RawImage>(); return mRawImage; } }
        
        public Rect value { 
            get
            {
                return new Rect(tweenFactor * (to.x - from.x) + from.x,
                    tweenFactor * (to.y - from.y) + from.y,
                    tweenFactor * (to.width - from.width) + from.width,
                    tweenFactor * (to.height - from.height) + from.height);
            }
            set
            {
                cachedImage.uvRect = value;
            } 
        }
        
        protected override void OnUpdate(float factor, bool isFinished)
        {
            value = new Rect(factor * (to.x - from.x) + from.x,
                factor * (to.y - from.y) + from.y,
                factor * (to.width - from.width) + from.width,
                factor * (to.height - from.height) + from.height);
        }
        
        public static TweenUVRect Begin (GameObject go, float duration, Rect uvRect)
        {
            TweenUVRect comp = UITweener.Begin<TweenUVRect>(go, duration);
            comp.from = comp.value;
            comp.to = uvRect;

            if (duration <= 0f)
            {
                comp.Sample(1f, true);
                comp.enabled = false;
            }
            return comp;
        }
    }
}
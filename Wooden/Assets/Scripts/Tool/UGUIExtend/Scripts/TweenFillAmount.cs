using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TweenFillAmount : UITweener
{
    public float from = 0;
    public float to = 1;

    Image mImg;
    
    public Image cachedImage { get { if (mImg == null) mImg = GetComponent<Image>(); return mImg; } }
    
    public float value { get { return cachedImage.fillAmount; } set { cachedImage.fillAmount = value; } }
    
    protected override void OnUpdate(float factor, bool isFinished)
    {
        value = from * (1f - factor) + to * factor;
    }
    
    static public TweenFillAmount Begin (GameObject go, float duration, float number)
    {
        TweenFillAmount comp = UITweener.Begin<TweenFillAmount>(go, duration);
        comp.from = comp.value;
        comp.to = number;

        if (duration <= 0f)
        {
            comp.Sample(1f, true);
            comp.enabled = false;
        }
        return comp;
    }
}

﻿using UnityEngine;

/// <summary>
/// Tween the object's position.
/// </summary>

[AddComponentMenu("UI/Tween/Tween TransformPosition")]
public class TweenTransformPosition : UITweener
{
    public Vector3 from;
    public Vector3 to;

    [HideInInspector]
    public bool worldSpace = false;

    Transform mTrans;

    public Transform cachedTransform { get { if (mTrans == null) mTrans = GetComponent<Transform>(); return mTrans; } }

    /// <summary>
    /// Tween's current value.
    /// </summary>

    public Vector3 value
    {
        get
        {
            return worldSpace ? cachedTransform.position : cachedTransform.localPosition;
        }
        set
        {
            if (worldSpace) cachedTransform.position = value;
            else cachedTransform.localPosition = value;
        }
    }

    /// <summary>
    /// Tween the value.
    /// </summary>

    protected override void OnUpdate(float factor, bool isFinished) { value = from * (1f - factor) + to * factor; }

    /// <summary>
    /// Start the tweening operation.
    /// </summary>

    static public TweenTransformPosition Begin(GameObject go, float duration, Vector3 pos)
    {
        TweenTransformPosition comp = UITweener.Begin<TweenTransformPosition>(go, duration);
        comp.from = comp.value;
        comp.to = pos;

        if (duration <= 0f)
        {
            comp.Sample(1f, true);
            comp.enabled = false;
        }
        return comp;
    }

    /// <summary>
    /// Start the tweening operation.
    /// </summary>

    static public TweenTransformPosition Begin(GameObject go, float duration, Vector3 pos, bool worldSpace)
    {
        TweenTransformPosition comp = UITweener.Begin<TweenTransformPosition>(go, duration);
        comp.worldSpace = worldSpace;
        comp.from = comp.value;
        comp.to = pos;

        if (duration <= 0f)
        {
            comp.Sample(1f, true);
            comp.enabled = false;
        }
        return comp;
    }

    [ContextMenu("Set 'From' to current value")]
    public override void SetStartToCurrentValue() { from = value; }

    [ContextMenu("Set 'To' to current value")]
    public override void SetEndToCurrentValue() { to = value; }

    [ContextMenu("Assume value of 'From'")]
    void SetCurrentValueToStart() { value = from; }

    [ContextMenu("Assume value of 'To'")]
    void SetCurrentValueToEnd() { value = to; }
}

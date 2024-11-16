//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2015 Tasharen Entertainment
//----------------------------------------------

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum TweenDirection
{
	Reverse = -1,
	Toggle = 0,
	Forward = 1,
}

/// <summary>
/// Base class for all tweening operations.
/// </summary>

public abstract class UITweener : MonoBehaviour
{
	/// <summary>
	/// Current tween that triggered the callback function.
	/// </summary>

	static public UITweener current;

	public enum Method
	{
		Linear = 1,
		EaseIn = 2,
		EaseOut = 3,
		EaseInOut = 4,
		BounceIn = 5,
		BounceOut = 6,
		EaseOutQuart = 7,
	}

	public enum Style
	{
		Once,
		Loop,
		PingPong,
	}

	/// <summary>
	/// Tweening method used.
	/// </summary>

	[HideInInspector]
	public Method method = Method.Linear;

	/// <summary>
	/// Does it play once? Does it loop?
	/// </summary>

	[HideInInspector]
	public Style style = Style.Once;

	/// <summary>
	/// Optional curve to apply to the tween's time factor value.
	/// </summary>

	[HideInInspector]
	public AnimationCurve animationCurve = new AnimationCurve(new Keyframe(0f, 0f, 0f, 1f), new Keyframe(1f, 1f, 1f, 0f));

	/// <summary>
	/// Whether the tween will ignore the timescale, making it work while the game is paused.
	/// </summary>
	
	[HideInInspector]
	public bool ignoreTimeScale = true;

	/// <summary>
	/// How long will the tweener wait before starting the tween?
	/// </summary>

	[HideInInspector]
	public float delay = 0f;

	/// <summary>
	/// How long is the duration of the tween?
	/// </summary>

	[HideInInspector]
	public float duration = 1f;

	/// <summary>
	/// Whether the tweener will use steeper curves for ease in / out style interpolation.
	/// </summary>

	[HideInInspector]
	public bool steeperCurves = false;

	/// <summary>
	/// Event delegates called when the animation finishes.
	/// </summary>

	[HideInInspector] public Action onFinished;

	private int lastFinishCallbackFrameCount = 0;
	

	bool mStarted = false;
    bool mPause = false;
	float mStartTime = 0f;
	float mDuration = 0f;
	float mAmountPerDelta = 1000f;
	float mFactor = 0f;

	/// <summary>
	/// Amount advanced per delta time.
	/// </summary>

	public float amountPerDelta
	{
		get
		{
			if (mDuration != duration)
			{
				mDuration = duration;
				mAmountPerDelta = Mathf.Abs((duration > 0f) ? 1f / duration : 1000f) * Mathf.Sign(mAmountPerDelta);
			}
			return mAmountPerDelta;
		}
	}

	/// <summary>
	/// Tween factor, 0-1 range.
	/// </summary>

	public float tweenFactor { get { return mFactor; } set { mFactor = Mathf.Clamp01(value); } }

	/// <summary>
	/// Direction that the tween is currently playing in.
	/// </summary>

	public TweenDirection direction { get { return amountPerDelta < 0f ? TweenDirection.Reverse : TweenDirection.Forward; } }

	/// <summary>
	/// This function is called by Unity when you add a component. Automatically set the starting values for convenience.
	/// </summary>

	void Reset ()
	{
		if (!mStarted)
		{
			SetStartToCurrentValue();
			SetEndToCurrentValue();
		}
	}

	/// <summary>
	/// Update as soon as it's started so that there is no delay.
	/// </summary>

	protected virtual void Start () { Update(); }

	/// <summary>
	/// Update the tweening factor and call the virtual update function.
	/// </summary>

	void Update ()
	{
        if (mPause) return;
		float delta = ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;
		float time = ignoreTimeScale ? Time.unscaledTime : Time.time;

		if (!mStarted)
		{
			mStarted = true;
			mStartTime = time + delay;
		}

		if (time < mStartTime) return;

		// Advance the sampling factor
		mFactor += amountPerDelta * delta;

		// Loop style simply resets the play factor after it exceeds 1.
		if (style == Style.Loop)
		{
			if (mFactor > 1f)
			{
				mFactor -= Mathf.Floor(mFactor);
			}
		}
		else if (style == Style.PingPong)
		{
			// Ping-pong style reverses the direction
			if (mFactor > 1f)
			{
				mFactor = 1f - (mFactor - Mathf.Floor(mFactor));
				mAmountPerDelta = -mAmountPerDelta;
			}
			else if (mFactor < 0f)
			{
				mFactor = -mFactor;
				mFactor -= Mathf.Floor(mFactor);
				mAmountPerDelta = -mAmountPerDelta;
			}
		}

		// If the factor goes out of range and this is a one-time tweening operation, disable the script
		if ((style == Style.Once) && (duration == 0f || mFactor > 1f || mFactor < 0f))
		{
			mFactor = Mathf.Clamp01(mFactor);
			Sample(mFactor, true);
			enabled = false;

			// 这里控制一帧只能回调一次，否则可能出现回调中再次播放，而deltaTime太大，直接播放完成了，又走到这里，导致再次回调，形成死循环
			int frameCount = Time.frameCount;
			if (onFinished != null && lastFinishCallbackFrameCount != frameCount)
			{
				lastFinishCallbackFrameCount = frameCount;
				onFinished();
			}
		}
		else Sample(mFactor, false);
	}

	/// <summary>
	/// Convenience function -- set a new OnFinished event delegate (here for to be consistent with RemoveOnFinished).
	/// </summary>

	public void SetOnFinished(Action del) { onFinished = del; }

	/// <summary>
	/// Convenience function -- add a new OnFinished event delegate (here for to be consistent with RemoveOnFinished).
	/// </summary>

	public void AddOnFinished (Action del) { onFinished += del; }

	/// <summary>
	/// Remove an OnFinished delegate. Will work even while iterating through the list when the tweener has finished its operation.
	/// </summary>

	public void RemoveOnFinished (Action del)
	{
		if (onFinished != null) onFinished -= del;
	}

	/// <summary>
	/// Mark as not started when finished to enable delay on next play.
	/// </summary>

	void OnDisable () { mStarted = false; }

	/// <summary>
	/// Sample the tween at the specified factor.
	/// </summary>

	public void Sample (float factor, bool isFinished)
	{
		// Calculate the sampling value
		float val = Mathf.Clamp01(factor);

		switch (method)
		{
			case Method.EaseIn:
				val = 1f - Mathf.Sin(0.5f * Mathf.PI * (1f - val));
				if (steeperCurves) val *= val;
				break;
			case Method.EaseOut:
				val = Mathf.Sin(0.5f * Mathf.PI * val);

				if (steeperCurves)
				{
					val = 1f - val;
					val = 1f - val * val;
				}
				break;
			case Method.EaseInOut:
				const float pi2 = Mathf.PI * 2f;
				val = val - Mathf.Sin(val * pi2) / pi2;

				if (steeperCurves)
				{
					val = val * 2f - 1f;
					float sign = Mathf.Sign(val);
					val = 1f - Mathf.Abs(val);
					val = 1f - val * val;
					val = sign * val * 0.5f + 0.5f;
				}
				break;
			case Method.BounceIn:
				val = BounceLogic(val);
				break;
			case Method.BounceOut:
				val = 1f - BounceLogic(1f - val);
				break;
			case Method.EaseOutQuart:
				val = -Mathf.Pow(val - 1, 4) + 1;
				break;
		}

		// Call the virtual update
		OnUpdate((animationCurve != null) ? animationCurve.Evaluate(val) : val, isFinished);
	}

	/// <summary>
	/// Main Bounce logic to simplify the Sample function
	/// </summary>
	
	float BounceLogic (float val)
	{
		if (val < 0.363636f) // 0.363636 = (1/ 2.75)
		{
			val = 7.5685f * val * val;
		}
		else if (val < 0.727272f) // 0.727272 = (2 / 2.75)
		{
			val = 7.5625f * (val -= 0.545454f) * val + 0.75f; // 0.545454f = (1.5 / 2.75) 
		}
		else if (val < 0.909090f) // 0.909090 = (2.5 / 2.75) 
		{
			val = 7.5625f * (val -= 0.818181f) * val + 0.9375f; // 0.818181 = (2.25 / 2.75) 
		}
		else
		{
			val = 7.5625f * (val -= 0.9545454f) * val + 0.984375f; // 0.9545454 = (2.625 / 2.75) 
		}
		return val;
	}

	/// <summary>
	/// Play the tween forward.
	/// </summary>

	public void PlayForward () { Play(true); }

	/// <summary>
	/// Play the tween in reverse.
	/// </summary>
	
	public void PlayReverse () { Play(false); }

	/// <summary>
	/// Manually activate the tweening process, reversing it if necessary.
	/// </summary>

	public void Play (bool forward)
	{
        mPause = false;
        mAmountPerDelta = Mathf.Abs(amountPerDelta);
		if (!forward) mAmountPerDelta = -mAmountPerDelta;
		enabled = true;
		Update();
	}

    /// <summary>
    /// 暂停动画
    /// </summary>
    public void Pause()
    {
        mPause = true;
    }

    /// <summary>
    /// 恢复动画
    /// </summary>
    public void Resume()
    {
        mPause = false;
    }

    /// <summary>
    /// Manually reset the tweener's state to the beginning.
    /// If the tween is playing forward, this means the tween's start.
    /// If the tween is playing in reverse, this means the tween's end.
    /// </summary>

    public void ResetToBeginning ()
	{
		mStarted = false;
        mPause = false;
		mFactor = (amountPerDelta < 0f) ? 1f : 0f;
		Sample(mFactor, false);
	}

	/// <summary>
	/// Manually start the tweening process, reversing its direction.
	/// </summary>

	public void Toggle ()
	{
		if (mFactor > 0f)
		{
			mAmountPerDelta = -amountPerDelta;
		}
		else
		{
			mAmountPerDelta = Mathf.Abs(amountPerDelta);
		}
		enabled = true;
	}

	/// <summary>
	/// Actual tweening logic should go here.
	/// </summary>

	abstract protected void OnUpdate (float factor, bool isFinished);

	/// <summary>
	/// Starts the tweening operation.
	/// </summary>

	static public T Begin<T> (GameObject go, float duration) where T : UITweener
	{
		T comp = go.GetComponent<T>();
#if UNITY_FLASH
		if ((object)comp == null) comp = (T)go.AddComponent<T>();
#else
		if (comp == null)
		{
			comp = go.AddComponent<T>();

			if (comp == null)
			{
				Debug.LogError("Unable to add " + typeof(T) + " to " + go);
				return null;
			}
		}
#endif
		comp.mStarted = false;
		comp.duration = duration;
		comp.mFactor = 0f;
		comp.mAmountPerDelta = Mathf.Abs(comp.amountPerDelta);
		comp.style = Style.Once;
		comp.animationCurve = new AnimationCurve(new Keyframe(0f, 0f, 0f, 1f), new Keyframe(1f, 1f, 1f, 0f));
		comp.enabled = true;
		return comp;
	}

	/// <summary>
	/// Set the 'from' value to the current one.
	/// </summary>

	public virtual void SetStartToCurrentValue () { }

	/// <summary>
	/// Set the 'to' value to the current one.
	/// </summary>

	public virtual void SetEndToCurrentValue () { }

	public virtual void SkipToEnd()
	{
		Sample(1f, true);
		enabled = false;
	}
}

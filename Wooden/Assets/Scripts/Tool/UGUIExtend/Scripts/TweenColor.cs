using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Tween the object's color.
/// </summary>

[AddComponentMenu("UI/Tween/Tween Color")]
public class TweenColor : UITweener
{
	public Color from = Color.white;
	public Color to = Color.white;

	bool mCached = false;
	Graphic mGraphic;
	Material mMat;
	Light mLight;
	SpriteRenderer mSr;
    Color mCacheColor;

	void Cache ()
	{
		mCached = true;
		mGraphic = GetComponent<Graphic>();
		if (mGraphic != null) return;

		mSr = GetComponent<SpriteRenderer>();
		if (mSr != null) return;

		#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6|| UNITY_4_7
		Renderer ren = renderer;
#else
		Renderer ren = GetComponent<Renderer>();
#endif
		if (ren != null)
		{
			mMat = ren.material;
			return;
		}

		#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6|| UNITY_4_7
		mLight = light;
#else
		mLight = GetComponent<Light>();
#endif
		if (mLight == null) mGraphic = GetComponentInChildren<Graphic>();
	}

	/// <summary>
	/// Tween's current value.
	/// </summary>

	public Color value
	{
		get
		{
			if (!mCached) Cache();
			if (mGraphic != null) return mGraphic.color.linear;
			if (mMat != null) return mMat.color;
			if (mSr != null) return mSr.color;
			if (mLight != null) return mLight.color;
            return mCacheColor;
		}
		set
		{
			if (!mCached) Cache();
			if (mGraphic != null) mGraphic.color = value;
			else if (mMat != null) mMat.color = value;
			else if (mSr != null) mSr.color = value;
            else if (mLight != null)
            {
                mLight.color = value;
                mLight.enabled = (value.r + value.g + value.b) > 0.01f;
            }
            else
            {
                mCacheColor = value;
            }
		}
	}

	/// <summary>
	/// Tween the value.
	/// </summary>

	protected override void OnUpdate (float factor, bool isFinished) { value = Color.Lerp(from, to, factor); }

	/// <summary>
	/// Start the tweening operation.
	/// </summary>

	static public TweenColor Begin (GameObject go, float duration, Color color)
	{
#if UNITY_EDITOR
		if (!Application.isPlaying) return null;
#endif
		TweenColor comp = UITweener.Begin<TweenColor>(go, duration);
		comp.from = comp.value;
		comp.to = color;

		if (duration <= 0f)
		{
			comp.Sample(1f, true);
			comp.enabled = false;
		}
		return comp;
	}

	[ContextMenu("Set 'From' to current value")]
	public override void SetStartToCurrentValue () { from = value; }

	[ContextMenu("Set 'To' to current value")]
	public override void SetEndToCurrentValue () { to = value; }

	[ContextMenu("Assume value of 'From'")]
	void SetCurrentValueToStart () { value = from; }

	[ContextMenu("Assume value of 'To'")]
	void SetCurrentValueToEnd () { value = to; }

	private void OnDestroy()
	{
		if (mMat) Destroy(mMat);
	}
}

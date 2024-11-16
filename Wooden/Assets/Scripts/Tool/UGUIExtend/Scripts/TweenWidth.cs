using UnityEngine;

/// <summary>
/// Tween the widget's size.
/// </summary>

[RequireComponent(typeof(RectTransform))]
[AddComponentMenu("UI/Tween/Tween Width")]
public class TweenWidth : UITweener
{
	public float from = 100;
	public float to = 100;

	RectTransform mRectTransform;

	public RectTransform CachedRectTransform { get { if (mRectTransform == null) mRectTransform = GetComponent<RectTransform>(); return mRectTransform; } }

	/// <summary>
	/// Tween's current value.
	/// </summary>

	public float value { get { return CachedRectTransform.rect.width; } set { CachedRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, value);} }

	/// <summary>
	/// Tween the value.
	/// </summary>

	protected override void OnUpdate (float factor, bool isFinished)
	{
		value = Mathf.RoundToInt(from * (1f - factor) + to * factor);
	}

	/// <summary>
	/// Start the tweening operation.
	/// </summary>

	static public TweenWidth Begin (RectTransform widget, float duration, int width)
	{
		TweenWidth comp = UITweener.Begin<TweenWidth>(widget.gameObject, duration);
		comp.from = widget.rect.width;
		comp.to = width;

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
}

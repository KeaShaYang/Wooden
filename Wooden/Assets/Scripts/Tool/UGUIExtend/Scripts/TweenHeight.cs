using UnityEngine;

/// <summary>
/// Tween the widget's size.
/// </summary>
[RequireComponent(typeof(RectTransform))]
[AddComponentMenu("UI/Tween/Tween Height")]
public class TweenHeight : UITweener
{
	public float from = 100;
	public float to = 100;

	RectTransform mRectTransform;

	public RectTransform CachedRectTransform { get { if (mRectTransform == null) mRectTransform = GetComponent<RectTransform>(); return mRectTransform; } }

	/// <summary>
	/// Tween's current value.
	/// </summary>

	public float value { get { return CachedRectTransform.rect.height; } set { CachedRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, value); } }

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

	static public TweenHeight Begin (RectTransform widget, float duration, int height)
	{
		TweenHeight comp = UITweener.Begin<TweenHeight>(widget.gameObject, duration);
		comp.from = widget.rect.height;
		comp.to = height;

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

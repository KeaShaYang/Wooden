using UnityEngine.EventSystems;
using System.Collections.Generic;
using Tool.UGUIExtend.Scripts;

namespace UnityEngine.UI
{
    [AddComponentMenu("Layout/UI Size Fitter (Custom)")]
    [ExecuteInEditMode]
    [RequireComponent(typeof(RectTransform))]
    public class UISizeFitter : UIBehaviour, ILayoutSelfController
    {

        public enum FitMode
        {
            Unconstrained,
            MinSize,
            PreferredSize,
            CustomLimit
        }

        [SerializeField] protected FitMode m_HorizontalFit = FitMode.CustomLimit;
        public FitMode horizontalFit { get { return m_HorizontalFit; } set { if (SetPropertyUtility.SetStruct(ref m_HorizontalFit, value)) SetDirty(); } }

        [SerializeField] protected FitMode m_VerticalFit = FitMode.CustomLimit;
        public FitMode verticalFit { get { return m_VerticalFit; } set { if (SetPropertyUtility.SetStruct(ref m_VerticalFit, value)) SetDirty(); } }


        [SerializeField] protected float m_MaxHorizontal = -1;
        public float maxHorizontal { get { return m_MaxHorizontal; } set { if (m_MaxHorizontal != value) { m_MaxHorizontal = value; SetDirty(); } } }

        [SerializeField] protected float m_MaxVertical = -1;
        public float maxVertical { get { return m_MaxVertical; } set { if (m_MaxVertical != value) { m_MaxVertical = value; SetDirty(); } } }

        
        // 同步UI高度相关

        [SerializeField] protected bool m_SyncUIHeight = false;
        public bool syncUIHeight { get { return m_SyncUIHeight; } set { if (m_SyncUIHeight != value) { m_SyncUIHeight = value; SetDirty(); } } }

        [SerializeField] protected RectTransform m_SyncUI = null;
        public RectTransform syncUI { get { return m_SyncUI; } set { if (m_SyncUI != value) { m_SyncUI = value; SetDirty(); } } }

        [SerializeField] protected float m_SyncMaxHeight = 100;
        public float syncMaxHeight { get { return m_SyncMaxHeight; } set { if (m_SyncMaxHeight != value) { m_SyncMaxHeight = value; SetDirty(); } } }

        [SerializeField] protected bool m_DisableScrollRect = false;
        public bool disableScrollRect { get { return m_DisableScrollRect; } set { if (m_DisableScrollRect != value) { m_DisableScrollRect = value; SetDirty(); } } }

        [SerializeField] protected ScrollRect m_ScrollRect = null;
        public ScrollRect scrollRect { get { return m_ScrollRect; } set { if (m_ScrollRect != value) { m_ScrollRect = value; SetDirty(); } } }

        [SerializeField] protected MyScrollRect m_MyScrollRect = null;
        public MyScrollRect myScrollRect { get { return m_MyScrollRect; } set { if (m_MyScrollRect != value) { m_MyScrollRect = value; SetDirty(); } } }



        private float preferredHeight = 0;
        public float PreferredHeight
        {
            get
            {
                return preferredHeight;
            }
        }

 

        [System.NonSerialized] private RectTransform m_Rect;
        private RectTransform rectTransform
        {
            get
            {
                if (m_Rect == null)
                    m_Rect = GetComponent<RectTransform>();
                return m_Rect;
            }
        }

        private DrivenRectTransformTracker m_Tracker;

        protected UISizeFitter()
        { }

        protected override void OnEnable()
        {
            base.OnEnable();
            SetDirty();
        }

        protected override void OnDisable()
        {
            m_Tracker.Clear();
            LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
            base.OnDisable();
        }

        protected override void OnRectTransformDimensionsChange()
        {
            SetDirty();
        }

        private void HandleSelfFittingAlongAxis(int axis)
        {

            FitMode fitting = (axis == 0 ? horizontalFit : verticalFit);

            if (fitting != FitMode.CustomLimit)
            {

                if (fitting == FitMode.Unconstrained)
                {
                    // Keep a reference to the tracked transform, but don't control its properties:
                    m_Tracker.Add(this, rectTransform, DrivenTransformProperties.None);
                    return;
                }

                m_Tracker.Add(this, rectTransform, (axis == 0 ? DrivenTransformProperties.SizeDeltaX : DrivenTransformProperties.SizeDeltaY));

                // Set size to min or preferred size
                if (fitting == FitMode.MinSize)
                    rectTransform.SetSizeWithCurrentAnchors((RectTransform.Axis)axis, LayoutUtility.GetMinSize(m_Rect, axis));
                else
                    rectTransform.SetSizeWithCurrentAnchors((RectTransform.Axis)axis, LayoutUtility.GetPreferredSize(m_Rect, axis));


                if (syncUIHeight && axis == 1) // 目前只处理垂直方向高度的拉长，水平有需要再改
                {
                    if(syncUI != null)
                    {
                        float size = LayoutUtility.GetPreferredSize(m_Rect, axis);
                        bool needDisableScroll = (size <= syncMaxHeight);
                        float syncSize = Mathf.Min(syncMaxHeight, size);
                        syncUI.SetSizeWithCurrentAnchors((RectTransform.Axis)axis, syncSize);
                        if (disableScrollRect)
                        {
                            if (needDisableScroll)
                            {
                                if(scrollRect != null)
                                {
                                    scrollRect.vertical = false;
                                }
                                if(myScrollRect != null)
                                {
                                    myScrollRect.vertical = false;
                                }
                            }
                            else
                            {
                                if (scrollRect != null)
                                {
                                    scrollRect.vertical = true;
                                }
                                if (myScrollRect != null)
                                {
                                    myScrollRect.vertical = true;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                m_Tracker.Add(this, rectTransform, (axis == 0 ? DrivenTransformProperties.SizeDeltaX : DrivenTransformProperties.SizeDeltaY));
                float min = (axis == 0 ? m_MaxHorizontal : m_MaxVertical);
                float size = LayoutUtility.GetPreferredSize(m_Rect, axis);
                preferredHeight = size;
                if (min >= 0) { size = Mathf.Min(size, min); }
                rectTransform.SetSizeWithCurrentAnchors((RectTransform.Axis)axis, size);
            }

        }
        
        public virtual void SetLayoutHorizontal()
        {
            m_Tracker.Clear();
            HandleSelfFittingAlongAxis(0);
        }

        public virtual void SetLayoutVertical()
        {
            HandleSelfFittingAlongAxis(1);
        }

        protected void SetDirty()
        {
            if (!IsActive())
                return;
            LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            SetDirty();
        }

#endif

        private bool SetStruct<T>(ref T currentValue, T newValue) where T : struct
        {
            if (EqualityComparer<T>.Default.Equals(currentValue, newValue))
                return false;

            currentValue = newValue;
            return true;
        }

    }
}

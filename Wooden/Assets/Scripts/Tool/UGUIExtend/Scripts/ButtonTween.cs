using UnityEngine;
using UnityEngine.EventSystems;

namespace Tool.UGUIExtend.Scripts
{
    [ExecuteAlways]
    public class ButtonTween : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, ISerializationCallbackReceiver
    {
        public Vector3 initScale = new Vector3(1f, 1f, 1f);
        public Transform target;
        Vector3 pressed = new Vector3(0.92f, 0.92f, 0.92f);
        float duration = 0.1f;

        float timer;
        Transform tweenTransform;
        Vector3 mFromScale;
        Vector3 mToScale;
        bool bIsTweening;
        bool bIsPressing;
        
#if UNITY_EDITOR
        private void Reset()
        {
            if (!Application.isPlaying)
            {
                RectTransform rectTran = GetComponent<RectTransform>();
                if (rectTran && rectTran.pivot != new Vector2(0.5f, 0.5f))
                {
                    UnityEditor.EditorUtility.DisplayDialog("提示", "ButtonTween节点RectTransform的pivot必须为0.5, 0.5", "ok");
                    DestroyImmediate(this);
                }
            }
        }
#endif
        
        void Awake()
        {
            if (target == null)
            {
                tweenTransform = transform;
            }
            else
            {
                tweenTransform = target;
            }
        }
        
        void OnDisable ()
        {
            if (tweenTransform) tweenTransform.localScale = initScale;
            bIsTweening = false;
            bIsPressing = false;
        }

        //在lateUpdate里面做tween可以覆盖Animator的缩放
        private void LateUpdate()
        {
            if (!bIsTweening) return;
            timer += Time.deltaTime;
            float value = timer / duration;
            if (value >= 1)
            {
                value = 1;
                if (!bIsPressing) bIsTweening = false;
            }
            tweenTransform.localScale = Vector3.LerpUnclamped(mFromScale, mToScale, value);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!tweenTransform) return;
            bIsTweening = true;
            bIsPressing = true;
            timer = 0;
            mFromScale = tweenTransform.localScale;
            mToScale.x = initScale.x * pressed.x;
            mToScale.y = initScale.y * pressed.y;
            mToScale.z = initScale.z * pressed.z;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!tweenTransform) return;
            bIsPressing = false;
            timer = 0;
            mFromScale = tweenTransform.localScale;
            mToScale = initScale;
        }

        public void OnBeforeSerialize()
        {
#if UNITY_EDITOR
            // 处理unity编辑器下的复制操作导致unity崩溃问题
            Event e = Event.current;
            if (e != null && e.type == EventType.ExecuteCommand)
            {
                if (e.commandName == "Paste" || e.commandName == "Duplicate")
                {
                    return;
                }
            }
#endif
            if (target)
            {
                initScale = target.localScale;
            }
            else
            {
                initScale = transform.localScale;
            }
        }

        public void OnAfterDeserialize()
        {
            
        }
    }
}
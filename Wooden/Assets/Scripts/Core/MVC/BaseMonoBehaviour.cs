using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BaseMonoBehaviour : MonoBehaviour
{
    protected virtual void Awake()
    {
    }
    public void AddClickEvent(GameObject obj,Action clickEvent)
    {
        // 为Image所在的游戏对象添加EventTrigger组件
        EventTrigger eventTrigger = gameObject.AddComponent<EventTrigger>();
        // 创建一个新的事件入口
        EventTrigger.Entry entry = new EventTrigger.Entry();
        // 设置事件类型为点击（PointerClick）
        entry.eventID = EventTriggerType.PointerClick;
        // 添加事件处理函数
        entry.callback = new EventTrigger.TriggerEvent();
        entry.callback.AddListener((data) => {
            clickEvent();
        });
        // 将事件入口添加到EventTrigger组件中
        eventTrigger.triggers.Add(entry);
    }
}


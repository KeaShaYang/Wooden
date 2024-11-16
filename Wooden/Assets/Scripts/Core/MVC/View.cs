using UnityEngine;
using System;
using System.Collections.Generic;

public abstract class View : BaseMonoBehaviour
{
    protected Model _bindModel;
    Dictionary<string, Notifier.StandardDelegate> m_AllFun = new Dictionary<string, Notifier.StandardDelegate>();
    public bool V_Isloaded = false;
    public string V_PrefabPath;
    public virtual void Init(Model model)
    {
        _bindModel = model;
    }
    protected void BindModel(Enum Attribute, Notifier.StandardDelegate fun)
    {
        if (null != _bindModel)
        {
            string keyName = Attribute.ToString();
            if (m_AllFun.ContainsKey(keyName))
            {
#if UNITY_EDITOR
                if (m_AllFun[keyName] != fun)
                    Debug.LogError("在同一个model上监听了同一事件：" + keyName);
#endif
                _bindModel.RemoveEventHandler(keyName, m_AllFun[keyName]);
                m_AllFun.Remove(keyName);
            }
            _bindModel.AddEventHandler(keyName, fun);
            m_AllFun.Add(keyName, fun);
        }
    }
    protected void UnBindModel(Enum Attribute, Notifier.StandardDelegate fun)
    {
        if (null != _bindModel)
        {
            string keyName = Attribute.ToString();
            if (m_AllFun.ContainsKey(keyName))
            {
                _bindModel.RemoveEventHandler(keyName, m_AllFun[keyName]);
                if (!m_AllFun.Remove(keyName))
                {
                    Debug.Log("回调不存在，无法移除");
                }
            }

        }
    }
    protected virtual void UnBindModel(Enum Attr)
    {
        if (null == _bindModel)
        {
            return;
        }
        _bindModel.RemoveAllEventHandler(Attr.ToString());
    }
    public virtual void F_Reset()
    {
        ClearModelAndBind();
    }
    protected override void Awake()
    {
        base.Awake();
    }
    protected virtual void OnDestroy()
    {
        ClearModelAndBind();
    }
    private void ClearModelAndBind()
    {
        if (null != _bindModel)
        {
            var iter = m_AllFun.GetEnumerator();
            while (iter.MoveNext())
            {
                _bindModel.RemoveEventHandler(iter.Current.Key, iter.Current.Value);
                iter.Dispose();
            }
            _bindModel = null;
        }
    }
}


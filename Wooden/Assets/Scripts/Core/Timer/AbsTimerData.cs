using System;

internal abstract class AbsTimerData
{
    private uint m_nTimerId;
    public int Time;
    public int CalledTime = 0;
    public uint NTimerId
    {
        get { return m_nTimerId; }
        set { m_nTimerId = value; }
    }
    private int m_nInterval;
    public int NInterval
    {
        get { return m_nInterval; }
        set { m_nInterval = value; }
    }

    private ulong m_unNextTick;
    public ulong UnNextTick
    {
        get { return m_unNextTick; }
        set { m_unNextTick = value; }
    }
    public abstract Delegate Action
    {
        get;
        set;
    }
    public virtual void DoAction()
    {
        CalledTime++;
    }

}
internal class TimerData : AbsTimerData
{
    private Action m_action;
    public override Delegate Action
    {
        get { return m_action; }
        set { m_action = value as Action; }
    }
    public override void DoAction()
    {
        if (this != null && m_action != null)
        {
            m_action();
            base.DoAction();
        }

    }
}
internal class TimerData<T> : AbsTimerData
{
    private Action<T> m_action;
    public override Delegate Action
    {
        get { return m_action; }
        set { m_action = value as Action<T>; }
    }
    private T m_arg1;
    public T Arg1
    {
        get { return m_arg1; }
        set { m_arg1 = value; }
    }

    public override void DoAction()
    {
        if (this != null && m_action != null)
        {
            m_action(m_arg1);
            base.DoAction();
        }

    }
}


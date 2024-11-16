using System;
using System.Collections.Generic;
using System.Diagnostics;

public class TimerHeap
{
    private static uint m_nNextTimerId;
    private static uint m_unTick;
    private static Stopwatch m_stopWatch;
    private static Dictionary<uint, AbsTimerData> m_queueDic;
    private static List<AbsTimerData> m_queue;
    private static readonly object m_queueLock = new object();
    private TimerHeap() { }
    static TimerHeap()
    {
        m_queueDic = new Dictionary<uint, AbsTimerData>();
        m_queue = new List<AbsTimerData>();
        m_stopWatch = new Stopwatch();
    }
    public static uint AddTimer(uint start, int interval, Action handler, int time = 0)
    {
        if ((interval == 0 && start == 0) || (start == 0 && time == 1))
        {
            handler();
            return 0;
        }
        TimerData p = GetTimerData(new TimerData(), start, interval);
        p.Action = handler;
        p.Time = time;
        return AddTimer(p);
    }
    public static uint AddTimer<T>(uint start, int interval, Action<T> handler, T arg1, int time = 0)
    {
        if ((interval == 0 && start == 0) || (start == 0 && time == 1))
        {
            handler(arg1);
            return 0;
        }
        TimerData<T> p = GetTimerData(new TimerData<T>(), start, interval);
        p.Action = handler;
        p.Arg1 = arg1;
        p.Time = time;
        return AddTimer(p);
    }
    public static void F_DelTimer(uint timerId)
    {
        lock (m_queueLock)
        {

            if (m_queueDic.ContainsKey(timerId))
            {
                m_queue.Remove(m_queueDic[timerId]);
                m_queueDic.Remove(timerId);
            }
        }
    }
    public static void Tick()
    {
        m_unTick += (uint)m_stopWatch.ElapsedMilliseconds;
        m_stopWatch.Restart();
        m_stopWatch.Start();
        lock (m_stopWatch)
        {
            int index = 0;
            int OneCount = m_queue.Count;
            while (m_queue.Count != 0)
            {
                AbsTimerData p = m_queue[index];
                if (m_unTick < p.UnNextTick)
                {
                    break;
                }
                m_queueDic.Remove(p.NTimerId);
                m_queue.RemoveAt(index);
                if (p.NInterval > 0 && (p.Time <= 0 || p.CalledTime < p.Time - 1))
                {
                    UnityEngine.Debug.Log(string.Format("timernTimerId {0}, NInterval={1} , p.Time = {2} ", p.NTimerId, p.NInterval, p.Time));
                    p.UnNextTick += (ulong)p.NInterval;
                    p.DoAction();
                    m_queueDic.Add(p.NTimerId, p);
                    m_queue.Add(p);
                }
                else
                {
                    p.DoAction();

                }
            }
        }
    }
    public static void Reset()
    {
        m_unTick = 0;
        m_nNextTimerId = 0;
        lock (m_queueLock)
            while (m_queue.Count > 0)
            {
                AbsTimerData p = m_queue[m_queue.Count - 1];
                m_queueDic.Add(p.NTimerId, p);
                m_queue.RemoveAt(m_queue.Count - 1);
            }
    }
    private static uint AddTimer(AbsTimerData p)
    {
        lock (m_queueLock)
        {
            m_queue.Add(p);
            m_queueDic.Add(p.NTimerId, p);
        }
        return p.NTimerId;
    }
    private static T GetTimerData<T>(T p, uint start, int interval) where T : AbsTimerData
    {
        p.NInterval = interval;
        p.NTimerId = ++m_nNextTimerId;
        p.UnNextTick = m_unTick + 1 + start;
        return p;
    }
}


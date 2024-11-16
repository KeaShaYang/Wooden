using System;
using UnityEngine;

public class Debugger
{
    private static bool m_showLog = true;
    public static void Log(object msg, bool forceLog = false)
    {
        if (m_showLog || forceLog)
            Debug.Log(msg);
    }
    public static void Log(string msg, bool forceLog = false)
    {
        if (m_showLog || forceLog)
            Debug.Log(msg);
    }
    public static void LogError(object msg, bool forceLog = false)
    {
        if (m_showLog || forceLog)
            Debug.LogError(msg);
    }
    public static void LogError(string msg, bool forceLog = false)
    {
        if (m_showLog || forceLog)
            Debug.LogError(msg);
    }
    public static void LogWarning(object msg,  bool forceLog = false)
    {
        if (m_showLog || forceLog)
            Debug.LogWarning(msg);
    }
    public static void LogWarning(string msg,  bool forceLog = false)
    {
        if (m_showLog || forceLog)
            Debug.LogWarning(msg);
    }
    public static void LogException(Exception exception, bool forceLog = false)
    {
        if (m_showLog || forceLog)
            Debug.LogException(exception);
    }
}

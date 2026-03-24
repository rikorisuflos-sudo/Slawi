using System.Diagnostics;
using UnityEngine;
/// <summary>
/// デバッグログ用クラス
/// インスタンス化したいな～
/// </summary>
public static class DebugLogger
{
    [Conditional("UNITY_EDITOR")]
    public static void Log(object o)
    {
        UnityEngine.Debug.Log(o);
    }
}


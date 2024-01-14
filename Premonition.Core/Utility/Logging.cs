using System.Collections.Generic;
using JetBrains.Annotations;

namespace Premonition.Core.Utility;

/// <summary>
/// Contains logging methods for Premonition
/// </summary>
[PublicAPI]
public static class Logging
{

    /// <summary>
    /// All the active log listeners
    /// </summary>
    [PublicAPI]
    public static readonly List<ILogListener> Listeners = [];
    
    /// <summary>
    /// Log something as debug
    /// </summary>
    /// <param name="value">The value being logged</param>
    public static void LogDebug(object value)
    {
        foreach (var listener in Listeners)
        {
            listener.LogDebug(value);
        }
    }

    /// <summary>
    /// Log something as info
    /// </summary>
    /// <param name="value">The value being logged</param>
    public static void LogInfo(object value)
    {
        foreach (var listener in Listeners)
        {
            listener.LogInfo(value);
        }
    }

    /// <summary>
    /// Log something as warning
    /// </summary>
    /// <param name="value">The value being logged</param>
    public static void LogWarning(object value)
    {
        foreach (var listener in Listeners)
        {
            listener.LogWarning(value);
        }
    }

    /// <summary>
    /// Log something as error
    /// </summary>
    /// <param name="value">The value being logged</param>
    public static void LogError(object value)
    {
        
        foreach (var listener in Listeners)
        {
            listener.LogError(value);
        }
    }
}
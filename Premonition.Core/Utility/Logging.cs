using JetBrains.Annotations;

namespace Premonition.Core.Utility;

[PublicAPI]
public static class Logging
{

    [PublicAPI]
    public static readonly List<ILogListener> Listeners = [];
    
    public static void LogDebug(object value)
    {
        foreach (var listener in Listeners)
        {
            listener.LogDebug(value);
        }
    }

    public static void LogInfo(object value)
    {
        foreach (var listener in Listeners)
        {
            listener.LogInfo(value);
        }
    }

    public static void LogWarning(object value)
    {
        foreach (var listener in Listeners)
        {
            listener.LogWarning(value);
        }
    }

    public static void LogError(object value)
    {
        
        foreach (var listener in Listeners)
        {
            listener.LogError(value);
        }
    }
}
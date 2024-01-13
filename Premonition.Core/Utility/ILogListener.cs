namespace Premonition.Core.Utility;

public interface ILogListener
{
    public void LogDebug(object value);
    public void LogInfo(object value);
    public void LogWarning(object value);
    public void LogError(object value);

    public static ILogListener CreateListener(Action<object> logDebug,
        Action<object> logInfo,
        Action<object> logWarning,
        Action<object> logError) => new DynamicLogListener(logDebug, logInfo, logWarning, logError);
    
    internal class DynamicLogListener(
        Action<object> logDebugAction,
        Action<object> logInfoAction,
        Action<object> logWarningAction,
        Action<object> logErrorAction) : ILogListener
    {
        public void LogDebug(object value) => logDebugAction(value);

        public void LogInfo(object value) => logInfoAction(value);

        public void LogWarning(object value) => logWarningAction(value);

        public void LogError(object value) => logErrorAction(value);
    }
}
using System;

namespace Premonition.Core.Utility;

/// <summary>
/// A log listener for premonitions logging
/// </summary>
public interface ILogListener
{
    /// <summary>
    /// Called when a debug log is being sent
    /// </summary>
    /// <param name="value">The object being logged</param>
    public void LogDebug(object value);
    /// <summary>
    /// Called when an info log is being sent
    /// </summary>
    /// <param name="value">The object being logged</param>
    public void LogInfo(object value);
    /// <summary>
    /// Called when a warning is being sent
    /// </summary>
    /// <param name="value">The object being logged</param>
    public void LogWarning(object value);
    /// <summary>
    /// Called when an error is being sent
    /// </summary>
    /// <param name="value">The object being logged</param>
    public void LogError(object value);

    /// <summary>
    /// Creates a log listener from delegates
    /// </summary>
    /// <param name="logDebug">The debug logging delegate</param>
    /// <param name="logInfo">The info logging delegate</param>
    /// <param name="logWarning">The warning logging delegate</param>
    /// <param name="logError">The error logging delegate</param>
    /// <returns>A log listener with the interface filled in with calls to those delegates</returns>
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
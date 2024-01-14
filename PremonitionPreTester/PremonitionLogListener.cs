using BepInEx.Logging;
using Premonition.Core.Utility;
using ILogListener = Premonition.Core.Utility.ILogListener;

namespace PremonitionTesters;

/// <summary>
/// The log listener used for testing
/// </summary>
public sealed class PremonitionLogListener : ILogListener
{
    /// <inheritdoc />
    public void LogDebug(object value)
    {
        Console.WriteLine($"[{DateTime.Now}] [DEBUG] {value}");
    }

    /// <inheritdoc />
    public void LogInfo(object value)
    {
        Console.WriteLine($"[{DateTime.Now}] [ INFO] {value}");
    }

    /// <inheritdoc />
    public void LogWarning(object value)
    {
        Console.WriteLine($"[{DateTime.Now}] [ WARN] {value}");
    }

    /// <inheritdoc />
    public void LogError(object value)
    {
        Console.WriteLine($"[{DateTime.Now}] [ERROR] {value}");
    }
}
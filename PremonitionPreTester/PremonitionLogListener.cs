using BepInEx.Logging;
using Premonition.Core.Utility;
using ILogListener = Premonition.Core.Utility.ILogListener;

namespace PremonitionTesters;

public sealed class PremonitionLogListener : ILogListener
{
    public void Dispose()
    {
    }

    public void LogEvent(object sender, LogEventArgs eventArgs)
    {
        Console.WriteLine($"[{DateTime.Now}] {eventArgs}");
    }


    
    public void LogDebug(object value)
    {
        Console.WriteLine($"[{DateTime.Now}] [DEBUG] {value}");
    }

    public void LogInfo(object value)
    {
        Console.WriteLine($"[{DateTime.Now}] [ INFO] {value}");
    }

    public void LogWarning(object value)
    {
        Console.WriteLine($"[{DateTime.Now}] [ WARN] {value}");
    }

    public void LogError(object value)
    {
        Console.WriteLine($"[{DateTime.Now}] [ERROR] {value}");
    }
}
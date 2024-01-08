using BepInEx.Logging;

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
}
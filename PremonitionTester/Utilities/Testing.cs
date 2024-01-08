using System.Diagnostics;
using JetBrains.Annotations;
using PremonitionTester.Utilities.Exceptions;

namespace PremonitionTester.Utilities;

[PublicAPI]
public static class Testing
{
    internal static List<string> TestLogOutput = []; // This will be reset before every test

    internal static void ResetLog()
    {
        TestLogOutput = [];
    }
    
    public static void Log(object obj)
    {
        var str = obj.ToString();
        var lines = str!.Split('\n');
        TestLogOutput.AddRange(lines);
    }


    public static string GetCallLocation()
    {
        var stackTrace = new StackTrace(true);
        StackFrame? frame;
        // var frame = stackTrace.GetFrame(2)!;
        // frame.GetMethod().DeclaringType;
        var index = 0;
        while ((frame = stackTrace.GetFrame(index)) is not null && frame.GetMethod()!.DeclaringType == typeof(Testing))
        {
            index += 1;
        }
        if (frame == null) return "unknown:0:0";
        
        var filename = new FileInfo(frame.GetFileName()!).Name;
        var line = frame.GetFileLineNumber();
        var column = frame.GetFileColumnNumber();
        return $"{filename}:{line}:{column}";
    }

    public static void Pass()
    {
        throw new TestPassedException();
    }

    public static void Fail(string reason)
    {
        throw new TestFailedException($"{GetCallLocation()}: {reason}");
    }

    public static void Skip(string reason)
    {
        throw new TestSkippedException($"{GetCallLocation()}: {reason}");
    }

    public static void Assert(bool value, string message)
    {
        if (!value)
        {
            Fail($"assertion failed: {message}");
        }
    }

    public static void AssertEqual<T>(T a, T b) => Assert(a != null && a.Equals(b), $"{a} != {b}");
}
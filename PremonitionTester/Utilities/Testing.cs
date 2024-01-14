using System.Diagnostics;
using JetBrains.Annotations;
using PremonitionTester.Utilities.Exceptions;

namespace PremonitionTester.Utilities;

/// <summary>
/// Contains methods used for testing
/// </summary>
[PublicAPI]
public static class Testing
{
    internal static List<string> TestLogOutput = []; // This will be reset before every test

    internal static void ResetLog()
    {
        TestLogOutput = [];
    }
    
    /// <summary>
    /// Log a value from your test
    /// </summary>
    /// <param name="obj">The value being logged</param>
    public static void Log(object obj)
    {
        var str = obj.ToString();
        var lines = str!.Split('\n');
        TestLogOutput.AddRange(lines);
    }


    private static string GetCallLocation()
    {
        var stackTrace = new StackTrace(true);
        StackFrame? frame;
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

    /// <summary>
    /// Declare this test passed
    /// </summary>
    public static void Pass()
    {
        throw new TestPassedException();
    }

    /// <summary>
    /// Declare this test failed
    /// </summary>
    /// <param name="reason">The reason of failure</param>
    public static void Fail(string reason)
    {
        throw new TestFailedException($"{GetCallLocation()}: {reason}");
    }

    /// <summary>
    /// Declare this test skipped
    /// </summary>
    /// <param name="reason">The reason for being skipped</param>
    public static void Skip(string reason)
    {
        throw new TestSkippedException($"{GetCallLocation()}: {reason}");
    }

    /// <summary>
    /// Assert that value is true, otherwise fail
    /// </summary>
    /// <param name="value">The value to assert</param>
    /// <param name="message">The message when failed</param>
    public static void Assert(bool value, string message)
    {
        if (!value)
        {
            Fail($"assertion failed: {message}");
        }
    }

    /// <summary>
    /// Assert that 2 values are equal, otherwise fail
    /// </summary>
    /// <param name="a">The first value</param>
    /// <param name="b">The second value</param>
    public static void AssertEqual<T>(T a, T b) => Assert(a != null && a.Equals(b), $"{a} != {b}");
}
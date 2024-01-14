using JetBrains.Annotations;

namespace PremonitionTester.Utilities.Exceptions;

/// <summary>
/// Thrown when a test is failed
/// </summary>
/// <param name="reason">The reason for failure</param>
[PublicAPI]
public class TestFailedException(string reason) : Exception(reason);
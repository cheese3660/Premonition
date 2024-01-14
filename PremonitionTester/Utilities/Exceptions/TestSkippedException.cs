using JetBrains.Annotations;

namespace PremonitionTester.Utilities.Exceptions;

/// <summary>
/// Thrown when a test is skipped
/// </summary>
/// <param name="reason">The reason for skipping</param>
[PublicAPI]
public class TestSkippedException(string reason) : Exception(reason);
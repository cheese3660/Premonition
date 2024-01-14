using JetBrains.Annotations;

namespace PremonitionTester.Utilities.Exceptions;

/// <summary>
/// Thrown when a test is passed
/// </summary>
[PublicAPI]
public class TestPassedException : Exception;
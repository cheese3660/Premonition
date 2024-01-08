using JetBrains.Annotations;

namespace PremonitionTester.Utilities.Exceptions;

[PublicAPI]
public class TestFailedException(string reason) : Exception(reason);
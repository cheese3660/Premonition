using JetBrains.Annotations;

namespace PremonitionTester.Utilities.Exceptions;

[PublicAPI]
public class TestSkippedException(string reason) : Exception(reason);
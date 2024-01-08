using DummyGame;

namespace PremonitionTester;

public static class Tester
{
    public static bool RunTests()
    {
        StaticTests.StaticVoidTestOne();
        Console.WriteLine($"{StaticTests.StaticVoidTestOneDummyValue} - should be 2");
        return true;
    }
}
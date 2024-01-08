namespace TesterDLL;


using DummyGame;

public static class TestRunner
{
    public static bool RunTests()
    {
        
        StaticTests.StaticVoidTestOne();
        Console.WriteLine(StaticTests.StaticVoidTestOneDummyValue);
        return true;
    }
}
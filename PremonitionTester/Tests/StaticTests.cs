using DummyGame;
using PremonitionTester.Attributes;
using PremonitionTester.Utilities;

namespace PremonitionTester.Tests;

[TestSection("Static Tests")]
public static class StaticTests
{
    [Test("StaticVoidMethodOne() Trampoline")]
    public static void StaticVoidMethodOneTest()
    {
        StaticMethods.StaticVoidMethodOne();
        Testing.Log("Testing that StaticVoidMethodOneDummyValue is 2");
        Testing.AssertEqual(StaticMethods.StaticVoidMethodOneDummyValue, 2);
    }

    [Test("StaticVoidMethodTwo() Prefix")]
    public static void StaticVoidMethodTwoTest()
    {
        StaticMethods.StaticVoidMethodTwo();
        Testing.Log("Testing that StaticVoidMethodTwoDummyValue is 10");
        Testing.AssertEqual(StaticMethods.StaticVoidMethodTwoDummyValue, 10);
    }
    
    [Test("StaticVoidMethodThree() Postfix")]
    public static void StaticVoidMethodThree()
    {
        StaticMethods.StaticVoidMethodThree();
        Testing.Log("Testing that StaticVoidMethodThreeDummyValue is 16");
        Testing.AssertEqual(StaticMethods.StaticVoidMethodThreeDummyValue, 16);
    }
}
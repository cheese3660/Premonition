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

    [Test("MultipleReturn() Postfix")]
    public static void MultipleReturn()
    {
        Testing.Log("MultipleReturn(-1)");
        StaticMethods.MultipleReturn(-1);
        Testing.Log("Testing that MultipleReturnDummyValue is 6");
        Testing.AssertEqual(StaticMethods.MultipleReturnDummyValue, 6);
        Testing.Log("MultipleReturn(0)");
        StaticMethods.MultipleReturn(0);
        Testing.Log("Testing that MultipleReturnDummyValue is 11");
        Testing.AssertEqual(StaticMethods.MultipleReturnDummyValue, 11);
        Testing.Log("MultipleReturn(1)");
        StaticMethods.MultipleReturn(1);
        Testing.Log("Testing that MultipleReturnDummyValue is 16");
        Testing.AssertEqual(StaticMethods.MultipleReturnDummyValue, 16);
    }

    [Test("Generic() Trampoline")]
    public static void Generic()
    {
        Testing.Log("Testing that Generic([1,2,3],0) returns 2");
        Testing.AssertEqual(StaticMethods.Generic([1, 2, 3], 0), 2);
        Testing.Log("Testing that Generic([1,2,3],1) returns 3");
        Testing.AssertEqual(StaticMethods.Generic([1, 2, 3], 1), 3);
        Testing.Log("Testing that Generic([\"Hi\",\"Bye\",\"???\"],0) returns \"Bye\"");
        Testing.AssertEqual(StaticMethods.Generic(["Hi","Bye","???"], 0), "Bye");
        Testing.Log("Testing that Generic([\"Hi\",\"Bye\",\"???\"],1) returns \"???\"");
        Testing.AssertEqual(StaticMethods.Generic(["Hi","Bye","???"], 1), "???");
    }
}
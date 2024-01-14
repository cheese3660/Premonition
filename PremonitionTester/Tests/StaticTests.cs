using DummyGame;
using PremonitionTester.Attributes;
using PremonitionTester.Utilities;

namespace PremonitionTester.Tests;

/// <summary>
/// A list of tests for DummyGame.StaticMethods
/// </summary>
[TestSection("Static Tests")]
public static class StaticTests
{
    /// <summary>
    /// Tests whether or not the trampoline for StaticVoidMethodOne worked
    /// </summary>
    [Test("StaticVoidMethodOne() Trampoline")]
    public static void StaticVoidMethodOne()
    {
        StaticMethods.StaticVoidMethodOne();
        Testing.Log("Testing that StaticVoidMethodOneDummyValue is 2");
        Testing.AssertEqual(StaticMethods.StaticVoidMethodOneDummyValue, 2);
    }

    /// <summary>
    /// Tests whether or not the prefix for StaticVoidMethodTwo worked
    /// </summary>
    [Test("StaticVoidMethodTwo() Prefix")]
    public static void StaticVoidMethodTwo()
    {
        StaticMethods.StaticVoidMethodTwo();
        Testing.Log("Testing that StaticVoidMethodTwoDummyValue is 10");
        Testing.AssertEqual(StaticMethods.StaticVoidMethodTwoDummyValue, 10);
    }
    
    
    /// <summary>
    /// Tests whether or not the postfix for StaticVoidMethodThree worked
    /// </summary>
    [Test("StaticVoidMethodThree() Postfix")]
    public static void StaticVoidMethodThree()
    {
        StaticMethods.StaticVoidMethodThree();
        Testing.Log("Testing that StaticVoidMethodThreeDummyValue is 16");
        Testing.AssertEqual(StaticMethods.StaticVoidMethodThreeDummyValue, 16);
    }

    /// <summary>
    /// Tests whether or not the postfix for MultipleReturn worked
    /// </summary>
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

    
    /// <summary>
    /// Tests whether or not the trampoline for Generic worked
    /// </summary>
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
        Testing.Log("Testing that Generic([(0,1,2),(3,4,5),(6,7,8)],0) returns (3,4,5)");
        Testing.AssertEqual(StaticMethods.Generic([(0,1,2),(3,4,5),(6,7,8)],0),(3,4,5));
        Testing.Log("Testing that Generic([(0,1,2),(3,4,5),(6,7,8)],1) returns (6,7,8)");
        Testing.AssertEqual(StaticMethods.Generic([(0,1,2),(3,4,5),(6,7,8)],1),(6,7,8));
    }
    
    /// <summary>
    /// Tests whether or not the postfix for GenericValue worked
    /// </summary>
    [Test("GenericValue() Trampoline")]
    public static void GenericValue()
    {
        Testing.Log("Testing that GenericValue(0) returns 0");
        Testing.AssertEqual(StaticMethods.GenericValue(0),0);
        Testing.Log("Testing that GenericValue((0,1,2)) returns (0,1,2)");
        Testing.AssertEqual(StaticMethods.GenericValue((0,1,2)),(0,1,2));
    }
    
    
    /// <summary>
    /// Tests whether or not the postfix for ReturnsDoubleInput worked
    /// </summary>
    [Test("ReturnsDoubleInput() Postfix")]
    public static void ReturnsDoubleInput()
    {
        Testing.Log("Testing that ReturnsDoubleInput(2) returns 8");
        Testing.AssertEqual(StaticMethods.ReturnsDoubleInput(2), 8);
        Testing.Log("Testing that ReturnsDoubleInput(4) returns 16");
        Testing.AssertEqual(StaticMethods.ReturnsDoubleInput(4), 16);
    }
    
    
    /// <summary>
    /// Tests whether or not the prefix for ReturnsTripleInput worked
    /// </summary>
    [Test("ReturnsTripleInput() Prefix")]
    public static void ReturnsTripleInput()
    {
        Testing.Log("Testing that ReturnsTripleInput(2) returns 0");
        Testing.AssertEqual(StaticMethods.ReturnsTripleInput(2), 0);
        Testing.Log("Testing that ReturnsTripleInput(4) returns 12");
        Testing.AssertEqual(StaticMethods.ReturnsTripleInput(4), 12);
    }
}
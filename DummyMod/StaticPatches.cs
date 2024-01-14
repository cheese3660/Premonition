using System;
using System.Collections.Generic;
using Premonition.Core.Attributes;
using DummyGame;
namespace DummyMod;


/// <summary>
/// A list of patches to be used for testing premonition
/// </summary>
[PremonitionAssembly("DummyGame")]
[PremonitionType("DummyGame.StaticMethods")]
public class StaticPatches
{
    /// <summary>
    /// Patches DummyGame.StaticMethods.StaticVoidMethodOne to set the dummy value to 2
    /// </summary>
    [PremonitionMethod(nameof(StaticMethods.StaticVoidMethodOne))]
    [PremonitionTrampoline]
    public static void StaticVoidMethodOne()
    {
        StaticMethods.StaticVoidMethodOneDummyValue = 2;
    }

    /// <summary>
    /// Patches DummyGame.StaticMethods.StaticVoidMethodTwo to set the dummy value to 9
    /// </summary>
    [PremonitionMethod(nameof(StaticMethods.StaticVoidMethodTwo))]
    [PremonitionPrefix]
    public static void StaticVoidMethodTwo() => StaticMethods.StaticVoidMethodTwoDummyValue = 9;

    /// <summary>
    /// Patches DummyGame.StaticMethods.StaticVoidMethodThree to double the dummy value
    /// </summary>
    [PremonitionMethod(nameof(StaticMethods.StaticVoidMethodThree))]
    [PremonitionPostfix]
    public static void StaticVoidMethodThree() => StaticMethods.StaticVoidMethodThreeDummyValue *= 2;

    /// <summary>
    /// Patches DummyGame.StaticMethods.MultipleReturn to add 1 to the dummy value
    /// </summary>
    [PremonitionMethod(nameof(StaticMethods.MultipleReturn))]
    [PremonitionPostfix]
    public static void MultipleReturn() => StaticMethods.MultipleReturnDummyValue += 1;


    /// <summary>
    /// Patches DummyGame.StaticMethods.Generic to get the next value in the list
    /// </summary>
    /// <param name="from">The list</param>
    /// <param name="index">The index into the list</param>
    /// <returns>from[index+1]</returns>
    [PremonitionMethod(nameof(StaticMethods.Generic))]
    [PremonitionTrampoline]
    public static T Generic<T>(List<T> from, int index) => from[index + 1];


    /// <summary>
    /// Patches DummyGame.StaticMethods.GenericValue to return the given value
    /// </summary>
    /// <param name="from">The given value</param>
    /// <returns>The given value</returns>
    [PremonitionMethod(nameof(StaticMethods.GenericValue))]
    [PremonitionTrampoline]
    public static T? GenericValue<T>(T from) where T : struct => from;

    /// <summary>
    /// Patches DummyGame.StaticMethods.ReturnsDoubleInput to double the input once more
    /// </summary>
    /// <param name="__retVal">The return value that ReturnsDoubleInput is going to return</param>
    /// <returns>The return value times 2</returns>
    [PremonitionMethod(nameof(StaticMethods.ReturnsDoubleInput))]
    [PremonitionPostfix]
    // ReSharper disable once InconsistentNaming
    public static int ReturnsDoubleInput(int __retVal) => __retVal * 2;

    /// <summary>
    /// Patches DummyGame.StaticMethods.ReturnsTripleInput to only return triple the input if input >= 3, and otherwise return 0 
    /// </summary>
    /// <param name="input">The given input</param>
    /// <param name="__retVal">The return value to return if the input is &lt; 3</param>
    /// <returns>True if the input is >= 3, as to continue the original method</returns>
    [PremonitionMethod(nameof(StaticMethods.ReturnsTripleInput))]
    [PremonitionPrefix]
    // ReSharper disable once InconsistentNaming
    public static bool ReturnsTripleInput(int input, out int __retVal)
    {
        __retVal = 0;
        return input >= 3;
    }
}
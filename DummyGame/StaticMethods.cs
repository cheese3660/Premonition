using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace DummyGame;

/// <summary>
/// This file contains a bunch of static methods for testing purposes of the patcher
/// </summary>
[PublicAPI]
public static class StaticMethods
{
    /// <summary>
    /// A dummy value to check if StaticVoidMethodOne() was patched successfully
    /// </summary>
    public static int StaticVoidMethodOneDummyValue;
    
    
    /// <summary>
    /// This should set StaticVoidMethodOneDummyValue to 1, if this was messed with properly, it should instead set StaticVoidMethodOneDummyValue to 2
    /// </summary>
    public static void StaticVoidMethodOne()
    {
        StaticVoidMethodOneDummyValue = 1;
    }


    /// <summary>
    /// A dummy value to check if StaticVoidMethodTwo() was patched successfully
    /// </summary>
    public static int StaticVoidMethodTwoDummyValue = 3;


    /// <summary>
    /// This should set StaticVoidMethodTwoDummyValue to 4, if this was messed with properly, it should instead set it to 10
    /// </summary>
    public static void StaticVoidMethodTwo()
    {
        StaticVoidMethodTwoDummyValue += 1;
    }
    
    /// <summary>
    /// A dummy value to check if StaticVoidMethodThree() was patched successfully
    /// </summary>
    public static int StaticVoidMethodThreeDummyValue = 4;
    
    /// <summary>
    /// This should set StaticVoidMethodTwoDummyValue to 8, if this was messed with properly, it should instead set it to 16
    /// </summary>
    public static void StaticVoidMethodThree()
    {
        StaticVoidMethodThreeDummyValue *= 2;
    }

    
    /// <summary>
    /// A dummy value to check if MultipleReturn() was patched successfully
    /// </summary>
    public static int MultipleReturnDummyValue;
    
    
    /// <summary>
    /// If patched correctly, this set MultipleReturnDummyValue to one greater than it otherwise would be set to
    /// </summary>
    /// <param name="index">An index to "get" between -1 and 1</param>
    public static void MultipleReturn(int index)
    {
        switch (index)
        {
            case -1:
                MultipleReturnDummyValue = 5;
                break;
            case 0:
                MultipleReturnDummyValue = 10;
                break;
            case 1:
                MultipleReturnDummyValue = 15;
                break;
            default:
                return;
        }
    }
    
    
    /// <summary>
    /// When patched properly should return from[index+1]
    /// </summary>
    /// <param name="from">The list</param>
    /// <param name="index">The index to get from the list</param>
    /// <typeparam name="T">The type of the list</typeparam>
    /// <returns>from[index]</returns>
    public static T Generic<T>(List<T> from, int index)
    {
        return from[index];
    }
    
    /// <summary>
    /// When patched properly should return from
    /// </summary>
    /// <param name="from">A value</param>
    /// <typeparam name="T">The type of the value</typeparam>
    /// <returns>null</returns>
    public static T? GenericValue<T>(T from) where T : struct
    {
        return null;
    }


    /// <summary>
    /// When patched properly, should return 4x input
    /// </summary>
    /// <param name="input">The input</param>
    /// <returns>Double the input</returns>
    public static int ReturnsDoubleInput(int input)
    {
        return input * 2;
    }

    /// <summary>
    /// When patched properly should return triple the input if the input is greater than 2, otherwise zero
    /// </summary>
    /// <param name="input">The input</param>
    /// <returns>Triple the input</returns>
    public static int ReturnsTripleInput(int input) => input * 3;
}
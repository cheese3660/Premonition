using System;
using JetBrains.Annotations;

namespace DummyGame;

[PublicAPI]
public static class StaticMethods
{
    public static int StaticVoidMethodOneDummyValue;
    
    
    /// <summary>
    /// This should set StaticVoidMethodOneDummyValue to 1, if this was messed with properly, it should instead set StaticVoidMethodOneDummyValue to 2
    /// </summary>
    public static void StaticVoidMethodOne()
    {
        StaticVoidMethodOneDummyValue = 1;
    }


    public static int StaticVoidMethodTwoDummyValue = 3;


    /// <summary>
    /// This should set StaticVoidMethodTwoDummyValue to 4, if this was messed with properly, it should instead set it to 10
    /// </summary>
    public static void StaticVoidMethodTwo()
    {
        StaticVoidMethodTwoDummyValue += 1;
    }

    public static int StaticVoidMethodThreeDummyValue = 4;
    
    /// <summary>
    /// This should set StaticVoidMethodTwoDummyValue to 8, if this was messed with properly, it should instead set it to 16
    /// </summary>
    public static void StaticVoidMethodThree()
    {
        StaticVoidMethodThreeDummyValue *= 2;
    }

    public static int MultipleReturnDummyValue;
    
    
    // The MultipleReturnDummyValue should be incremented by one if patched correctly
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
    
    
    // Lets do a quick generic test, when patched correctly, this should always return the index + 1;
    public static T Generic<T>(List<T> from, int index)
    {
        return from[index];
    }
    
    public static T? GenericValue<T>(T from) where T : struct
    {
        return null;
    }


    public static int ReturnsDoubleInput(int input)
    {
        return input * 2;
    }

    public static int ReturnsTripleInput(int input) => input * 3;
}
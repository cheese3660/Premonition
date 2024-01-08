using System;
using JetBrains.Annotations;

namespace DummyGame;

[PublicAPI]
public class StaticMethods
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
}
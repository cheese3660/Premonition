using System;
using Premonition.Attributes;
using DummyGame;
namespace DummyMod;


[PremonitionAssembly("DummyGame")]
[PremonitionType("DummyGame.StaticMethods")]
public class StaticPatches
{
    [PremonitionMethod(nameof(StaticMethods.StaticVoidMethodOne))]
    [PremonitionTrampoline]
    public static void StaticVoidMethodOne()
    {
        StaticMethods.StaticVoidMethodOneDummyValue = 2;
    }

    [PremonitionMethod(nameof(StaticMethods.StaticVoidMethodTwo))]
    [PremonitionPrefix]
    public static void StaticVoidMethodTwo()
    {
        StaticMethods.StaticVoidMethodTwoDummyValue = 9;
    }
    
    [PremonitionMethod(nameof(StaticMethods.StaticVoidMethodThree))]
    [PremonitionPostfix]
    public static void StaticVoidMethodThree()
    {
        StaticMethods.StaticVoidMethodThreeDummyValue *= 2;
    }

    [PremonitionMethod(nameof(StaticMethods.MultipleReturn))]
    [PremonitionPostfix]
    public static void MultipleReturn()
    {
        StaticMethods.MultipleReturnDummyValue += 1;
    }
    
    [PremonitionMethod(nameof(StaticMethods.Generic))]
    [PremonitionTrampoline]
    public static T Generic<T>(List<T> from, int index)
    {
        return from[index + 1];
    }
}
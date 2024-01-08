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
    [PremonitionPrefix]
    public static void StaticVoidMethodThree()
    {
        StaticMethods.StaticVoidMethodThreeDummyValue *= 2;
    }
}
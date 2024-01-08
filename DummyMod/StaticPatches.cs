using System;
using Premonition.Attributes;
using DummyGame;
namespace DummyMod;


[PremonitionAssembly("DummyGame")]
[PremonitionType("DummyGame.StaticTests")]
public class StaticPatches
{
    [PremonitionMethod(nameof(StaticTests.StaticVoidTestOne))]
    [PremonitionTrampoline]
    public static void StaticVoidTestOne()
    {
        StaticTests.StaticVoidTestOneDummyValue = 2;
    }
}
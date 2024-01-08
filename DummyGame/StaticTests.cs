using System;
using JetBrains.Annotations;

namespace DummyGame;

[PublicAPI]
public class StaticTests
{
    public static int StaticVoidTestOneDummyValue;
    
    
    /// <summary>
    /// This should set StaticVoidTestOneDummyValue to 1, if this was messed with properly, it should instead set StaticVoidTestOneDummyValue to 2
    /// </summary>
    public static void StaticVoidTestOne()
    {
        StaticVoidTestOneDummyValue = 1;
    }
}
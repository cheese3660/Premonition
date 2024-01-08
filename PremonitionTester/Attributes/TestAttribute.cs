using JetBrains.Annotations;

namespace PremonitionTester.Attributes;


[AttributeUsage(AttributeTargets.Method)]
[MeansImplicitUse]
public class TestAttribute(string name) : Attribute
{
    public string Name => name;
}
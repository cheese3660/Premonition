using JetBrains.Annotations;

namespace PremonitionTester.Attributes;


/// <summary>
/// Declares this method to be a test
/// </summary>
/// <param name="name">The name of the test</param>
[AttributeUsage(AttributeTargets.Method)]
[MeansImplicitUse]
public class TestAttribute(string name) : Attribute
{
    /// <summary>
    /// The name of the test
    /// </summary>
    public string Name => name;
}
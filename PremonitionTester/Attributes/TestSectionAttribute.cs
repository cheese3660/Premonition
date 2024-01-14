using JetBrains.Annotations;

namespace PremonitionTester.Attributes;

/// <summary>
/// Declares this type to be a test section
/// </summary>
/// <param name="sectionName">The name of the test section</param>
[AttributeUsage(AttributeTargets.Class)]
[MeansImplicitUse]
public class TestSectionAttribute(string sectionName) : Attribute
{
    /// <summary>
    /// The name of the section
    /// </summary>
    public string SectionName => sectionName;
}
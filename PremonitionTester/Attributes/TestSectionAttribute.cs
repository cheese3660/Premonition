using JetBrains.Annotations;

namespace PremonitionTester.Attributes;

[AttributeUsage(AttributeTargets.Class)]
[MeansImplicitUse]
public class TestSectionAttribute(string sectionName) : Attribute
{
    public string SectionName => sectionName;
}
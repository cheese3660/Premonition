using JetBrains.Annotations;
using Mono.Cecil;
using Premonition.Utility;

namespace Premonition.Attributes;

[AttributeUsage(AttributeTargets.Method)]
[MeansImplicitUse]
[PublicAPI]
public class PremonitionPrefix : Attribute
{
    internal static PremonitionPrefix? FromCecilMethod(MethodDefinition md)
    {
        var attr = MetadataHelper.GetCustomAttributes<PremonitionPrefix>(md).FirstOrDefault();
        return attr == null ? null : new PremonitionPrefix();
    }
}
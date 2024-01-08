using JetBrains.Annotations;
using Mono.Cecil;
using Premonition.Utility;

namespace Premonition.Attributes;

[AttributeUsage(AttributeTargets.Method)]
[MeansImplicitUse]
[PublicAPI]
public class PremonitionTrampoline : Attribute
{
    internal static PremonitionTrampoline? FromCecilMethod(MethodDefinition md)
    {
        var attr = MetadataHelper.GetCustomAttributes<PremonitionTrampoline>(md).FirstOrDefault();
        return attr == null ? null : new PremonitionTrampoline();
    }
}
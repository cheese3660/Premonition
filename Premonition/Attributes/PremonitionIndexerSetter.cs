using JetBrains.Annotations;
using Mono.Cecil;
using Premonition.Utility;

namespace Premonition.Attributes;

[AttributeUsage(AttributeTargets.Method)]
[MeansImplicitUse]
[PublicAPI]
public class PremonitionIndexerSetter() : PremonitionSetter("Item")
{
    internal new static PremonitionIndexerSetter? FromCecilMethod(MethodDefinition md)
    {
        var attr = MetadataHelper.GetCustomAttributes<PremonitionIndexerSetter>(md).FirstOrDefault();
        return attr == null ? null : new PremonitionIndexerSetter();
    }
}
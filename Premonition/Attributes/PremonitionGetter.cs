using JetBrains.Annotations;
using Mono.Cecil;
using Premonition.Utility;

namespace Premonition.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
[MeansImplicitUse]
[PublicAPI]
public class PremonitionGetter(string property) : PremonitionMethod($"get_{property}")
{
    internal new static PremonitionGetter? FromCecilType(TypeDefinition td)
    {
        var attr = MetadataHelper.GetCustomAttributes<PremonitionGetter>(td,false).FirstOrDefault();
        return attr == null ? null : new PremonitionGetter((string)attr.ConstructorArguments[0].Value);
    }

    internal new static PremonitionGetter? FromCecilMethod(MethodDefinition md)
    {
        var attr = MetadataHelper.GetCustomAttributes<PremonitionGetter>(md).FirstOrDefault();
        return attr == null ? null : new PremonitionGetter((string)attr.ConstructorArguments[0].Value);
    }
}
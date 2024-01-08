using JetBrains.Annotations;
using Mono.Cecil;
using Premonition.Utility;

namespace Premonition.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
[MeansImplicitUse]
[PublicAPI]
public class PremonitionMethod(string methodName) : Attribute
{
    public string MethodName => methodName;
    internal static PremonitionMethod? FromCecilType(TypeDefinition td)
    {
        var attr = MetadataHelper.GetCustomAttributes<PremonitionMethod>(td,false).FirstOrDefault();
        return attr == null ? null : new PremonitionMethod((string)attr.ConstructorArguments[0].Value);
    }

    internal static PremonitionMethod? FromCecilMethod(MethodDefinition md)
    {
        var attr = MetadataHelper.GetCustomAttributes<PremonitionMethod>(md).FirstOrDefault();
        return attr == null ? null : new PremonitionMethod((string)attr.ConstructorArguments[0].Value);
    }
}
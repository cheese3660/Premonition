using JetBrains.Annotations;
using Mono.Cecil;
using Premonition.Utility;

namespace Premonition.Attributes;


[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
[MeansImplicitUse]
[PublicAPI]
public class PremonitionType(string fullName) : Attribute
{
    public string FullName => fullName;
    
    internal static PremonitionType? FromCecilType(TypeDefinition td)
    {
        var attr = MetadataHelper.GetCustomAttributes<PremonitionType>(td,false).FirstOrDefault();
        return attr == null ? null : new PremonitionType((string)attr.ConstructorArguments[0].Value);
    }

    internal static PremonitionType? FromCecilMethod(MethodDefinition md)
    {
        var attr = MetadataHelper.GetCustomAttributes<PremonitionType>(md).FirstOrDefault();
        return attr == null ? null : new PremonitionType((string)attr.ConstructorArguments[0].Value);
    }
}
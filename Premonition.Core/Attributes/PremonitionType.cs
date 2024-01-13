using JetBrains.Annotations;
using Mono.Cecil;
using Premonition.Core.Utility;

namespace Premonition.Core.Attributes;


[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
[MeansImplicitUse]
[PublicAPI]
public class PremonitionType(string fullName) : Attribute
{
    public string FullName => fullName;
    
    internal static PremonitionType? FromCecilType(TypeDefinition td)
    {
        var attr = CecilHelper.GetCustomAttributes<PremonitionType>(td,false).FirstOrDefault();
        return attr == null ? null : new PremonitionType((string)attr.ConstructorArguments[0].Value);
    }

    internal static PremonitionType? FromCecilMethod(MethodDefinition md)
    {
        var attr = CecilHelper.GetCustomAttributes<PremonitionType>(md).FirstOrDefault();
        return attr == null ? null : new PremonitionType((string)attr.ConstructorArguments[0].Value);
    }
}
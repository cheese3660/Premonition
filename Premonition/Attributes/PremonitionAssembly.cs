using JetBrains.Annotations;
using Mono.Cecil;
using Premonition.Utility;

namespace Premonition.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
[MeansImplicitUse]
[PublicAPI]
public class PremonitionAssembly(string assembly) : Attribute
{
    public string Assembly => assembly;
    internal static PremonitionAssembly? FromCecilType(TypeDefinition td)
    {
        var attr = MetadataHelper.GetCustomAttributes<PremonitionAssembly>(td,false).FirstOrDefault();
        return attr == null ? null : new PremonitionAssembly((string)attr.ConstructorArguments[0].Value);
    }
    
    
    internal static PremonitionAssembly? FromCecilMethod(MethodDefinition md)
    {
        var attr = MetadataHelper.GetCustomAttributes<PremonitionAssembly>(md).FirstOrDefault();
        return attr == null ? null : new PremonitionAssembly((string)attr.ConstructorArguments[0].Value);
    }
}
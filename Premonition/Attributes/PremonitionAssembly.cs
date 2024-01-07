using Mono.Cecil;
using Premonition.Utility;

namespace Premonition.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public class PremonitionAssembly(string assembly) : Attribute
{


    internal static PremonitionAssembly? FromCecilType(TypeDefinition td)
    {
        var attr = MetadataHelper.GetCustomAttributes<PremonitionAssembly>(td,false).FirstOrDefault();
        return attr == null ? null : new PremonitionAssembly((string)attr.ConstructorArguments[0].Value);
    }
}
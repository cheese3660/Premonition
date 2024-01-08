using JetBrains.Annotations;
using Mono.Cecil;
using Premonition.Utility;

namespace Premonition.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
[MeansImplicitUse]
[PublicAPI]
public class PremonitionConstructor() : PremonitionMethod(MethodConstants.Constructor)
{
    internal new static PremonitionConstructor? FromCecilType(TypeDefinition td)
    {
        var attr = MetadataHelper.GetCustomAttributes<PremonitionConstructor>(td,false).FirstOrDefault();
        return attr == null ? null : new PremonitionConstructor();
    }
    
    
    internal new static PremonitionConstructor? FromCecilMethod(MethodDefinition md)
    {
        var attr = MetadataHelper.GetCustomAttributes<PremonitionConstructor>(md).FirstOrDefault();
        return attr == null ? null : new PremonitionConstructor();
    }
}
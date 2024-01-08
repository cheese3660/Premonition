using JetBrains.Annotations;
using Mono.Cecil;
using Premonition.Utility;

namespace Premonition.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
[MeansImplicitUse]
[PublicAPI]
public class PremonitionDestructor() : PremonitionMethod(MethodConstants.Destructor)
{
    
    internal new static PremonitionDestructor? FromCecilType(TypeDefinition td)
    {
        var attr = MetadataHelper.GetCustomAttributes<PremonitionDestructor>(td,false).FirstOrDefault();
        return attr == null ? null : new PremonitionDestructor();
    }
    
    
    internal new static PremonitionDestructor? FromCecilMethod(MethodDefinition md)
    {
        var attr = MetadataHelper.GetCustomAttributes<PremonitionDestructor>(md).FirstOrDefault();
        return attr == null ? null : new PremonitionDestructor();
    }
}
using Premonition.Utility;
using JetBrains.Annotations;
using Mono.Cecil;

namespace Premonition.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
[MeansImplicitUse]
[PublicAPI]
public class PremonitionIndexerGetter() : PremonitionGetter("Item")
{
    
    internal new static PremonitionIndexerGetter? FromCecilType(TypeDefinition td)
    {
        var attr = MetadataHelper.GetCustomAttributes<PremonitionIndexerGetter>(td,false).FirstOrDefault();
        return attr == null ? null : new PremonitionIndexerGetter();
    }
    
    
    internal new static PremonitionIndexerGetter? FromCecilMethod(MethodDefinition md)
    {
        var attr = MetadataHelper.GetCustomAttributes<PremonitionIndexerGetter>(md).FirstOrDefault();
        return attr == null ? null : new PremonitionIndexerGetter();
    }
}
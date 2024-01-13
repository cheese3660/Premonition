using JetBrains.Annotations;
using Mono.Cecil;
using Premonition.Core.Utility;

namespace Premonition.Core.Attributes;

[AttributeUsage(AttributeTargets.Method)]
[MeansImplicitUse]
[PublicAPI]
public class PremonitionIndexerGetter() : PremonitionGetter("Item")
{
    internal new static PremonitionIndexerGetter? FromCecilMethod(MethodDefinition md)
    {
        var attr = CecilHelper.GetCustomAttributes<PremonitionIndexerGetter>(md).FirstOrDefault();
        return attr == null ? null : new PremonitionIndexerGetter();
    }
}
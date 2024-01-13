using JetBrains.Annotations;
using Mono.Cecil;
using Premonition.Core.Utility;

namespace Premonition.Core.Attributes;

[AttributeUsage(AttributeTargets.Method)]
[MeansImplicitUse]
[PublicAPI]
public class PremonitionIndexerSetter() : PremonitionSetter("Item")
{
    internal new static PremonitionIndexerSetter? FromCecilMethod(MethodDefinition md)
    {
        var attr = CecilHelper.GetCustomAttributes<PremonitionIndexerSetter>(md).FirstOrDefault();
        return attr == null ? null : new PremonitionIndexerSetter();
    }
}
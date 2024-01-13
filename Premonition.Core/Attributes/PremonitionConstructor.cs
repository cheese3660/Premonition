using JetBrains.Annotations;
using Mono.Cecil;
using Premonition.Core.Utility;

namespace Premonition.Core.Attributes;

[AttributeUsage(AttributeTargets.Method)]
[MeansImplicitUse]
[PublicAPI]
public class PremonitionConstructor() : PremonitionMethod(MethodConstants.Constructor)
{
    internal new static PremonitionConstructor? FromCecilMethod(MethodDefinition md)
    {
        var attr = CecilHelper.GetCustomAttributes<PremonitionConstructor>(md).FirstOrDefault();
        return attr == null ? null : new PremonitionConstructor();
    }
}
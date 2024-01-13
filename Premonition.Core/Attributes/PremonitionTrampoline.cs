using JetBrains.Annotations;
using Mono.Cecil;
using Premonition.Core.Utility;

namespace Premonition.Core.Attributes;

[AttributeUsage(AttributeTargets.Method)]
[MeansImplicitUse]
[PublicAPI]
public class PremonitionTrampoline : Attribute
{
    internal static PremonitionTrampoline? FromCecilMethod(MethodDefinition md)
    {
        var attr = CecilHelper.GetCustomAttributes<PremonitionTrampoline>(md).FirstOrDefault();
        return attr == null ? null : new PremonitionTrampoline();
    }
}
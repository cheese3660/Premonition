using JetBrains.Annotations;
using Mono.Cecil;
using Premonition.Core.Utility;

namespace Premonition.Core.Attributes;

[AttributeUsage(AttributeTargets.Method)]
[MeansImplicitUse]
[PublicAPI]
public class PremonitionSetter(string property) : PremonitionMethod($"set_{property}")
{
    internal new static PremonitionSetter? FromCecilMethod(MethodDefinition md)
    {
        var attr = CecilHelper.GetCustomAttributes<PremonitionSetter>(md).FirstOrDefault();
        return attr == null ? null : new PremonitionSetter((string)attr.ConstructorArguments[0].Value);
    }
}
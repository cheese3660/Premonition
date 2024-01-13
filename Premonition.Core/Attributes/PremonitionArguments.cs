using JetBrains.Annotations;
using Mono.Cecil;
using Premonition.Core.Utility;

namespace Premonition.Core.Attributes;


[AttributeUsage(AttributeTargets.Method)]
[MeansImplicitUse]
[PublicAPI]
public class PremonitionArguments(params string[] argumentTypes) : Attribute
{
    public List<string> ArgumentTypes => argumentTypes.ToList();
    internal static PremonitionArguments? FromCecilMethod(MethodDefinition md)
    {
        var attr = CecilHelper.GetCustomAttributes<PremonitionArguments>(md).FirstOrDefault();
        return attr == null ? null : new PremonitionArguments((string[])attr.ConstructorArguments[0].Value);
    }
}
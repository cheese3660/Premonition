using JetBrains.Annotations;
using Mono.Cecil;
using Premonition.Core.Utility;

namespace Premonition.Core.Attributes;

[AttributeUsage(AttributeTargets.Method)]
[MeansImplicitUse]
[PublicAPI]
public class PremonitionMethod(string methodName) : Attribute
{
    public string MethodName => methodName;

    internal static PremonitionMethod? FromCecilMethod(MethodDefinition md)
    {
        var attr = CecilHelper.GetCustomAttributes<PremonitionMethod>(md).FirstOrDefault();
        return attr == null ? null : new PremonitionMethod((string)attr.ConstructorArguments[0].Value);
    }
}
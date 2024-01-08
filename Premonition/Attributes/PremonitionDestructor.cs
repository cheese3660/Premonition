using JetBrains.Annotations;
using Mono.Cecil;
using Premonition.Utility;

namespace Premonition.Attributes;

[AttributeUsage(AttributeTargets.Method)]
[MeansImplicitUse]
[PublicAPI]
public class PremonitionDestructor() : PremonitionMethod(MethodConstants.Destructor)
{
    internal new static PremonitionDestructor? FromCecilMethod(MethodDefinition md)
    {
        var attr = CecilHelper.GetCustomAttributes<PremonitionDestructor>(md).FirstOrDefault();
        return attr == null ? null : new PremonitionDestructor();
    }
}
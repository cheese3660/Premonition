using System;
using System.Linq;
using JetBrains.Annotations;
using Mono.Cecil;
using Premonition.Core.Utility;

namespace Premonition.Core.Attributes;

/// <summary>
/// Patch a property setter
/// </summary>
/// <param name="property">The name of the property being patched</param>
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
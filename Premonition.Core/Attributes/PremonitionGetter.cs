using System;
using System.Linq;
using JetBrains.Annotations;
using Mono.Cecil;
using Premonition.Core.Utility;

namespace Premonition.Core.Attributes;

/// <summary>
/// Patch a getter for a property
/// </summary>
/// <param name="property">The name of the property</param>
[AttributeUsage(AttributeTargets.Method)]
[MeansImplicitUse]
[PublicAPI]
public class PremonitionGetter(string property) : PremonitionMethod($"get_{property}")
{

    internal new static PremonitionGetter? FromCecilMethod(MethodDefinition md)
    {
        var attr = CecilHelper.GetCustomAttributes<PremonitionGetter>(md).FirstOrDefault();
        return attr == null ? null : new PremonitionGetter((string)attr.ConstructorArguments[0].Value);
    }
}
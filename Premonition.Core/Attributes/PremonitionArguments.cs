using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Mono.Cecil;
using Premonition.Core.Utility;

namespace Premonition.Core.Attributes;


/// <summary>
/// Determines the argument types to match against for the target method
/// </summary>
/// <param name="argumentTypes">A list of argument types, where each type is the full typename, or null for any argument type</param>
[AttributeUsage(AttributeTargets.Method)]
[MeansImplicitUse]
[PublicAPI]
public class PremonitionArguments(params string[] argumentTypes) : Attribute
{
    /// <summary>
    /// The argument types to check against, null means any type
    /// </summary>
    public List<string>? ArgumentTypes => argumentTypes.ToList();
    internal static PremonitionArguments? FromCecilMethod(MethodDefinition md)
    {
        var attr = CecilHelper.GetCustomAttributes<PremonitionArguments>(md).FirstOrDefault();
        return attr == null ? null : new PremonitionArguments((string[])attr.ConstructorArguments[0].Value);
    }
}
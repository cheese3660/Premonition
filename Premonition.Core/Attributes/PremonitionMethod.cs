using System;
using System.Linq;
using JetBrains.Annotations;
using Mono.Cecil;
using Premonition.Core.Utility;

namespace Premonition.Core.Attributes;

/// <summary>
/// Determines the method to be patched
/// </summary>
/// <param name="methodName">The name of the method to be patched</param>
[AttributeUsage(AttributeTargets.Method)]
[MeansImplicitUse]
[PublicAPI]
public class PremonitionMethod(string methodName) : Attribute
{
    /// <summary>
    /// The name of the method being matched against for patching purposes
    /// </summary>
    public string MethodName => methodName;

    internal static PremonitionMethod? FromCecilMethod(MethodDefinition md)
    {
        var attr = CecilHelper.GetCustomAttributes<PremonitionMethod>(md).FirstOrDefault();
        return attr == null ? null : new PremonitionMethod((string)attr.ConstructorArguments[0].Value);
    }
}
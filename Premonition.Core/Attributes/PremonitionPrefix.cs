using System;
using System.Linq;
using JetBrains.Annotations;
using Mono.Cecil;
using Premonition.Core.Utility;

namespace Premonition.Core.Attributes;

/// <summary>
/// Patch the target method by running code when the method returns
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
[MeansImplicitUse]
[PublicAPI]
public class PremonitionPrefix : Attribute
{
    internal static PremonitionPrefix? FromCecilMethod(MethodDefinition md)
    {
        var attr = CecilHelper.GetCustomAttributes<PremonitionPrefix>(md).FirstOrDefault();
        return attr == null ? null : new PremonitionPrefix();
    }
}
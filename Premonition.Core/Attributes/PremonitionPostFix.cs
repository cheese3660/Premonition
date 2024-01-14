using System;
using System.Linq;
using JetBrains.Annotations;
using Mono.Cecil;
using Premonition.Core.Utility;

namespace Premonition.Core.Attributes;

/// <summary>
/// Patch the target method by running this method before the target runs
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
[MeansImplicitUse]
[PublicAPI]
public class PremonitionPostfix : Attribute
{
    internal static PremonitionPostfix? FromCecilMethod(MethodDefinition md)
    {
        var attr = CecilHelper.GetCustomAttributes<PremonitionPostfix>(md).FirstOrDefault();
        return attr == null ? null : new PremonitionPostfix();
    }
}
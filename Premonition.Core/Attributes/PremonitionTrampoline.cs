using System;
using System.Linq;
using JetBrains.Annotations;
using Mono.Cecil;
using Premonition.Core.Utility;

namespace Premonition.Core.Attributes;

/// <summary>
/// Patch the target method by replacing its body with a call to this method
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
[MeansImplicitUse]
[PublicAPI]
public class PremonitionTrampoline : Attribute
{
    internal static PremonitionTrampoline? FromCecilMethod(MethodDefinition md)
    {
        var attr = CecilHelper.GetCustomAttributes<PremonitionTrampoline>(md).FirstOrDefault();
        return attr == null ? null : new PremonitionTrampoline();
    }
}
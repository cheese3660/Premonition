using System;
using System.Linq;
using JetBrains.Annotations;
using Mono.Cecil;
using Premonition.Core.Utility;

namespace Premonition.Core.Attributes;

/// <summary>
/// Patch a method that is a destructor
/// </summary>
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
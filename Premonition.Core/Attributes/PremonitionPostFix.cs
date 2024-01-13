﻿using JetBrains.Annotations;
using Mono.Cecil;
using Premonition.Core.Utility;

namespace Premonition.Core.Attributes;

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
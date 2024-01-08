﻿using JetBrains.Annotations;
using Mono.Cecil;
using Premonition.Utility;

namespace Premonition.Attributes;

[AttributeUsage(AttributeTargets.Method)]
[MeansImplicitUse]
[PublicAPI]
public class PremonitionSetter(string property) : PremonitionMethod($"set_{property}")
{
    internal new static PremonitionSetter? FromCecilMethod(MethodDefinition md)
    {
        var attr = MetadataHelper.GetCustomAttributes<PremonitionSetter>(md).FirstOrDefault();
        return attr == null ? null : new PremonitionSetter((string)attr.ConstructorArguments[0].Value);
    }
}
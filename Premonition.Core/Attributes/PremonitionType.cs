using System;
using System.Linq;
using JetBrains.Annotations;
using Mono.Cecil;
using Premonition.Core.Utility;

namespace Premonition.Core.Attributes;


/// <summary>
/// Determine the type to be patched
/// </summary>
/// <param name="fullName">The full name of the type being patched</param>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
[MeansImplicitUse]
[PublicAPI]
public class PremonitionType(string fullName) : Attribute
{
    /// <summary>
    /// The full name of the target type to be matched against for patching
    /// </summary>
    public string FullName => fullName;
    
    internal static PremonitionType? FromCecilType(TypeDefinition td)
    {
        var attr = CecilHelper.GetCustomAttributes<PremonitionType>(td,false).FirstOrDefault();
        return attr == null ? null : new PremonitionType((string)attr.ConstructorArguments[0].Value);
    }

    internal static PremonitionType? FromCecilMethod(MethodDefinition md)
    {
        var attr = CecilHelper.GetCustomAttributes<PremonitionType>(md).FirstOrDefault();
        return attr == null ? null : new PremonitionType((string)attr.ConstructorArguments[0].Value);
    }
}
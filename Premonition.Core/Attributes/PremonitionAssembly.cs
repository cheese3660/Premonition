using System;
using System.Linq;
using JetBrains.Annotations;
using Mono.Cecil;
using Premonition.Core.Utility;

namespace Premonition.Core.Attributes;

/// <summary>
/// Determine which assembly (DLL) is being patched.
/// </summary>
/// <param name="assembly">The name of the assembly to be patched.</param>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
[MeansImplicitUse]
[PublicAPI]
public class PremonitionAssembly(string assembly) : Attribute
{
    /// <summary>
    /// The name of the assembly to be matched against for patching.
    /// </summary>
    public string Assembly => assembly;
    internal static PremonitionAssembly? FromCecilType(TypeDefinition td)
    {
        var attr = CecilHelper.GetCustomAttributes<PremonitionAssembly>(td,false).FirstOrDefault();
        return attr == null ? null : new PremonitionAssembly((string)attr.ConstructorArguments[0].Value);
    }
    
    
    internal static PremonitionAssembly? FromCecilMethod(MethodDefinition md)
    {
        var attr = CecilHelper.GetCustomAttributes<PremonitionAssembly>(md).FirstOrDefault();
        return attr == null ? null : new PremonitionAssembly((string)attr.ConstructorArguments[0].Value);
    }
}
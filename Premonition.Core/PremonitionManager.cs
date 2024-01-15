using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Premonition.Core.Attributes;

namespace Premonition.Core;

/// <summary>
/// This is the main manager class for premonition patching
/// </summary>
public class PremonitionManager(IAssemblyResolver? resolver = null)
{
    
    /// <summary>
    /// The list of currently active patcher methods
    /// </summary>
    public readonly List<PremonitionPatcher> PremonitionPatchers = [];
    
    /// <summary>
    /// Reads a dll file to get all the patchers from it
    /// </summary>
    /// <param name="dllPath">Said dll file</param>
    public void ReadAssembly(string dllPath)
    {
        var assemblyDefinition = AssemblyDefinition.ReadAssembly(dllPath, new ReaderParameters
        {
            AssemblyResolver = resolver
        });
        RegisterAssembly(assemblyDefinition);
    }



    private void RegisterAssembly(AssemblyDefinition definition)
    {
        foreach (var type in definition.Modules.SelectMany(x => x.GetTypes()))
        {
            RegisterType(type);
        }
    }


    private void RegisterType(TypeDefinition type)
    {
        string? assemblyName = null;
        string? typeName = null;
        if (PremonitionAssembly.FromCecilType(type) is {} premonitionAssembly)
        {
            assemblyName = premonitionAssembly.Assembly;
        }

        if (PremonitionType.FromCecilType(type) is { } premonitionType)
        {
            typeName = premonitionType.FullName;
        }

        foreach (var method in type.Methods.Where(x => x.IsStatic))
        {
            RegisterMethod(method,assemblyName,typeName);
        }
    }

    private void RegisterMethod(MethodDefinition method, string? assemblyName, string? typeName)
    {
        bool hadPremonitionAttribute = false;
        if (PremonitionAssembly.FromCecilMethod(method) is { } premonitionAssembly)
        {
            hadPremonitionAttribute = true;
            if (!string.IsNullOrEmpty(assemblyName))
            {
                LogError(
                    $"Assembly name for patch method {method.FullName} is overqualified, this method will not be used");
                return;
            }
            assemblyName = premonitionAssembly.Assembly;
        }

        if (PremonitionType.FromCecilMethod(method) is { } premonitionType)
        {
            hadPremonitionAttribute = true;
            if (!string.IsNullOrEmpty(typeName))
            {
                LogError(
                    $"Type name for patch method {method.FullName} is overqualified, this method will not be used");
                return;
            }
            typeName = premonitionType.FullName;
        }

        if (assemblyName != null && typeName != null)
        {
            List<string> missingAttributes = [];
            var methodName = GetMethodName(method);
            if (methodName != null) hadPremonitionAttribute = true;
            else
                missingAttributes.Add(
                    "PremonitionMethod/PremonitionConstructor/PremonitionDestructor/PremonitionGetter/PremonitionSetter/PremonitionIndexerGetter/PremonitionIndexerSetter");
            var patchType = GetPatchType(method);

            if (patchType != null) hadPremonitionAttribute = true;
            else missingAttributes.Add("PremonitionPrefix/PremonitionPostfix/PremonitionTrampoline");

            var argumentTypes = GetArgumentTypes(method);
            
            
            // ReSharper disable once ConvertIfStatementToSwitchStatement
            if (hadPremonitionAttribute && missingAttributes.Count > 0)
            {
                LogError(
                    $"Patch method {method.FullName} is missing the following necessary attributes: {string.Join(", ", missingAttributes)}, this method will not be used");
                return;
            } 
            if (!hadPremonitionAttribute)
            {
                return;
            }
            
            PremonitionPatchers.Add(new PremonitionPatcher(assemblyName,typeName,methodName!,argumentTypes,patchType!.Value,method));
            
        } else if (hadPremonitionAttribute)
        {
            LogError(
                $"Patch method {method.FullName} or type {method.DeclaringType.FullName} is missing a {(typeName == null ? "PremonitionType" : "PremonitionAssembly")} attribute, this method will not be used");
        }
    }

    private List<string>? GetArgumentTypes(MethodDefinition method) =>
        PremonitionArguments.FromCecilMethod(method)?.ArgumentTypes;
    
    private PatchType? GetPatchType(MethodDefinition method)
    {
        PatchType? patchType = null;
        if (PremonitionPrefix.FromCecilMethod(method) is not null)
        {
            patchType = PatchType.Prefix;
        }

        if (PremonitionPostfix.FromCecilMethod(method) is not null)
        {
            if (patchType.HasValue)
            {
                LogError(
                    $"Patch type for patch method {method.FullName} is overqualified, this method will not be used");
                return null;
            }
            patchType = PatchType.Postfix;
        }
        if (PremonitionTrampoline.FromCecilMethod(method) is not null)
        {
            if (patchType.HasValue)
            {
                LogError(
                    $"Patch type for patch method {method.FullName} is overqualified, this method will not be used");
                return null;
            }
            patchType = PatchType.Trampoline;
        }

        return patchType;
    }
    
    private string? GetMethodName(MethodDefinition method)
    {
        string? methodName = null;
        if (PremonitionMethod.FromCecilMethod(method) is { } premonitionMethod)
        {
            methodName = premonitionMethod.MethodName;
        }

        if (PremonitionConstructor.FromCecilMethod(method) is { } premonitionConstructor)
        {
            if (!string.IsNullOrEmpty(methodName))
            {
                LogError(
                    $"Method name for patch method {method.FullName} is overqualified, this method will not be used");
                return null;
            }
            methodName = premonitionConstructor.MethodName;
        }
        
        if (PremonitionDestructor.FromCecilMethod(method) is { } premonitionDestructor)
        {
            if (!string.IsNullOrEmpty(methodName))
            {
                LogError(
                    $"Method name for patch method {method.FullName} is overqualified, this method will not be used");
                return null;
            }
            methodName = premonitionDestructor.MethodName;
        }

        if (PremonitionGetter.FromCecilMethod(method) is { } premonitionGetter)
        {
            if (!string.IsNullOrEmpty(methodName))
            {
                LogError(
                    $"Method name for patch method {method.FullName} is overqualified, this method will not be used");
                return null;
            }
            methodName = premonitionGetter.MethodName;
        }
        
        if (PremonitionSetter.FromCecilMethod(method) is { } premonitionSetter)
        {
            if (!string.IsNullOrEmpty(methodName))
            {
                LogError(
                    $"Method name for patch method {method.FullName} is overqualified, this method will not be used");
                return null;
            }
            methodName = premonitionSetter.MethodName;
        }
        
        if (PremonitionIndexerGetter.FromCecilMethod(method) is { } premonitionIndexerGetter)
        {
            if (!string.IsNullOrEmpty(methodName))
            {
                LogError(
                    $"Method name for patch method {method.FullName} is overqualified, this method will not be used");
                return null;
            }
            methodName = premonitionIndexerGetter.MethodName;
        }
        
        if (PremonitionIndexerSetter.FromCecilMethod(method) is { } premonitionIndexerSetter)
        {
            if (!string.IsNullOrEmpty(methodName))
            {
                LogError(
                    $"Method name for patch method {method.FullName} is overqualified, this method will not be used");
                return null;
            }
            methodName = premonitionIndexerSetter.MethodName;
        }
        
        
        
        return methodName;
    }


    /// <summary>
    /// Apply the active patchers to an assembly, removing any that are applied
    /// </summary>
    /// <param name="definition">The assembly to be modified</param>
    public void Patch(AssemblyDefinition definition)
    {
        List<int> toRemove = [];
        for (var i = 0; i < PremonitionPatchers.Count; i++)
        {
            if (PremonitionPatchers[i].Patch(definition))
            {
                toRemove.Add(i);
            }
        }

        toRemove.Reverse();
        foreach (var index in toRemove)
        {
            PremonitionPatchers.RemoveAt(index);
        }
    }
}
using System;
using System.Text;
using Mono.Cecil;

namespace Premonition.Core.Utility;

/// <summary>
/// A set of extensions to make dealing with cecil easier
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Makes a method a generic instances based on generic parameters
    /// </summary>
    /// <param name="method">The method being made into a generic instance</param>
    /// <param name="args">The generic parameters to apply</param>
    /// <returns>A generic instance of the method with the given generic parameters</returns>
    /// <exception cref="ArgumentException">Thrown if there are an invalid number of generic parameters</exception>
    public static MethodReference MakeGeneric(this MethodReference method, params GenericParameter[] args)
    {
        if (args.Length == 0)
        {
            return method;
        }

        if (method.GenericParameters.Count != args.Length)
        {
            throw new ArgumentException("Invalid number of generic type arguments supplied");
        }

        var genericTypeRef = new GenericInstanceMethod(method);
        foreach (var arg in args)
        {
            genericTypeRef.GenericArguments.Add(arg);
        }

        return genericTypeRef;
    }

    private static string ToIlString(this MethodDefinition definition)
    {
        var sb = new StringBuilder();
        if (definition.IsStatic) sb.Append("static ");
        sb.Append(definition);
        sb.Append(" [\n");
        foreach (var variable in definition.Body.Variables)
        {
            sb.Append($"\t{variable}\n");
        }
        sb.Append("]\n{\n");
        foreach (var instruction in definition.Body.Instructions)
        {
            sb.Append($"\t{instruction}\n");
        }

        sb.Append("}");
        return sb.ToString();
    }

    
    // ReSharper disable once UnusedMember.Global
    internal static void Dump(this MethodDefinition definition)
    {
        foreach (var line in definition.ToIlString().Split("\n"))
        {
            LogInfo(line);
        }
    }
    
}
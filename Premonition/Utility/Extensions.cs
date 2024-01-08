using System.Text;
using Mono.Cecil;

namespace Premonition.Utility;

public static class Extensions
{
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
    
    public static string ToIlString(this MethodDefinition definition)
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

    public static void Dump(this MethodDefinition definition)
    {
        foreach (var line in definition.ToIlString().Split("\n"))
        {
            Premonition.LogSource.LogInfo(line);
        }
    }
    
}
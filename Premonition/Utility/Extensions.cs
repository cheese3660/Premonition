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
}
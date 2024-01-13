using Mono.Cecil;

namespace Premonition.Core.Utility;

public static class CecilHelper
{
    public static IEnumerable<CustomAttribute> GetCustomAttributes<T>(
        TypeDefinition td,
        bool inherit)
        where T : Attribute
    {
        var customAttributes = new List<CustomAttribute>();
        var type = typeof (T);
        var typeDefinition = td;
        do
        {
            customAttributes.AddRange(typeDefinition!.CustomAttributes.Where<CustomAttribute>((Func<CustomAttribute, bool>) (ca => ca.AttributeType.FullName == type.FullName)));
            typeDefinition = typeDefinition.BaseType?.Resolve();
        }
        while (inherit && typeDefinition?.FullName != "System.Object");
        return customAttributes;
    }
    
    public static IEnumerable<CustomAttribute> GetCustomAttributes<T>(
        MethodDefinition md)
        where T : Attribute
    {
        var customAttributes = new List<CustomAttribute>();
        var type = typeof (T);
        customAttributes.AddRange(md!.CustomAttributes.Where<CustomAttribute>((Func<CustomAttribute, bool>) (ca => ca.AttributeType.FullName == type.FullName)));
        return customAttributes;
    }
}
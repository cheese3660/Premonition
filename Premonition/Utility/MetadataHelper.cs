using Mono.Cecil;

namespace Premonition.Utility;

public static class MetadataHelper
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
}
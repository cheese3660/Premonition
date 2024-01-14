using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

namespace Premonition.Core.Utility;

/// <summary>
/// A helper class for dealing with getting attributes from cecil
/// </summary>
public static class CecilHelper
{
    /// <summary>
    /// Get custom attributes from a cecil type
    /// </summary>
    /// <param name="td">The cecil type</param>
    /// <param name="inherit">Are parent type attributes considered?</param>
    /// <typeparam name="T">The type of the attribute to get</typeparam>
    /// <returns>All the attributes that are found</returns>
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
    
    /// <summary>
    /// Get custom attributes from a cecil method
    /// </summary>
    /// <param name="md">The cecil method</param>
    /// <typeparam name="T">The type of the attribute to get</typeparam>
    /// <returns>All the attributes that are found</returns>
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
using Mono.Cecil;
using Mono.Cecil.Rocks;

namespace Premonition;

internal class PremonitionPatcher(
    string assembly,
    string typeName,
    string methodName,
    List<string>? argumentTypes,
    PatchType patchType,
    MethodReference patchMethod)
{
    internal string Assembly => assembly;

    /// <summary>
    /// Patches the method in the target assembly
    /// </summary>
    /// <param name="definition">The assembly being referred to</param>
    /// <returns>If this patcher has completed its purpose (successfully or not)</returns>
    internal bool Patch(AssemblyDefinition definition)
    {
        if (definition.FullName != assembly) return false;
        var type = definition.Modules.SelectMany(x => x.GetTypes())
            .FirstOrDefault(x => x.FullName == typeName);
        if (type == null)
        {
            Premonition.LogSource.LogError(
                $"Error patching {assembly}:{typeName}:{methodName} with method {patchMethod.FullName}, type does not exist!");
            return true;
        }

        var possibleMethods = type.GetMethods().Where(method => method.Name == methodName).ToList();
        if (possibleMethods.Count == 0)
        {
            Premonition.LogSource.LogError(
                $"Error patching {assembly}:{typeName}:{methodName} with method {patchMethod.FullName}, method does not exist!");
            return true;
        }

        MethodDefinition method;
        if (argumentTypes != null)
        {
            possibleMethods =
                possibleMethods.Where(possibleMethod => possibleMethod.Parameters.Count == argumentTypes.Count)
                    .ToList();
            if (possibleMethods.Count == 0)
            {
                Premonition.LogSource.LogError(
                    $"Error patching {assembly}:{typeName}:{methodName} with method {patchMethod.FullName}, method overload does not exist with specified arguments!");
                return true;
            }
            List<MethodDefinition> selectedMethods = [];
            foreach (var possibleMethod in possibleMethods)
            {
                var validMethod = true;
                for (var i = 0; i < argumentTypes.Count; i++)
                {
                    if (argumentTypes[i] == null) continue;
                    if (possibleMethod.Parameters[i].ParameterType.FullName == argumentTypes[i]) continue;
                    validMethod = false;
                    break;
                }
                if (validMethod)
                {
                    selectedMethods.Add(possibleMethod);
                }
            }

            if (selectedMethods.Count == 0)
            {
                Premonition.LogSource.LogError(
                    $"Error patching {assembly}:{typeName}:{methodName} with method {patchMethod.FullName}, method overload does not exist with specified arguments!");
                return true;
            }

            if (selectedMethods.Count > 1)
            {
                Premonition.LogSource.LogError(
                    $"Error patching {assembly}:{typeName}:{methodName} with method {patchMethod.FullName}, ambiguous method reference! (Possible methods are as follows)");
                foreach (var selectedMethod in selectedMethods)
                {
                    Premonition.LogSource.LogInfo(
                        $"{selectedMethod.FullName}{(selectedMethod.GenericParameters.Count > 0 ? $"<{string.Join(", ", selectedMethod.GenericParameters.Select(x => x.FullName))}>" : "")}({string.Join(", ", selectedMethod.Parameters.Select(x => x.ParameterType.FullName))})");
                }
                return true;
            }

            method = selectedMethods.First();
        }
        else
        {
            if (possibleMethods.Count > 1)
            {
                Premonition.LogSource.LogError(
                    $"Error patching {assembly}:{typeName}:{methodName} with method {patchMethod.FullName}, ambiguous method reference! (Possible methods are as follows)");
                foreach (var possibleMethod in possibleMethods)
                {
                    Premonition.LogSource.LogInfo(
                        $"{possibleMethod.FullName}{(possibleMethod.GenericParameters.Count > 0 ? $"<{string.Join(", ", possibleMethod.GenericParameters.Select(x => x.FullName))}>" : "")}({string.Join(", ", possibleMethod.Parameters.Select(x => x.ParameterType.FullName))})");
                }

                return true;
            }
            method = possibleMethods.First();
        }

        return true;
    }
}
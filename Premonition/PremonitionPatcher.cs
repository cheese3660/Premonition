using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using Mono.Collections.Generic;
using MonoMod.Utils;
using static Premonition.Utility.Extensions;

namespace Premonition;

internal class PremonitionPatcher(
    string assembly,
    string typeName,
    string methodName,
    IReadOnlyList<string>? argumentTypes,
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


    private static void Patch(PatchType type, MethodDefinition methodBeingPatched, MethodReference patchMethod)
    {
        switch (type)
        {
            case PatchType.Prefix:
                PrefixPatch(methodBeingPatched, patchMethod);
                break;
            case PatchType.Postfix:
                PostfixPatch(methodBeingPatched, patchMethod);
                break;
            case PatchType.Trampoline:
                TrampolinePatch(methodBeingPatched, patchMethod);
                break;
            default:
                Premonition.LogSource.LogError(
                    $"Error patching {methodBeingPatched.FullName} with {patchMethod.FullName}, invalid patch type: {type}");
                break;
        }
    }


    private static MethodReference? Import(MethodReference methodBeingPatched, MethodReference patchMethod)
    {
        var patchMethodInModule = methodBeingPatched.Module.ImportReference(patchMethod);
        if (methodBeingPatched.HasGenericParameters)
        {
            if (!patchMethod.HasGenericParameters)
            {
                Premonition.LogSource.LogError($"Error patching {methodBeingPatched.FullName} with {patchMethod.FullName}, the method being patched has generic parameters while the patch method does not");
                return null;
            }

            if (patchMethod.GenericParameters.Count != methodBeingPatched.GenericParameters.Count)
            {
                Premonition.LogSource.LogError(
                    $"Error patching {methodBeingPatched.FullName} with {patchMethod.FullName}, mismatched amount of generic parameters");
                return null;
            }

            try
            {
                patchMethodInModule = patchMethodInModule.MakeGeneric(methodBeingPatched.GenericParameters.ToArray());
            }
            catch (Exception e)
            {
                Premonition.LogSource.LogError(
                    $"Error patching {methodBeingPatched.FullName} with {patchMethod.FullName}, mismatched amount of generic {e}");
                return null;
            }
        } else if (patchMethod.HasGenericParameters)
        {
            Premonition.LogSource.LogError($"Error patching {methodBeingPatched.FullName} with {patchMethod.FullName}, the patch method has generic parameters while the method being patched does not");
            return null;
        }
        return patchMethodInModule;
    }

    private static void PrefixPatch(MethodDefinition methodBeingPatched, MethodReference patchMethod)
    {
    }

    private static void PostfixPatch(MethodDefinition methodBeingPatched, MethodReference patchMethod)
    {
        
        if (patchMethod.ReturnType.FullName == "System.Void")
        {
            PostfixPatchVoid(methodBeingPatched, patchMethod);
        }
        else
        {
            if (patchMethod.ReturnType.FullName != methodBeingPatched.ReturnType.FullName)
            {
                Premonition.LogSource.LogError(
                    $"Error patching {methodBeingPatched.FullName} with {patchMethod.FullName}, mismatched return types");
                return;
            }

            if (patchMethod.Parameters.Count == 0 || patchMethod.Parameters[0].Name != "__retVal")
            {
                PostfixPatchTailCall(methodBeingPatched, patchMethod, true);
            }
            else if (patchMethod.Parameters[0].ParameterType.FullName == methodBeingPatched.ReturnType.FullName)
            {
                PostfixPatchTailCall(methodBeingPatched, patchMethod);
            }
            else
            {
                Premonition.LogSource.LogError(
                    $"Error patching {methodBeingPatched.FullName} with {patchMethod.FullName}, type of __retVal argument does not match the return type of the method being patched");
                return;
            }
        }
    }
    
    private static IEnumerator<Instruction> ReplaceReturnInstructions(IEnumerable<Instruction> body,List<Instruction> replacement) {
        foreach (var instruction in body)
        {
            if (instruction.OpCode != OpCodes.Ret) yield return instruction;
            foreach (var replacementInstruction in replacement)
            {
                yield return replacementInstruction;
            }
            yield return instruction;
        }
    }
    
    private static void PostfixPatchTailCall(MethodDefinition methodBeingPatched, MethodReference patchMethod, bool drop=false)
    {
        // We already know with this one that the first argument should be the return value, so lets get every other argument
        List<int> argumentIndices = [];

        foreach (var argument in patchMethod.Parameters.Skip(1))
        {
            if (argument.Name == "__instance")
            {
                argumentIndices.Add(0);
            } else if (argument.Name == "__retVal")
            {
                Premonition.LogSource.LogError(
                    $"Error patching {methodBeingPatched.FullName} with {patchMethod.FullName}, __retVal must be the first argument if it is used");
                return;
            }
            else
            {
                var found = false;
                for (var i = 0; i < methodBeingPatched.Parameters.Count; i++)
                {
                    if (methodBeingPatched.Parameters[i].Name != argument.Name) continue;
                    found = true;
                    argumentIndices.Add(i);
                    break;
                }
                if (found) continue;
                Premonition.LogSource.LogError(
                    $"Error patching {methodBeingPatched.FullName} with {patchMethod.FullName}, unknown argument: {argument.Name}");
                return;
            }
        }

        var patchMethodInModule = Import(methodBeingPatched, patchMethod);
        if (patchMethodInModule == null) return;

        List<Instruction> replacementInstructions = [];
        if (drop) replacementInstructions.Add(Instruction.Create(OpCodes.Pop));
        foreach (var argumentIndex in argumentIndices)
        {
            switch (argumentIndex)
            {
                case 0:
                    replacementInstructions.Add(Instruction.Create(OpCodes.Ldarg_0));
                    break;
                case 1:
                    replacementInstructions.Add(Instruction.Create(OpCodes.Ldarg_1));
                    break;
                case 2:
                    replacementInstructions.Add(Instruction.Create(OpCodes.Ldarg_2));
                    break;
                case 3:
                    replacementInstructions.Add(Instruction.Create(OpCodes.Ldarg_3));
                    break;
                default:
                {
                    replacementInstructions.Add(argumentIndex < 256
                        ? Instruction.Create(OpCodes.Ldarg_S, methodBeingPatched.Parameters[argumentIndex])
                        : Instruction.Create(OpCodes.Ldarg, methodBeingPatched.Parameters[argumentIndex]));
                    break;
                }
            }
        }
        replacementInstructions.Add(Instruction.Create(OpCodes.Call, patchMethodInModule));
        var newBody = new Collection<Instruction>();
        var enumerator = ReplaceReturnInstructions(methodBeingPatched.Body.Instructions, replacementInstructions);
        while (enumerator.MoveNext())
        {
            newBody.Add(enumerator.Current);
        }

        methodBeingPatched.Body.Instructions.Clear();
        methodBeingPatched.Body.Instructions.AddRange(newBody);
    }

    private static void PostfixPatchVoid(MethodDefinition methodBeingPatched, MethodReference patchMethod)
    {
        List<int> argumentIndices = [];
        var first = true;
        foreach (var argument in patchMethod.Parameters)
        {
            if (argument.Name == "__instance")
            {
                argumentIndices.Add(0);
            }
            else if (argument.Name == "__retVal")
            {
                if (first)
                {
                    if (argument.ParameterType.FullName != methodBeingPatched.ReturnType.FullName)
                    {
                        Premonition.LogSource.LogError(
                            $"Error patching {methodBeingPatched.FullName} with {patchMethod.FullName}, type of __retVal argument does not match the return type of the method being patched");
                        return;
                    }
                    argumentIndices.Add(-1);
                }
                else
                {
                    Premonition.LogSource.LogError(
                        $"Error patching {methodBeingPatched.FullName} with {patchMethod.FullName}, __retVal must be the first argument if it is used");
                    return;
                }
            }
            else
            {
                var found = false;
                for (var i = 0; i < methodBeingPatched.Parameters.Count; i++)
                {
                    if (methodBeingPatched.Parameters[i].Name != argument.Name) continue;
                    found = true;
                    argumentIndices.Add(i);
                    break;
                }
                if (found) continue;
                Premonition.LogSource.LogError(
                    $"Error patching {methodBeingPatched.FullName} with {patchMethod.FullName}, unknown argument: {argument.Name}");
                return;
            }
            first = false;
        }

        var patchMethodInModule = Import(methodBeingPatched, patchMethod);
        if (patchMethodInModule == null) return;

        List<Instruction> replacementInstructions = [];
        foreach (var argumentIndex in argumentIndices)
        {
            switch (argumentIndex)
            {
                case -1:
                    replacementInstructions.Add(Instruction.Create(OpCodes.Dup));
                    break;
                case 0:
                    replacementInstructions.Add(Instruction.Create(OpCodes.Ldarg_0));
                    break;
                case 1:
                    replacementInstructions.Add(Instruction.Create(OpCodes.Ldarg_1));
                    break;
                case 2:
                    replacementInstructions.Add(Instruction.Create(OpCodes.Ldarg_2));
                    break;
                case 3:
                    replacementInstructions.Add(Instruction.Create(OpCodes.Ldarg_3));
                    break;
                default:
                {
                    replacementInstructions.Add(argumentIndex < 256
                        ? Instruction.Create(OpCodes.Ldarg_S, methodBeingPatched.Parameters[argumentIndex])
                        : Instruction.Create(OpCodes.Ldarg, methodBeingPatched.Parameters[argumentIndex]));
                    break;
                }
            }
        }
        replacementInstructions.Add(Instruction.Create(OpCodes.Call, patchMethodInModule));
        var newBody = new Collection<Instruction>();
        var enumerator = ReplaceReturnInstructions(methodBeingPatched.Body.Instructions, replacementInstructions);
        while (enumerator.MoveNext())
        {
            newBody.Add(enumerator.Current);
        }

        methodBeingPatched.Body.Instructions.Clear();
        methodBeingPatched.Body.Instructions.AddRange(newBody);
    }
    
    private static void TrampolinePatch(MethodDefinition methodBeingPatched, MethodReference patchMethod)
    {
        // This is the simplest style of patch, we just replace the body of the method to a call to our patch method

        List<int> argumentIndices = [];

        foreach (var argument in patchMethod.Parameters)
        {
            if (argument.Name == "__instance")
            {
                argumentIndices.Add(0);
            }
            else
            {
                var found = false;
                for (var i = 0; i < methodBeingPatched.Parameters.Count; i++)
                {
                    if (methodBeingPatched.Parameters[i].Name != argument.Name) continue;
                    found = true;
                    argumentIndices.Add(i);
                    break;
                }
                if (found) continue;
                Premonition.LogSource.LogError(
                    $"Error trampolining {methodBeingPatched.FullName} with {patchMethod.FullName}, unknown argument: {argument.Name}");
                return;
            }
        }

        var patchMethodInModule = Import(methodBeingPatched, patchMethod);
        if (patchMethodInModule == null) return;
        
        methodBeingPatched.Body.Instructions.Clear();
        foreach (var argumentIndex in argumentIndices)
        {
            switch (argumentIndex)
            {
                case 0:
                    methodBeingPatched.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
                    break;
                case 1:
                    methodBeingPatched.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_1));
                    break;
                case 2:
                    methodBeingPatched.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_2));
                    break;
                case 3:
                    methodBeingPatched.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_3));
                    break;
                default:
                {
                    methodBeingPatched.Body.Instructions.Add(argumentIndex < 256
                        ? Instruction.Create(OpCodes.Ldarg_S, methodBeingPatched.Parameters[argumentIndex])
                        : Instruction.Create(OpCodes.Ldarg, methodBeingPatched.Parameters[argumentIndex]));
                    break;
                }
            }
        }

        methodBeingPatched.Body.Instructions.Add(Instruction.Create(OpCodes.Call, patchMethodInModule));
        methodBeingPatched.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
    }
}
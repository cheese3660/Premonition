using System.Reflection;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using Mono.Collections.Generic;
using MonoMod.Utils;
using Premonition.Core.Utility;

namespace Premonition.Core;

/// <summary>
/// A descriptor for a patcher
/// </summary>
/// <param name="assembly">The assembly being patched</param>
/// <param name="typeName">The type being patched</param>
/// <param name="methodName">The method being patched</param>
/// <param name="argumentTypes">The optional argument types to match for the method being patched</param>
/// <param name="patchType">The type of patch</param>
/// <param name="patchMethod">The method being used to patch with</param>
public class PremonitionPatcher(
    string assembly,
    string typeName,
    string methodName,
    IReadOnlyList<string>? argumentTypes,
    PatchType patchType,
    MethodReference patchMethod)
{
    /// <summary>
    /// The assembly being patched
    /// </summary>
    public string Assembly => assembly;

    /// <summary>
    /// Patches the method in the target assembly
    /// </summary>
    /// <param name="definition">The assembly being referred to</param>
    /// <returns>If this patcher has completed its purpose (successfully or not)</returns>
    public bool Patch(AssemblyDefinition definition)
    {
        if (definition.Name.Name != assembly) return false;
        var type = definition.Modules.SelectMany(x => x.GetTypes())
            .FirstOrDefault(x => x.FullName == typeName);
        if (type == null)
        {
            LogError(
                $"Error patching {assembly}:{typeName}:{methodName} with method {patchMethod.FullName}, type does not exist!");
            return true;
        }

        var possibleMethods = type.GetMethods().Where(method => method.Name == methodName).ToList();
        if (possibleMethods.Count == 0)
        {
            LogError(
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
                LogError(
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
                LogError(
                    $"Error patching {assembly}:{typeName}:{methodName} with method {patchMethod.FullName}, method overload does not exist with specified arguments!");
                return true;
            }

            if (selectedMethods.Count > 1)
            {
                LogError(
                    $"Error patching {assembly}:{typeName}:{methodName} with method {patchMethod.FullName}, ambiguous method reference! (Possible methods are as follows)");
                foreach (var selectedMethod in selectedMethods)
                {
                    LogInfo(
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
                LogError(
                    $"Error patching {assembly}:{typeName}:{methodName} with method {patchMethod.FullName}, ambiguous method reference! (Possible methods are as follows)");
                foreach (var possibleMethod in possibleMethods)
                {
                    LogInfo(
                        $"{possibleMethod.FullName}{(possibleMethod.GenericParameters.Count > 0 ? $"<{string.Join(", ", possibleMethod.GenericParameters.Select(x => x.FullName))}>" : "")}({string.Join(", ", possibleMethod.Parameters.Select(x => x.ParameterType.FullName))})");
                }

                return true;
            }

            method = possibleMethods.First();
        }
        Patch(patchType, method, patchMethod);
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
                LogError(
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
                LogError($"Error patching {methodBeingPatched.FullName} with {patchMethod.FullName}, the method being patched has generic parameters while the patch method does not");
                return null;
            }

            if (patchMethod.GenericParameters.Count != methodBeingPatched.GenericParameters.Count)
            {
                LogError(
                    $"Error patching {methodBeingPatched.FullName} with {patchMethod.FullName}, mismatched amount of generic parameters");
                return null;
            }

            try
            {
                patchMethodInModule = patchMethodInModule.MakeGeneric(methodBeingPatched.GenericParameters.ToArray());
            }
            catch (Exception e)
            {
                LogError(
                    $"Error patching {methodBeingPatched.FullName} with {patchMethod.FullName}, mismatched amount of generic {e}");
                return null;
            }
        } else if (patchMethod.HasGenericParameters)
        {
            LogError($"Error patching {methodBeingPatched.FullName} with {patchMethod.FullName}, the patch method has generic parameters while the method being patched does not");
            return null;
        }
        return patchMethodInModule;
    }

    private static void PrefixPatch(MethodDefinition methodBeingPatched, MethodReference patchMethod)
    {
        List<int> argumentIndices = [];

        var patchMethodInModule = Import(methodBeingPatched, patchMethod);
        if (patchMethodInModule == null) return;
        var argIndex = 0;
        foreach (var argument in patchMethod.Parameters)
        {
            var argumentInModule = patchMethodInModule.Parameters[argIndex];
            // ReSharper disable once ConvertIfStatementToSwitchStatement
            if (argument.Name == "__instance")
            {
                argumentIndices.Add(0);
            }
            else if (argument.Name == "__retVal")
            {
                if (!argument.IsOut)
                {
                    LogError(
                        $"Error patching {methodBeingPatched.FullName} with {patchMethod.FullName}, __retVal must be an out parameter for prefix methods");
                    return;
                }

                if (patchMethodInModule.ReturnType.FullName != TypeConstants.Boolean)
                {
                    LogError(
                        $"Error patching {methodBeingPatched.FullName} with {patchMethod.FullName}, prefix methods with a __retVal parameter must return System.Boolean");
                    return;
                }

                if (methodBeingPatched.ReturnType.FullName != argumentInModule.ParameterType.FullName)
                {
                    LogError(
                        $"Error patching {methodBeingPatched.FullName} with {patchMethod.FullName}, __retVal much match the return type of the method being patched");
                    return;
                    
                }
                argumentIndices.Add(-1);
            }
            else
            {
                var found = false;
                for (var i = 0; i < methodBeingPatched.Parameters.Count; i++)
                {
                    if (methodBeingPatched.Parameters[i].Name != argument.Name) continue;
                    found = true;
                    argumentIndices.Add(methodBeingPatched.IsStatic ? i : i+1);
                    break;
                }
                if (found) continue;
                LogError(
                    $"Error patching {methodBeingPatched.FullName} with {patchMethod.FullName}, unknown argument: {argument.Name}");
                return;
            }

            argIndex++;
        }

        switch (patchMethodInModule.ReturnType.FullName)
        {
            case TypeConstants.Boolean:
                PrefixPatchConditional(methodBeingPatched,patchMethodInModule,argumentIndices);
                break;
            case TypeConstants.Void:
                PrefixPatchAlways(methodBeingPatched,patchMethodInModule,argumentIndices);
                break;
            default:
                LogError(
                    $"Error patching {methodBeingPatched.FullName} with {patchMethod.FullName}, invalid return type: {patchMethodInModule.ReturnType.FullName}, expected System.Boolean or System.Void");
                break;
        }
    }

    private static void PrefixPatchAlways(MethodDefinition methodBeingPatched, MethodReference patchMethodInModule,
        List<int> argumentIndices)
    {
        if (patchMethodInModule.Parameters.Count >= methodBeingPatched.Body.MaxStackSize)
        {
            methodBeingPatched.Body.MaxStackSize += patchMethodInModule.Parameters.Count;
        }
        List<Instruction> prefixInstructions = [];
        foreach (var argumentIndex in argumentIndices)
        {
            switch (argumentIndex)
            {
                case 0:
                    prefixInstructions.Add(Instruction.Create(OpCodes.Ldarg_0));
                    break;
                case 1:
                    prefixInstructions.Add(Instruction.Create(OpCodes.Ldarg_1));
                    break;
                case 2:
                    prefixInstructions.Add(Instruction.Create(OpCodes.Ldarg_2));
                    break;
                case 3:
                    prefixInstructions.Add(Instruction.Create(OpCodes.Ldarg_3));
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

        prefixInstructions.Add(Instruction.Create(OpCodes.Call, patchMethodInModule));
        
        methodBeingPatched.Body.Instructions.InsertRange(0, prefixInstructions);
        LogInfo(
            $"Successfully patched {methodBeingPatched.FullName} with {patchMethodInModule.FullName}");
    }

    private static void PrefixPatchConditional(MethodDefinition methodBeingPatched, MethodReference patchMethodInModule,
        List<int> argumentIndices)
    {
        VariableDefinition? lastVariable = null;
        var lastIndex = 0;
        if (methodBeingPatched.ReturnType.FullName != TypeConstants.Void)
        {
            if (argumentIndices.All(x => x != -1))
            {
                LogError(
                    $"Error patching {methodBeingPatched.FullName} with {patchMethodInModule.FullName}, a __retVal out parameter is required for conditional prefix patches on methods with return types other than System.Void");
                return;
            }

            methodBeingPatched.Body.Variables.Add(new VariableDefinition(methodBeingPatched.ReturnType));
            lastVariable = methodBeingPatched.Body.Variables.Last();
            lastIndex = methodBeingPatched.Body.Variables.Count - 1;
        }
        if (patchMethodInModule.Parameters.Count >= methodBeingPatched.Body.MaxStackSize)
        {
            methodBeingPatched.Body.MaxStackSize += patchMethodInModule.Parameters.Count;
        }
        List<Instruction> prefixInstructions = [];
        foreach (var argumentIndex in argumentIndices)
        {
            switch (argumentIndex)
            {
                case -1:
                    prefixInstructions.Add(lastIndex < 256
                        ? Instruction.Create(OpCodes.Ldloca_S, lastVariable)
                        : Instruction.Create(OpCodes.Ldloca, lastVariable));
                    break;
                case 0:
                    prefixInstructions.Add(Instruction.Create(OpCodes.Ldarg_0));
                    break;
                case 1:
                    prefixInstructions.Add(Instruction.Create(OpCodes.Ldarg_1));
                    break;
                case 2:
                    prefixInstructions.Add(Instruction.Create(OpCodes.Ldarg_2));
                    break;
                case 3:
                    prefixInstructions.Add(Instruction.Create(OpCodes.Ldarg_3));
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

        prefixInstructions.Add(Instruction.Create(OpCodes.Call, patchMethodInModule));
        var firstInstruction = methodBeingPatched.Body.Instructions[0];
        prefixInstructions.Add(Instruction.Create(OpCodes.Brtrue_S, firstInstruction));
        // methodBeingPatched.Body.Variables.Add(
        //     new VariableDefinition(methodBeingPatched.Module.ImportReference(typeof(bool))));
        if (methodBeingPatched.ReturnType.FullName == TypeConstants.Void)
        {
            prefixInstructions.Add(Instruction.Create(OpCodes.Ret));
        }
        else
        {
            switch (lastIndex)
            {
                case 0:
                    prefixInstructions.Add(Instruction.Create(OpCodes.Ldloc_0));
                    break;
                
                case 1:
                    prefixInstructions.Add(Instruction.Create(OpCodes.Ldloc_1));
                    break;
                
                case 2:
                    prefixInstructions.Add(Instruction.Create(OpCodes.Ldloc_2));
                    break;
                case 3:
                    prefixInstructions.Add(Instruction.Create(OpCodes.Ldloc_3));
                    break;
                default:
                    methodBeingPatched.Body.Instructions.Add(
                        Instruction.Create(lastIndex < 256 ? OpCodes.Ldloc_S : OpCodes.Ldloc, lastVariable));
                    break;
            }
            prefixInstructions.Add(Instruction.Create(OpCodes.Ret));
        }
        
        methodBeingPatched.Body.Instructions.InsertRange(0, prefixInstructions);
        LogInfo(
            $"Successfully patched {methodBeingPatched.FullName} with {patchMethodInModule.FullName}");
    }
    
    private static void PostfixPatch(MethodDefinition methodBeingPatched, MethodReference patchMethod)
    {
        var patchMethodInModule = Import(methodBeingPatched, patchMethod);
        if (patchMethodInModule == null) return;
        
        if (patchMethodInModule.ReturnType.FullName == TypeConstants.Void)
        {
            PostfixPatchVoid(methodBeingPatched, patchMethodInModule,patchMethod);
        }
        else
        {
            if (patchMethodInModule.ReturnType.FullName != methodBeingPatched.ReturnType.FullName)
            {
                LogError(
                    $"Error patching {methodBeingPatched.FullName} with {patchMethodInModule.FullName}, mismatched return types");
                return;
            }

            if (patchMethod.Parameters.Count == 0 || patchMethod.Parameters[0].Name != "__retVal")
            {
                PostfixPatchTailCall(methodBeingPatched, patchMethodInModule,patchMethod,true);
            }
            else if (patchMethodInModule.Parameters[0].ParameterType.FullName == methodBeingPatched.ReturnType.FullName)
            {
                PostfixPatchTailCall(methodBeingPatched, patchMethodInModule,patchMethod);
            }
            else
            {
                LogError(
                    $"Error patching {methodBeingPatched.FullName} with {patchMethodInModule.FullName}, type of __retVal argument does not match the return type of the method being patched");
            }
        }
    }

    private static void RetargetSwitch(Instruction inst, Instruction replace, Instruction target)
    {
        var operands = (Instruction[])inst.Operand;
        foreach (var instruction in operands)
        {
            if (instruction.Operand == replace)
            {
                instruction.Operand = target;
            }
        }
    }
    
    private static Instruction RetargetBranch(Instruction branch, Instruction target, int sizeDifference)
    {
        if (branch.OpCode.OperandType is OperandType.ShortInlineBrTarget && sizeDifference >= 100)
        {
            switch (branch.OpCode.Code)
            {
                case Code.Br_S:
                    return Instruction.Create(OpCodes.Br, target);
                case Code.Brtrue_S:
                    return Instruction.Create(OpCodes.Brtrue, target);
                case Code.Brfalse_S:
                    return Instruction.Create(OpCodes.Brfalse, target);
                case Code.Bge_S:
                    return Instruction.Create(OpCodes.Bge, target);
                case Code.Bgt_S:
                    return Instruction.Create(OpCodes.Bgt, target);
                case Code.Ble_S:
                    return Instruction.Create(OpCodes.Ble, target);
                case Code.Blt_S:
                    return Instruction.Create(OpCodes.Blt, target);
                case Code.Beq_S:
                    return Instruction.Create(OpCodes.Beq, target);
                case Code.Bne_Un_S:
                    return Instruction.Create(OpCodes.Bne_Un, target);
                case Code.Bge_Un_S:
                    return Instruction.Create(OpCodes.Bge_Un, target);
                case Code.Bgt_Un_S:
                    return Instruction.Create(OpCodes.Bgt_Un, target);
                case Code.Ble_Un_S:
                    return Instruction.Create(OpCodes.Ble_Un, target);
                case Code.Blt_Un_S:
                    return Instruction.Create(OpCodes.Blt_Un, target);
                case Code.Leave_S:
                    return Instruction.Create(OpCodes.Leave, target);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        branch.Operand = target;
        return branch;
    }


    private static Instruction InstructionConstructor(OpCode code, object operand)
    {
        return (Instruction)Activator.CreateInstance(typeof(Instruction), BindingFlags.NonPublic | BindingFlags.Instance, null, [code, operand], null);
    }
    
    private static IEnumerator<Instruction> ReplaceReturnInstructions(IEnumerable<Instruction> body,List<Instruction> replacement)
    {
        List<int> branches = [];
        List<Instruction> switches = [];
        List<Instruction> result = [];
        var index = 0;
        
        foreach (var instruction in body)
        {
            if (instruction.OpCode.FlowControl is FlowControl.Branch or FlowControl.Cond_Branch)
            {
                if (instruction.OpCode == OpCodes.Switch)
                {
                    var targets = (instruction.Operand as Instruction[])!;
                    if (targets.Any(target => target.OpCode == OpCodes.Ret))
                    {
                        switches.Add(instruction);
                    }
                }
                else
                {
                    var target = (instruction.Operand as Instruction)!;
                    if (target.OpCode == OpCodes.Ret)
                    {
                        branches.Add(index);
                    }
                }
            }
            
            if (instruction.OpCode != OpCodes.Ret)
            {
                result.Add(instruction);
                index += 1;
                continue;
            }

            var first = true;
            foreach (var newInst in replacement.Select(replacementInstruction => InstructionConstructor(replacementInstruction.OpCode, replacementInstruction.Operand)))
            {
                if (first)
                {
                    foreach (var sw in switches)
                    {
                        RetargetSwitch(sw, instruction, newInst);
                    }

                    foreach (var idx in branches)
                    {
                        var branch = result[idx];
                        if (branch.Operand != instruction) continue;
                        var size = 0;
                        for (var i = idx; i < index; i++)
                        {
                            size += result[i].GetSize();
                        }
                        result[idx] = RetargetBranch(branch, newInst, size);
                    }
                    first = false;
                }
                // this should be perfectly safe, but we need to clone the instruction, there should be no case where this is a branch instruction for example
                result.Add(newInst);
                index += 1;
            }
            result.Add(instruction);
            index += 1;
        }

        foreach (var instruction in result)
        {
            yield return instruction;
        }
    }
    
    private static void PostfixPatchTailCall(MethodDefinition methodBeingPatched, MethodReference patchMethodInModule, MethodReference patchMethod, bool drop=false)
    {
        
        // We already know with this one that the first argument should be the return value, so lets get every other argument
        List<int> argumentIndices = [];

        foreach (var argument in patchMethod.Parameters.Skip(1))
        {
            switch (argument.Name)
            {
                case "__instance":
                    argumentIndices.Add(0);
                    break;
                case "__retVal":
                    LogError(
                        $"Error patching {methodBeingPatched.FullName} with {patchMethod.FullName}, __retVal must be the first argument if it is used");
                    return;
                default:
                {
                    var found = false;
                    for (var i = 0; i < methodBeingPatched.Parameters.Count; i++)
                    {
                        if (methodBeingPatched.Parameters[i].Name != argument.Name) continue;
                        found = true;
                        argumentIndices.Add(methodBeingPatched.IsStatic ? i : i+1);
                        break;
                    }
                    if (found) continue;
                    LogError(
                        $"Error patching {methodBeingPatched.FullName} with {patchMethod.FullName}, unknown argument: {argument.Name}");
                    return;
                }
            }
        }

        if (patchMethodInModule.Parameters.Count >= methodBeingPatched.Body.MaxStackSize)
        {
            methodBeingPatched.Body.MaxStackSize += patchMethodInModule.Parameters.Count;
        }
        
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
        LogInfo(
            $"Successfully patched {methodBeingPatched.FullName} with {patchMethodInModule.FullName}");
    }

    private static void PostfixPatchVoid(MethodDefinition methodBeingPatched, MethodReference patchMethodInModule, MethodReference patchMethod)
    {
        
        List<int> argumentIndices = [];
        var first = true;
        var argIndex = 0;
        foreach (var argument in patchMethod.Parameters)
        {
            var inModuleArgument = patchMethodInModule.Parameters[argIndex];
            switch (argument.Name)
            {
                case "__instance":
                    argumentIndices.Add(0);
                    break;
                case "__retVal" when first:
                {
                    if (inModuleArgument.ParameterType.FullName != methodBeingPatched.ReturnType.FullName)
                    {
                        LogError(
                            $"Error patching {methodBeingPatched.FullName} with {patchMethod.FullName}, type of __retVal argument does not match the return type of the method being patched");
                        return;
                    }
                    argumentIndices.Add(-1);
                    break;
                }
                case "__retVal":
                    LogError(
                        $"Error patching {methodBeingPatched.FullName} with {patchMethod.FullName}, __retVal must be the first argument if it is used");
                    return;
                default:
                {
                    var found = false;
                    for (var i = 0; i < methodBeingPatched.Parameters.Count; i++)
                    {
                        if (methodBeingPatched.Parameters[i].Name != argument.Name) continue;
                        found = true;
                        argumentIndices.Add(methodBeingPatched.IsStatic ? i : i+1);
                        break;
                    }
                    if (found) continue;
                    LogError(
                        $"Error patching {methodBeingPatched.FullName} with {patchMethodInModule.FullName}, unknown argument: {argument.Name}");
                    return;
                }
            }
            first = false;
            argIndex += 1;
        }
        
        if (patchMethodInModule.Parameters.Count >= methodBeingPatched.Body.MaxStackSize)
        {
            methodBeingPatched.Body.MaxStackSize += patchMethodInModule.Parameters.Count;
        }

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
        LogInfo(
            $"Successfully patched {methodBeingPatched.FullName} with {patchMethodInModule.FullName}");
    }
    
    private static void TrampolinePatch(MethodDefinition methodBeingPatched, MethodReference patchMethod)
    {
        // This is the simplest style of patch, we just replace the body of the method to a call to our patch method

        List<int> argumentIndices = [];

        var patchMethodInModule = Import(methodBeingPatched, patchMethod);
        if (patchMethodInModule == null) return;
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
                    argumentIndices.Add(methodBeingPatched.IsStatic ? i : i+1);
                    break;
                }
                if (found) continue;
                LogError(
                    $"Error patching {methodBeingPatched.FullName} with {patchMethod.FullName}, unknown argument: {argument.Name}");
                return;
            }
        }

        if (patchMethodInModule.Parameters.Count >= methodBeingPatched.Body.MaxStackSize)
        {
            methodBeingPatched.Body.MaxStackSize += patchMethodInModule.Parameters.Count;
        }
        
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
        
        // methodBeingPatched.Dump();
        
        LogInfo(
            $"Successfully patched {methodBeingPatched.FullName} with {patchMethod.FullName}");
    }
}
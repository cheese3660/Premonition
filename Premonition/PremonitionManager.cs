using Mono.Cecil;

namespace Premonition;

internal class PremonitionManager
{


    internal void ReadAssembly(string dllPath)
    {
        var assemblyDefinition = AssemblyDefinition.ReadAssembly(dllPath);
        RegisterAssembly(assemblyDefinition);
    }

    private void RegisterAssembly(AssemblyDefinition definition)
    {
        
    }
}
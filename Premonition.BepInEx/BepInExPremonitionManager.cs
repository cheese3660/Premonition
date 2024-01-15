using System.Reflection;
using BepInEx;
using Mono.Cecil;
using Premonition.Core;
using Premonition.Core.Utility;

namespace Premonition.BepInEx;

internal class BepInExPremonitionManager
{
    private static IAssemblyResolver GetResolver()
    {
        var resolver = new DefaultAssemblyResolver();
        HashSet<string> searchDirectories = [];
        foreach (var dll in Directory.EnumerateFiles(Paths.GameRootPath, "*.dll", SearchOption.AllDirectories))
        {
            var dllPath = new FileInfo(dll).Directory!.FullName;
            searchDirectories.Add(dllPath);
        }
        foreach (var directory in searchDirectories)
        {
            resolver.AddSearchDirectory(directory);
        }
        return resolver;
    }
    private PremonitionManager? _manager;
    private PremonitionManager Manager => _manager ??= new PremonitionManager(GetResolver());

    // ReSharper disable once InconsistentNaming
    internal HashSet<string>? TargetDLLs;
    
    internal void RegisterRuntimePremonition()
    {
        
        Logging.Listeners.Add(ILogListener.CreateListener(PremonitionEntrypoint.LogSource.LogDebug, PremonitionEntrypoint.LogSource.LogInfo, PremonitionEntrypoint.LogSource.LogWarning,
            PremonitionEntrypoint.LogSource.LogError));
        TargetDLLs = [];
        var searchPaths = PremonitionEntrypoint.ModPaths.Value!;
        foreach (var dll in searchPaths.Where(Directory.Exists).SelectMany(folder => Directory.EnumerateFiles(folder,"*.dll",SearchOption.AllDirectories)))
        {
            Manager.ReadAssembly(dll);
        }

        foreach (var patcher in Manager.PremonitionPatchers.Select(x => x.Assembly + ".dll"))
        {
            TargetDLLs.Add(patcher);
        }
    }
    internal void Patch(AssemblyDefinition def)
    {
        Manager.Patch(def);
    }
}
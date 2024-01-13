using Mono.Cecil;
using Premonition.Core;
using Premonition.Core.Utility;

namespace Premonition.BepInEx;

internal class BepInExPremonitionManager
{
    private PremonitionManager? _manager;
    private PremonitionManager Manager => _manager ??= new PremonitionManager();

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
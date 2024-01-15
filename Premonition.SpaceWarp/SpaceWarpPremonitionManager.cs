using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using Mono.Cecil;
using Premonition.Core;
using Premonition.Core.Utility;
using ILogListener = Premonition.Core.Utility.ILogListener;

namespace Premonition.SpaceWarp;

internal class SpaceWarpPremonitionManager
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
    
    private readonly PremonitionManager _premonitionManager = new(GetResolver());

    internal IEnumerable<string> ToPatch =>
        _premonitionManager.PremonitionPatchers.Select(x => x.Assembly + ".dll");

    internal ManualLogSource LogSource;


    internal SpaceWarpPremonitionManager()
    {
        LogSource = Logger.CreateLogSource("Premonition.SpaceWarp");
        Logging.Listeners.Add(ILogListener.CreateListener(LogSource.LogDebug, LogSource.LogInfo, LogSource.LogWarning,
            LogSource.LogError));
    }

    internal void Read(string dll)
    {
        _premonitionManager.ReadAssembly(dll);
    }

    internal void Apply(AssemblyDefinition definition)
    {
        _premonitionManager.Patch(definition);
    }
}
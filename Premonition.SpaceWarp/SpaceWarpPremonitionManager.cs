using BepInEx.Logging;
using Mono.Cecil;
using Premonition.Core;
using Premonition.Core.Utility;
using ILogListener = Premonition.Core.Utility.ILogListener;

namespace Premonition.SpaceWarp;

internal class SpaceWarpPremonitionManager
{
    private PremonitionManager _premonitionManager = new();

    internal IEnumerable<string> ToPatch =>
        _premonitionManager.PremonitionPatchers.Select(x => x.Assembly + ".dll");

    internal SpaceWarpPremonitionManager()
    {
        var logSource = Logger.CreateLogSource("Premonition.SpaceWarp");
        Logging.Listeners.Add(ILogListener.CreateListener(logSource.LogDebug, logSource.LogInfo, logSource.LogWarning,
            logSource.LogError));
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
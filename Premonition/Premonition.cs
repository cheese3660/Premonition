using System;
using JetBrains.Annotations;
using Mono.Cecil;
using BepInEx.Configuration;
using BepInEx.Logging;

namespace Premonition;

[UsedImplicitly]
public static class Premonition
{
    private static ConfigFile? _premonitionConfiguration;

    private static ManualLogSource? _logSource;
    internal static ManualLogSource LogSource => _logSource ??= Logger.CreateLogSource("Premonition");
    
    private static ConfigFile PremonitionConfiguration => _premonitionConfiguration ??=
        new ConfigFile(BepInEx.Paths.ConfigPath + "/premonition.cfg", true);

    private static ConfigEntry<List<string>>? _modPaths;

    private static ConfigEntry<List<string>> ModPaths => _modPaths ??= PremonitionConfiguration.Bind("Runtime",
        "Runtime DLL folders", new List<string> {BepInEx.Paths.PluginPath, BepInEx.Paths.GameRootPath + "/GameData/Mods"});

    private static ConfigEntry<bool>? _respectDisabledModsList;

    private static ConfigEntry<bool> RespectDisabledModsList => _respectDisabledModsList ??=
        PremonitionConfiguration.Bind("Preload", "Respect Disabled Mods List (When SpaceWarp is installed)", true);

    private static PremonitionManager? _manager;
    private static PremonitionManager Manager => _manager ??= new PremonitionManager();
    
    private static void RegisterRuntimePremonition()
    {
        _targetDLLs = [];
        var searchPaths = ModPaths.Value!;
        foreach (var dll in searchPaths.Where(Directory.Exists).SelectMany(folder => Directory.EnumerateFiles(folder,"*.dll",SearchOption.AllDirectories)))
        {
            Manager.ReadAssembly(dll);
        }

        foreach (var patcher in Manager.PremonitionPatchers.Select(x => x.Assembly + ".dll"))
        {
            _targetDLLs.Add(patcher);
        }
    }
    
    
    // ReSharper disable once InconsistentNaming
    private static HashSet<string>? _targetDLLs;

    [UsedImplicitly]
    // ReSharper disable once InconsistentNaming
    public static IEnumerable<string> TargetDLLs
    {
        get
        {
            if (_targetDLLs == null)
            {
                RegisterRuntimePremonition();
            }

            return _targetDLLs!;
        }
    }

    [UsedImplicitly]
    public static void Patch(ref AssemblyDefinition definition)
    {
        Manager.Patch(definition);
    }
}
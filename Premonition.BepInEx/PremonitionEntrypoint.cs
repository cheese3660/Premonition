using System;
using System.Reflection;
using JetBrains.Annotations;
using Mono.Cecil;
using BepInEx.Configuration;
using BepInEx.Logging;

namespace Premonition.BepInEx;

[UsedImplicitly]
public static class PremonitionEntrypoint
{
    static PremonitionEntrypoint()
    {
        Assembly.LoadFile($"{new FileInfo(typeof(PremonitionEntrypoint).Assembly.Location).Directory!.FullName}\\Premonition.Core.dll");
    }
    
    private static ConfigFile? _premonitionConfiguration;

    private static ManualLogSource? _logSource;
    internal static ManualLogSource LogSource => _logSource ??= Logger.CreateLogSource("Premonition.BepInEx");
    
    private static ConfigFile PremonitionConfiguration => _premonitionConfiguration ??=
        new ConfigFile(global::BepInEx.Paths.ConfigPath + "/premonition.cfg", true);

    private static ConfigEntry<List<string>>? _modPaths;

    internal static ConfigEntry<List<string>> ModPaths => _modPaths ??= PremonitionConfiguration.Bind("Runtime",
        "Runtime DLL folders", new List<string> {global::BepInEx.Paths.PluginPath});

    private static ConfigEntry<bool>? _respectDisabledModsList;

    private static ConfigEntry<bool> RespectDisabledModsList => _respectDisabledModsList ??=
        PremonitionConfiguration.Bind("Preload", "Respect Disabled Mods List (When SpaceWarp is installed)", true);

    private static BepInExPremonitionManager? _bepInExPremonitionManager;

    private static BepInExPremonitionManager BepInExPremonitionManager =>
        _bepInExPremonitionManager ??= new BepInExPremonitionManager();
    
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
                BepInExPremonitionManager.RegisterRuntimePremonition();
                _targetDLLs = BepInExPremonitionManager.TargetDLLs;
            }

            return _targetDLLs!;
        }
    }

    [UsedImplicitly]
    public static void Patch(ref AssemblyDefinition definition)
    {
        BepInExPremonitionManager.Patch(definition);
    }
}
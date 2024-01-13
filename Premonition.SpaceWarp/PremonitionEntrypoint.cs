using System.Reflection;
using JetBrains.Annotations;
using Mono.Cecil;
using Newtonsoft.Json.Linq;
using SpaceWarp.Preload.API;

namespace Premonition.SpaceWarp;

[UsedImplicitly]
public class PremonitionEntrypoint : BasePatcher
{
    private SpaceWarpPremonitionManager _manager;
    public PremonitionEntrypoint()
    {
        Assembly.LoadFile($"{new FileInfo(GetType().Assembly.Location).Directory!.FullName}\\Premonition.dll");

        _manager = new SpaceWarpPremonitionManager();

        var disabledPluginGuids = File.ReadAllLines(CommonPaths.DisabledPluginsFilepath);
        
        var swinfoPaths = Directory
            .EnumerateFiles(
                CommonPaths.BepInExPluginsPath,
                "swinfo.json",
                SearchOption.AllDirectories
            )
            .ToList();
        if (Directory.Exists(CommonPaths.InternalModLoaderPath))
        {
            swinfoPaths.AddRange(
                Directory.EnumerateFiles(
                    CommonPaths.InternalModLoaderPath,
                    "swinfo.json",
                    SearchOption.AllDirectories
                )
            );
        }

        foreach (var swinfoPath in swinfoPaths)
        {
            try
            {
                var guid = GetGuidFromSwinfo(swinfoPath);
                if (disabledPluginGuids.Contains(guid))
                {
                    continue;
                }
                var modFolder = Path.GetDirectoryName(swinfoPath)!;
                var dlls = Directory.EnumerateFiles(
                    modFolder,
                    "*.dll",
                    SearchOption.AllDirectories
                );
                foreach (var dll in dlls)
                {
                    _manager.Read(dll);
                }
            }
            catch (Exception e)
            {
                // ignore
            }
        }
    }


    public override void ApplyPatch(ref AssemblyDefinition assembly) => _manager.Apply(assembly);
    public override IEnumerable<string> DLLsToPatch => _manager.ToPatch;
    
    
    private static string GetGuidFromSwinfo(string swinfoPath)
    {
        var swinfo = JObject.Parse(File.ReadAllText(swinfoPath));

        var guid = swinfo["mod_id"]?.Value<string>();
        if (guid == null)
        {
            throw new Exception($"{swinfoPath} does not contain a mod_id.");
        }
        return guid;
    }
}
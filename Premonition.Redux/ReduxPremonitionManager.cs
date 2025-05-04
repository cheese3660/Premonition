using System.Reflection;
using System.Text;
using Mono.Cecil;
using Premonition.Core;

namespace Premonition.Redux;

public class ReduxPremonitionManager
{
    public static ReduxPremonitionManager Instance { get; private set; } = null!;

    public readonly string GameRootPath;
    private readonly PremonitionManager _premonitionManager;
    private IAssemblyResolver _assemblyResolver;

    internal static ReduxPremonitionManager CreateInstance(string gameRootPath)
    {
        Instance = new ReduxPremonitionManager(gameRootPath);
        return Instance;
    }
    
    private ReduxPremonitionManager(string gameRootPath)
    {
        GameRootPath = gameRootPath;
        _assemblyResolver = GetResolver(gameRootPath);
        _premonitionManager = new PremonitionManager(_assemblyResolver);
    }
    
    private static IAssemblyResolver GetResolver(string gameRootPath)
    {
        var resolver = new DefaultAssemblyResolver();
        HashSet<string> searchDirectories = [];
        foreach (var dll in Directory.EnumerateFiles(gameRootPath, "*.dll", SearchOption.AllDirectories))
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

    internal StringBuilder Log = new();
    internal void RegisterAllMods()
    {
        foreach (var dll in Directory.EnumerateFiles(Path.Combine(GameRootPath,"mods"), "*.dll", SearchOption.AllDirectories))
        {
            try
            {
                _premonitionManager.ReadAssembly(dll);
            }
            catch (BadImageFormatException)
            {
                /* ignore */
            }
            catch (Exception e)
            {
                Log.AppendLine($"Error with reading: {dll}:\n\t{e}");
            }
        }
    }

    internal void PatchAllAssemblies()
    {
        // TODO: Caching patched assemblies?
        using var stream = new MemoryStream();
        foreach (var dll in Directory.EnumerateFiles(Path.Combine(GameRootPath), "*.dll",
                     SearchOption.AllDirectories))
        {
            try
            {
                var assembly = AssemblyDefinition.ReadAssembly(dll, new ReaderParameters
                {
                    AssemblyResolver = _assemblyResolver
                });
                if (!_premonitionManager.Patch(assembly)) continue;
                stream.Seek(0, SeekOrigin.Begin);
                stream.SetLength(0);
                assembly.Write(stream);
                var asm = Assembly.Load(stream.GetBuffer());
            }
            catch (BadImageFormatException)
            {
                /* ignore */
            }
            catch (Exception e)
            {
                Log.AppendLine($"Error with patching: {dll}:\n\t{e}");
            }
        }
    }
}
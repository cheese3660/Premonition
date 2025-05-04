using System;
using System.Reflection;
using System.Text;
using Premonition.Redux;

// using Premonition.Redux;

namespace Doorstop;
class Entrypoint
{
    
    
    public static void Start()
    {
        // File.WriteAllText("doorstop.log","Premonition pre-loaded correctly!");
        // This is where we start injecting premonition
        var assemblyPath = new FileInfo(Assembly.GetExecutingAssembly().Location);
        var rootPath = assemblyPath.Directory!.Parent!.Parent!.FullName;
        var manager = ReduxPremonitionManager.CreateInstance(rootPath);
        manager.RegisterAllMods();
        manager.PatchAllAssemblies();
        File.WriteAllText("premonition.log",manager.Log.ToString());
    }
}
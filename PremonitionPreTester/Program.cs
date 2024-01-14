// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using System.Runtime.InteropServices;
using Mono.Cecil;
using Premonition.Core;
using Premonition.Core.Utility;
using PremonitionTester;
using PremonitionTesters;
// using TesterDLL;

Logging.Listeners.Add(new PremonitionLogListener());

var manager = new PremonitionManager();

manager.ReadAssembly("DummyMod.dll");

using (var definition = AssemblyDefinition.ReadAssembly("DummyGame.dll"))
{
    Console.WriteLine(definition.Name.Name);
    manager.Patch(definition);
    definition.Write("PatchedDummyGame.dll");
}

File.Delete("DummyGame.dll");
Thread.Sleep(100);
File.Copy("PatchedDummyGame.dll", "DummyGame.dll");
Thread.Sleep(100);

using (var tester = new Process())
{
    tester.StartInfo.FileName = RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
        ? "PremonitionTester"
        : "PremonitionTester.exe";

    tester.StartInfo.UseShellExecute = false;
    tester.Start();
    while (!tester.HasExited)
    {
    }

    // Console.WriteLine(tester.StandardOutput.ReadToEnd());
    File.WriteAllText("result", tester.ExitCode == 0 ? "SUCCESS" : "FAILURE");
    return 0;
}


// return TestRunner.RunTests() ? 0 : 1;

// StaticMethods.StaticVoidMethodOne();
// Console.WriteLine($"StaticVoidMethodOneDummyValue = {StaticMethods.StaticVoidMethodOneDummyValue}");
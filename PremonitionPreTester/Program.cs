// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using Mono.Cecil;
using Premonition.Core;
using Premonition.Core.Utility;
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
File.Copy("PatchedDummyGame.dll", "DummyGame.Dll");


var processInfo = Process.Start("PremonitionTester.exe");

while (!processInfo.HasExited)
{
    // Spin
}

return processInfo.ExitCode;

// return TestRunner.RunTests() ? 0 : 1;

// StaticMethods.StaticVoidMethodOne();
// Console.WriteLine($"StaticVoidMethodOneDummyValue = {StaticMethods.StaticVoidMethodOneDummyValue}");
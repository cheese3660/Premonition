// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using System.Reflection;
using BepInEx.Logging;
using Mono.Cecil;
using Mono.Cecil.Rocks;
using Premonition;
using Premonition.Utility;
using PremonitionTesters;
// using TesterDLL;


Logger.Listeners.Add(new PremonitionLogListener());

var manager = new PremonitionManager();

manager.ReadAssembly("DummyMod.dll");

var definition = AssemblyDefinition.ReadAssembly("DummyGame.dll");


Console.WriteLine(definition.Name.Name);
manager.Patch(definition);
definition.Write("PatchedDummyGame.dll");


Process.Start("PremonitionInvoker.exe");


// return TestRunner.RunTests() ? 0 : 1;

// StaticMethods.StaticVoidMethodOne();
// Console.WriteLine($"StaticVoidMethodOneDummyValue = {StaticMethods.StaticVoidMethodOneDummyValue}");

using System.Diagnostics;

while (true)
{
    if (Process.GetProcessesByName("PremonitionPreTester").Length == 0)
    {
        Console.WriteLine("Setting up invoker!");
        File.Delete("DummyGame.dll");
        File.Copy("PatchedDummyGame.dll", "DummyGame.Dll");
        Process.Start("PremonitionTester.exe");
        break;
    }
    else
    {
        Thread.Sleep(500);
    }
}
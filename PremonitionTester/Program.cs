
using PremonitionTester;
// Assembly.LoadFile($"{new FileInfo(Assembly.GetExecutingAssembly().Location).Directory!.FullName}/PatchedDummyGame.dll");

var result = Tester.RunTests();

return result ? 0 : 1;
using System.Reflection;
using DummyGame;
using PremonitionTester.Attributes;
using PremonitionTester.Utilities;
using PremonitionTester.Utilities.Exceptions;

namespace PremonitionTester;

/// <summary>
/// A class used for running tests
/// </summary>
public static class Tester
{
    private static Dictionary<string, List<(string, Action)>>? _tests;

    private static Dictionary<string, List<(string, Action)>> Tests
    {
        get
        {
            if (_tests != null) return _tests;
            _tests = new Dictionary<string, List<(string, Action)>>();

            var types = typeof(Tester).Assembly.GetTypes()
                .Where(type => type is { IsPublic: true, IsAbstract: true, IsSealed: true }).Where(type =>
                    type.GetCustomAttributes(typeof(TestSectionAttribute), false).Length > 0);
            foreach (var type in types)
            {
                var section =
                    (type.GetCustomAttributes(typeof(TestSectionAttribute), false).First() as TestSectionAttribute)!
                    .SectionName;
                var sectionList = _tests[section] = [];
                foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Static).Where(info =>
                             info.ReturnType == typeof(void) && info.ContainsGenericParameters == false &&
                             info.GetParameters().Length == 0))
                {
                    var test = method.GetCustomAttributes(typeof(TestAttribute), false).FirstOrDefault();
                    if (test is TestAttribute testAttribute)
                    {
                        sectionList.Add((testAttribute.Name, (Action)Delegate.CreateDelegate(typeof(Action), method)));
                    }
                }
            }

            return _tests;
        }
    }

    private enum TestResult
    {
        Passed,
        Skipped,
        Failed
    }

    /// <summary>
    /// Run all tests
    /// </summary>
    /// <returns>true if all tests passed</returns>
    public static bool RunTests()
    {
        Console.WriteLine("---- Running all tests ----");
        var testsPassed = 0;
        var testsSkipped = 0;
        var testsFailed = 0;
        foreach (var (section, tests) in Tests)
        {
            var sectionTestsPassed = 0;
            var sectionTestsSkipped = 0;
            var sectionTestsFailed = 0;

            Console.ResetColor();
            Console.WriteLine(section);
            foreach (var (name, test) in tests)
            {
                Testing.ResetLog();
                Console.ResetColor();
                Console.WriteLine($"\t{name}");

                var result = TestResult.Passed;
                var reason = "";
                try
                {
                    test();
                }
                catch (TestPassedException)
                {
                    result = TestResult.Passed;
                }
                catch (TestSkippedException testSkippedException)
                {
                    result = TestResult.Skipped;
                    reason = testSkippedException.Message;
                }
                catch (TestFailedException testFailedException)
                {
                    result = TestResult.Failed;
                    reason = testFailedException.Message;
                }
                catch (Exception e)
                {
                    result = TestResult.Failed;
                    reason = e.ToString();
                }

                foreach (var log in Testing.TestLogOutput)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write("\t\t[LOG]");
                    Console.ResetColor();
                    Console.WriteLine($": {log}");
                }

                // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
                switch (result)
                {
                    case TestResult.Passed:
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("\t\t[PASSED]");
                        sectionTestsPassed += 1;
                        break;
                    case TestResult.Skipped:
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.Write("\t\t[SKIPPED]");
                        sectionTestsSkipped += 1;
                        Console.ResetColor();
                    {
                        var lines = reason.Split('\n');
                        if (lines.Length <= 1)
                        {
                            Console.WriteLine($": {lines[0]}");
                        }
                        else
                        {
                            Console.WriteLine("");
                            foreach (var line in lines)
                            {
                                Console.WriteLine($"\t\t\t{line}");
                            }
                        }
                    }
                        break;
                    case TestResult.Failed:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("\t\t[FAILED]");
                        sectionTestsFailed += 1;
                        Console.ResetColor();
                    {
                        var lines = reason.Split('\n');
                        if (lines.Length <= 1)
                        {
                            Console.WriteLine($": {lines[0]}");
                        }
                        else
                        {
                            Console.WriteLine("");
                            foreach (var line in lines)
                            {
                                Console.WriteLine($"\t\t\t{line}");
                            }
                        }
                    }
                        break;
                }
                Console.ResetColor();
            }
            Console.WriteLine("\t---- Section Summary ----");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("\t PASSED");
            Console.ResetColor();
            Console.WriteLine($": {sectionTestsPassed}");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("\tSKIPPED");
            Console.ResetColor();
            Console.WriteLine($": {sectionTestsSkipped}");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("\t FAILED");
            Console.ResetColor();
            Console.WriteLine($": {sectionTestsFailed}");
            testsPassed += sectionTestsPassed;
            testsSkipped += sectionTestsSkipped;
            testsFailed += sectionTestsFailed;
        }

        Console.ResetColor();
        Console.WriteLine("---- Test Summary ----");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write(" PASSED");
        Console.ResetColor();
        Console.WriteLine($": {testsPassed}");
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.Write("SKIPPED");
        Console.ResetColor();
        Console.WriteLine($": {testsSkipped}");
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write(" FAILED");
        Console.ResetColor();
        Console.WriteLine($": {testsSkipped}");


        return testsFailed == 0;
    }
}
using System.Reflection;
using Backbone.SpecFlowCucumberResultsExporter.Model;

namespace Backbone.SpecFlowCucumberResultsExporter.Extensions;

public static class Extensions
{
    public static TestResult GetResult(this IEnumerable<ReportItem> items)
    {
        return items.Select(x => x.Result).GetResult();
    }

    public static TestResult GetResult(this IEnumerable<TestResult> results)
    {
        var testResults = results as TestResult[] ?? results.ToArray();
        if (testResults.Any(x => x == TestResult.Failed))
        {
            return TestResult.Failed;
        }

        if (testResults.Any(x => x == TestResult.Pending))
        {
            return TestResult.Pending;
        }

        if (testResults.Any(x => x == TestResult.Skipped))
        {
            return TestResult.Skipped;
        }

        if (testResults.Any(x => x == TestResult.Undefined))
        {
            return TestResult.Undefined;
        }

        return TestResult.Passed;
    }

    internal static TestResult ToTestResult(this ScenarioExecutionStatus executionStatus)
    {
        switch (executionStatus)
        {
            case ScenarioExecutionStatus.OK:
                return TestResult.Passed;
            case ScenarioExecutionStatus.StepDefinitionPending:
                return TestResult.Pending;
            case ScenarioExecutionStatus.TestError:
                return TestResult.Failed;
            default:
                return TestResult.Undefined;
        }
    }

    internal static IEnumerable<string> GetPendingSteps(this ScenarioContext scenarioContext)
    {
        return typeof(ScenarioContext)
            .GetProperty("PendingSteps", BindingFlags.NonPublic | BindingFlags.Instance)!
            .GetValue(scenarioContext, null) as IEnumerable<string>
               ?? [];
    }

    internal static string ReplaceFirst(this string s, string find, string replace)
    {
        var first = s.IndexOf(find, StringComparison.Ordinal);
        return s[..first] + replace + s[(first + find.Length)..];
    }

    internal static string GetParamName(this MethodInfo method, int index)
    {
        var retVal = string.Empty;

        if (method != null && method.GetParameters().Length > index)
        {
            retVal = method.GetParameters()[index].Name;
        }


        return retVal;
    }

    internal static ExceptionInfo ToExceptionInfo(this Exception ex)
    {
        if (ex == null)
        {
            return null;
        }

        return new ExceptionInfo(ex);
    }
}

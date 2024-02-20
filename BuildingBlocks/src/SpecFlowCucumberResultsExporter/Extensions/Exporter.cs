using Backbone.SpecFlowCucumberResultsExporter.Reporting;

namespace Backbone.SpecFlowCucumberResultsExporter.Extensions;
public class Exporter : ReportingStepDefinitions
{
    public static void ExportToCucumber(string? path = default, string? fileName = default)
    {
        path ??= "../../../TestResults/";
        fileName ??= $"specflow_cucumber_{DateTimeOffset.UtcNow:yyyyMMdd}.json";

        Reporters.Add(new JsonReporter());

        Reporters.FinishedReport += (_, args) =>
        {
            var file = new FileInfo(Path.Combine(path, fileName));
            file.Directory?.Create();
            File.WriteAllText(file.FullName, args.Reporter.WriteToString());
        };
    }
}

using SpecFlowCucumberResultsExporter.Reporting;

namespace SpecFlowCucumberResultsExporter.Extensions;
public class Exporter : ReportingStepDefinitions
{
    public static void ExportToCucumber(string path = default, string fileName = default)
    {
        path ??= "../../../TestResults/";
        fileName ??= $"specflow_cucumber_{DateTimeOffset.UtcNow:yyyyMMdd}.json";

        Reporters.Add(new JsonReporter());

        Reporters.FinishedReport += (sender, args) =>
        {
            var file = new FileInfo(path + fileName);
            if (file != null)
            {
                file.Directory.Create();
                File.WriteAllText(file.FullName, args.Reporter.WriteToString());
            }
        };
    }
}

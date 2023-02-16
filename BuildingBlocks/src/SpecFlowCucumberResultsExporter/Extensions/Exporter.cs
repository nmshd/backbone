using SpecFlowCucumberResultsExporter.Reporting;

namespace SpecFlowCucumberResultsExporter.Extensions;
public class Exporter : ReportingStepDefinitions
{
    public static void ExportToCucumber(string path = default, string fileName = default)
    {
        path ??= "../../../TestResults/";
        fileName ??= $"specflow_cucumber_{DateTimeOffset.UtcNow:yyyyMMdd}";
        fileName += ".json";

        Reporters.Add(new JsonReporter());

        Reporters.FinishedReport += (sender, args) =>
        {
            var file = new System.IO.FileInfo(path);
            if (file != null)
            {
                file.Directory.Create();
                System.IO.File.WriteAllText(file.FullName, args.Reporter.WriteToString());
            }
        };
    }
}

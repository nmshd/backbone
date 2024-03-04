using Backbone.SpecFlowCucumberResultsExporter.Model;
using Backbone.SpecFlowCucumberResultsExporter.Reporting;

namespace Backbone.SpecFlowCucumberResultsExporter.EventArgs;

public class ReportEventArgs : System.EventArgs
{
    public ReportEventArgs(Reporter reporter)
    {
        Reporter = reporter;
        Report = reporter.Report;
    }

    public Reporter Reporter { get; internal set; }
    public Report Report { get; internal set; }
}

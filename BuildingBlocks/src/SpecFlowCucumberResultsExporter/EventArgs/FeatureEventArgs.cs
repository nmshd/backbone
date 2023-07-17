using SpecFlowCucumberResultsExporter.Model;
using SpecFlowCucumberResultsExporter.Reporting;

namespace SpecFlowCucumberResultsExporter.EventArgs;

public class FeatureEventArgs : ReportEventArgs
{
    public FeatureEventArgs(Reporter reporter)
        : base(reporter)
    {
        Feature = Reporter.CurrentFeature;
    }

    public Feature Feature { get; internal set; }
}

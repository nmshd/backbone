using Backbone.SpecFlowCucumberResultsExporter.Model;
using Backbone.SpecFlowCucumberResultsExporter.Reporting;

namespace Backbone.SpecFlowCucumberResultsExporter.EventArgs;

public class FeatureEventArgs : ReportEventArgs
{
    public FeatureEventArgs(Reporter reporter)
        : base(reporter)
    {
        Feature = Reporter.CurrentFeature;
    }

    public Feature Feature { get; internal set; }
}

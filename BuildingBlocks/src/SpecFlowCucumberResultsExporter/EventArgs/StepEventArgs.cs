using Backbone.SpecFlowCucumberResultsExporter.Model;
using Backbone.SpecFlowCucumberResultsExporter.Reporting;

namespace Backbone.SpecFlowCucumberResultsExporter.EventArgs;

public class StepEventArgs : ScenarioEventArgs
{
    public StepEventArgs(Reporter reporter)
        : base(reporter)
    {
        Step = reporter.CurrentStep;
    }

    public Step Step { get; internal set; }
}

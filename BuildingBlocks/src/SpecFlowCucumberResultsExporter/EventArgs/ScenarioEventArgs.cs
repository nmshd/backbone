using Backbone.SpecFlowCucumberResultsExporter.Model;
using Backbone.SpecFlowCucumberResultsExporter.Reporting;

namespace Backbone.SpecFlowCucumberResultsExporter.EventArgs;

public class ScenarioEventArgs : FeatureEventArgs
{
    public ScenarioEventArgs(Reporter reporter)
        : base(reporter)
    {
        Scenario = Reporter.CurrentScenario;
    }

    public Scenario Scenario { get; internal set; }
}

using SpecFlowCucumberResultsExporter.Model;
using SpecFlowCucumberResultsExporter.Reporting;

namespace SpecFlowCucumberResultsExporter.EventArgs;

public class ScenarioEventArgs : FeatureEventArgs
{
    public ScenarioEventArgs(Reporter reporter)
        : base(reporter)
    {
        Scenario = Reporter.CurrentScenario;
    }

    public Scenario Scenario { get; internal set; }
}
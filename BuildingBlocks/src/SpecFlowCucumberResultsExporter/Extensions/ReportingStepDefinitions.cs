using SpecFlowCucumberResultsExporter.Reporting;

namespace SpecFlowCucumberResultsExporter.Extensions
{
    public abstract class ReportingStepDefinitions : ContextBoundObject
    {
        public async Task ReportStep(ScenarioContext scenarioContext, Func<Task> stepFunc, params object[] args)
        {
            await Reporters.ExecuteStep(scenarioContext, stepFunc, args);
        }
    }
}

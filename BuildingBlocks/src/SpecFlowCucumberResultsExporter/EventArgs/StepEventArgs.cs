using SpecFlowCucumberResultsExporter.Model;
using SpecFlowCucumberResultsExporter.Reporting;

namespace SpecFlowCucumberResultsExporter.EventArgs
{
    public class StepEventArgs : ScenarioEventArgs
	{
		public StepEventArgs(Reporter reporter)
			: base(reporter)
		{
			Step = reporter.CurrentStep;
		}

		public Step Step { get; internal set; }
	}
}
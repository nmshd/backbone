using Backbone.SpecFlowCucumberResultsExporter.EventArgs;

namespace Backbone.SpecFlowCucumberResultsExporter.Reporting;

public partial class Reporters
{
    #region Events

    public static event EventHandler<ReportEventArgs> StartedReport;

    public static event EventHandler<ReportEventArgs> FinishedReport;

    public static event EventHandler<FeatureEventArgs> StartedFeature;

    public static event EventHandler<FeatureEventArgs> FinishedFeature;

    public static event EventHandler<ScenarioEventArgs> StartedScenario;

    public static event EventHandler<ScenarioEventArgs> FinishedScenario;

    public static event EventHandler<StepEventArgs> StartedStep;

    public static event EventHandler<StepEventArgs> FinishedStep;

    #endregion Events

    #region Event Raising

    internal static void OnStartedReport(Reporter reporter)
    {
        RaiseEvent(StartedReport, new ReportEventArgs(reporter));
    }

    internal static void OnFinishedReport(Reporter reporter)
    {
        RaiseEvent(FinishedReport, new ReportEventArgs(reporter));
    }

    internal static void OnStartedFeature(Reporter reporter)
    {
        RaiseEvent(StartedFeature, new FeatureEventArgs(reporter));
    }

    internal static void OnFinishedFeature(Reporter reporter)
    {
        RaiseEvent(FinishedFeature, new FeatureEventArgs(reporter));
    }

    internal static void OnStartedScenario(Reporter reporter)
    {
        RaiseEvent(StartedScenario, new ScenarioEventArgs(reporter));
    }

    internal static void OnFinishedScenario(Reporter reporter)
    {
        RaiseEvent(FinishedScenario, new ScenarioEventArgs(reporter));
    }

    internal static void OnStartedStep(Reporter reporter)
    {
        RaiseEvent(StartedStep, new StepEventArgs(reporter));
    }

    internal static void OnFinishedStep(Reporter reporter)
    {
        RaiseEvent(FinishedStep, new StepEventArgs(reporter));
    }

    private static void RaiseEvent<T>(EventHandler<T> handler, T args)
        where T : ReportEventArgs
    {
        handler?.Invoke(null, args);
    }

    #endregion Event Raising
}

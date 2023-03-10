﻿using SpecFlowCucumberResultsExporter.Extensions;
using SpecFlowCucumberResultsExporter.Model;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Bindings.Reflection;

namespace SpecFlowCucumberResultsExporter.Reporting;

public partial class Reporters
{
    private static bool _testrunIsFirstFeature;

    [AfterFeature]
    internal static void AfterFeature()
    {
        foreach (var reporter in reporters)
        {
            var feature = reporter.CurrentFeature;

            var scenarioOutlineGroups = feature.Elements.GroupBy(scenario => scenario.Name)
                .Where((scenarioGrp, key) => scenarioGrp.Count() > 1)
                .Select((scenarioGrp, key) => scenarioGrp.ToList());

            foreach (var scenarioOutlineGroup in scenarioOutlineGroups)
            {
                for (var i = 0; i < scenarioOutlineGroup.Count(); i++)
                {
                    scenarioOutlineGroup[i].Name = string.Format("{0} (example {1})", scenarioOutlineGroup[i].Name, i + 1);
                }
            }

            feature.Result = feature.Elements.Exists(o => o.Result == TestResult.Failed)
                ? TestResult.Failed
                : TestResult.Passed;

            feature.EndTime = CurrentRunTime;
            OnFinishedFeature(reporter);
            reporter.CurrentFeature = null;
        }
    }

    [AfterScenario]
    internal static void AfterScenario()
    {
        foreach (var reporter in reporters.ToArray())
        {
            var scenario = reporter.CurrentScenario;
            scenario.EndTime = CurrentRunTime;
            scenario.Result = scenario.Steps.Exists(o => o.Result.Status == TestResult.Failed)
                ? TestResult.Failed
                : TestResult.Passed;
            OnFinishedScenario(reporter);
            reporter.CurrentScenario = null;
        }
    }

    [AfterTestRun]
    internal static void AfterTestRun()
    {
        foreach (var reporter in reporters)
        {
            reporter.Report.EndTime = CurrentRunTime;
            OnFinishedReport(reporter);
        }
    }

    [AfterStep]
    internal static void AfterStep(ScenarioContext scenarioContext)
    {
        var endtime = CurrentRunTime;
        var result = scenarioContext.ScenarioExecutionStatus.ToTestResult();
        var error = scenarioContext.TestError?.ToExceptionInfo().Message;
        error = error == null && result == TestResult.Pending ? new PendingStepException().ToExceptionInfo().Message : string.Empty;

        foreach (var reporter in reporters.ToArray())
        {
            var step = reporter.CurrentStep;
            step.EndTime = CurrentRunTime;
            step.Result = new StepResult
            {
                Duration = (long)((endtime - reporter.CurrentStep.StartTime).TotalMilliseconds * 1000000),
                Status = result,
                Error = error
            };
            OnFinishedStep(reporter);
            reporter.CurrentStep = null;
        }
    }


    [BeforeFeature]
    internal static void BeforeFeature(FeatureContext featureContext)
    {
        var starttime = CurrentRunTime;

        // Init reports when the first feature runs. This is intentionally not done in
        // BeforeTestRun(), to make sure other [BeforeTestRun] annotated methods can perform
        // initialization before the reports are created.
        if (_testrunIsFirstFeature)
        {
            foreach (var reporter in reporters)
            {
                reporter.Report = new Report
                {
                    Features = new List<Feature>(),
                    StartTime = starttime
                };

                OnStartedReport(reporter);
            }

            _testrunIsFirstFeature = false;
        }

        foreach (var reporter in reporters)
        {
            var featureId = featureContext.FeatureInfo.Title.Replace(" ", "-").ToLower();
            var feature = new Feature
            {
                Tags = featureContext.FeatureInfo.Tags.Select(tag => new Tag() { Name = "@" + tag }).ToList(),
                Elements = new List<Scenario>(),
                StartTime = starttime,
                Name = featureContext.FeatureInfo.Title,
                Description = featureContext.FeatureInfo.Description,
                Id = featureId,
                Uri = $"/{featureId}"
            };

            reporter.Report.Features.Add(feature);
            reporter.CurrentFeature = feature;

            OnStartedFeature(reporter);
        }
    }

    [BeforeScenario]
    internal static void BeforeScenario(ScenarioContext scenarioContext)
    {
        var starttime = CurrentRunTime;

        foreach (var reporter in reporters)
        {
            var scenario = new Scenario
            {
                Tags = scenarioContext.ScenarioInfo.Tags.Select(tag => new Tag() { Name = "@" + tag }).ToList(),
                StartTime = starttime,
                Name = scenarioContext.ScenarioInfo.Title,
                Steps = new List<Step>(),
                Description = scenarioContext.ScenarioInfo.Title
            };

            reporter.CurrentFeature.Elements.Add(scenario);
            reporter.CurrentScenario = scenario;

            OnStartedScenario(reporter);
        }
    }

    [BeforeTestRun]
    internal static void BeforeTestRun()
    {
        _testrunIsFirstFeature = true;
    }

    [BeforeStep]
    internal static void BeforeStep(ScenarioContext scenarioContext)
    {
        var starttime = CurrentRunTime;
        var stepInfo = ScenarioStepContext.Current.StepInfo;
        var binding = stepInfo.BindingMatch.StepBinding as StepDefinitionBinding;
        var method = binding?.Method as RuntimeBindingMethod;

        foreach (var reporter in reporters)
        {
            var step = CreateStep(scenarioContext, starttime);

            reporter.CurrentScenario.Steps.Add(step);
            reporter.CurrentStep = step;

            OnStartedStep(reporter);
        }
    }

}
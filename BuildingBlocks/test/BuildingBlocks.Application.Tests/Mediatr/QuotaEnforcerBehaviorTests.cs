﻿using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using Enmeshed.BuildingBlocks.Application.Attributes;
using Enmeshed.BuildingBlocks.Application.MediatR;
using Enmeshed.BuildingBlocks.Domain;
using FluentAssertions;
using MediatR;
using Xunit;
using Enmeshed.UnitTestTools.Extensions;
using Enmeshed.UnitTestTools.Behaviors;
using Enmeshed.BuildingBlocks.Application.QuotaCheck;

namespace Enmeshed.BuildingBlocks.Application.Tests.Mediatr;

public class QuotaEnforcerBehaviorTests
{
    [Fact]
    public async void Calls_next_when_no_metric_is_exhausted()
    {
        // Arrange
        var behavior = CreateQuotaEnforcerBehavior();
        var nextMock = new NextMock<Unit>();

        // Act
        Func<Task> acting = async () => await behavior.Handle(
            new TestCommand(),
            nextMock.Value,
            CancellationToken.None);

        await Task.Run(acting, CancellationToken.None);

        // Assert
        nextMock.WasCalled.Should().BeTrue();
    }

    [Fact]
    public void Throws_QuotaExhaustedException_when_exactly_one_metric_is_exhausted()
    {
        var exhaustionDate = DateTime.UtcNow.AddDays(1);
        var exhaustedMetricStatus = new MetricStatus(new MetricKey("exhausted"), exhaustionDate);

        // Arrange
        var behavior = CreateQuotaEnforcerBehavior(exhaustedMetricStatuses: exhaustedMetricStatus);

        // Act
        Func<Task> acting = async () => await behavior.Handle(
            new TestCommand(),
            new NextMock<Unit>().Value,
            CancellationToken.None
        );

        // Assert
        var exceptionExhaustedMetrics =
            acting.Should().AwaitThrowAsync<QuotaExhaustedException>().Which.ExhaustedMetricStatuses;
        exceptionExhaustedMetrics.Should().HaveCount(1);
        exceptionExhaustedMetrics.First().MetricKey.Should().Be(new MetricKey("exhausted"));
        exceptionExhaustedMetrics.First().IsExhaustedUntil.Should().Be(exhaustionDate);
    }

    [Fact]
    public void Thrown_QuotaExhaustedException_contains_information_about_each_exhausted_MetricStatus()
    {
        // Arrange
        var exhaustionDate1 = DateTime.UtcNow.AddDays(1);
        var exhaustionDate2 = DateTime.UtcNow.AddDays(10);
        var exhaustedMetricStatus1 = new MetricStatus(new MetricKey("exhausted1"), exhaustionDate1);
        var exhaustedMetricStatus2 = new MetricStatus(new MetricKey("exhausted2"), exhaustionDate2);
        var behavior = CreateQuotaEnforcerBehavior(exhaustedMetricStatuses: new[]
            { exhaustedMetricStatus1, exhaustedMetricStatus2 }
        );

        // Act
        Func<Task> acting = async () => await behavior.Handle(
            new TestCommand(),
            new NextMock<Unit>().Value,
            CancellationToken.None);

        // Assert
        var exceptionExhaustedMetrics =
            acting.Should().AwaitThrowAsync<QuotaExhaustedException>().Which.ExhaustedMetricStatuses;
        exceptionExhaustedMetrics.Should().HaveCount(2);
        exceptionExhaustedMetrics.First().MetricKey.Should().Be(new MetricKey("exhausted1"));
        exceptionExhaustedMetrics.First().IsExhaustedUntil.Should().Be(exhaustionDate1);

        exceptionExhaustedMetrics.Second().MetricKey.Should().Be(new MetricKey("exhausted2"));
        exceptionExhaustedMetrics.Second().IsExhaustedUntil.Should().Be(exhaustionDate2);
    }

    private static QuotaEnforcerBehavior<TestCommand, Unit> CreateQuotaEnforcerBehavior(
        params MetricStatus[] exhaustedMetricStatuses)
    {
        return new QuotaEnforcerBehavior<TestCommand, Unit>(new QuotaCheckerStub(new(exhaustedMetricStatuses)));
    }
}

internal class QuotaCheckerStub : IQuotaChecker
{
    private readonly CheckQuotaResult _expectedResult;

    public QuotaCheckerStub(CheckQuotaResult expectedResult)
    {
        _expectedResult = expectedResult;
    }

    public Task<CheckQuotaResult> CheckQuotaExhaustion(IEnumerable<MetricKey> metricKeys)
    {
        return Task.FromResult(_expectedResult);
    }
}

/// <summary>
/// The Metric key doesn't matter for these tests. The way the Mediatr Behavior is being
/// called does not inject the Metrics passed on the Attribute below. Tests will make use of
/// all the metrics available in the repository unless where specified.
/// </summary>
[ApplyQuotasForMetrics("DoesNotApplyToTests")]
internal class TestCommand : IRequest
{
}
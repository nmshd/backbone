using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Application.IntegrationEvents.Outgoing;
using Backbone.Modules.Quotas.Application.Tests.Extensions;
using Backbone.Modules.Quotas.Application.Tiers.Commands.DeleteTierQuotaDefinition;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;
using Enmeshed.BuildingBlocks.Domain;
using FakeItEasy;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.Logging;
using Xunit;
using MetricKey = Backbone.Modules.Quotas.Domain.Aggregates.Metrics.MetricKey;

namespace Backbone.Modules.Quotas.Application.Tests.Tests.Quotas.DeleteTierQuotaDefinition;
public class HandlerTests
{
    private readonly IEventBus _eventBus;

    public HandlerTests()
    {
        _eventBus = A.Fake<IEventBus>();
        AssertionScope.Current.FormattingOptions.MaxLines = 1000;
    }

    [Fact]
    public async Task Triggers_TierQuotaDefinitionDeletedIntegrationEvent()
    {
        // Arrange
        var tierId = new TierId("SomeTierId");
        var tier = new Tier(tierId, "some-tier-name");

        tier.CreateQuota(MetricKey.NumberOfSentMessages, 5, QuotaPeriod.Month);

        var command = new DeleteTierQuotaDefinitionCommand(tier.Id, tier.Quotas.First().Id);

        var tiersRepository = A.Fake<ITiersRepository>();
        A.CallTo(() => tiersRepository.Find(tierId, A<CancellationToken>._, A<bool>._)).Returns(tier);

        var handler = CreateHandler(tiersRepository);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        A.CallTo(() => _eventBus.Publish(A<IntegrationEvent>.That.IsInstanceOf(typeof(TierQuotaDefinitionDeletedIntegrationEvent)))).MustHaveHappenedOnceExactly();
    }

    private Handler CreateHandler(ITiersRepository tiersRepository)
    {
        var logger = A.Fake<ILogger<Handler>>();

        return new Handler(tiersRepository, logger, _eventBus);
    }
}



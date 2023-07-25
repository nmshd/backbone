﻿using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Application.Tiers.Commands.DeleteQuotaForIdentity;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using Enmeshed.BuildingBlocks.Domain;
using Enmeshed.UnitTestTools.Extensions;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Xunit;
using Handler = Backbone.Modules.Quotas.Application.Tiers.Commands.DeleteQuotaForIdentity.Handler;
using MetricKey = Backbone.Modules.Quotas.Domain.Aggregates.Metrics.MetricKey;

namespace Backbone.Modules.Quotas.Application.Tests.Tests.Quotas.DeleteIndividualQuota;
public class HandlerTests
{
    [Fact]
    public async Task Delete_individual_quota()
    {
        // Arrange
        var tierId = new TierId("SomeTierId");
        var identityAddress = "some-identity-address";
        var identity = new Identity("some-identity-address", tierId);
        var createdQuota = identity.CreateIndividualQuota(MetricKey.NumberOfSentMessages, 5, QuotaPeriod.Month);
        var command = new DeleteQuotaForIdentityCommand(identityAddress, createdQuota.Id);
        var identitiesRepository = A.Fake<IIdentitiesRepository>();
        A.CallTo(() => identitiesRepository.Find(identityAddress, A<CancellationToken>._, A<bool>._)).Returns(identity);
        var handler = CreateHandler(identitiesRepository);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        A.CallTo(() => identitiesRepository.Update(A<Identity>.That.Matches(t =>
                t.Address == identityAddress &&
                t.IndividualQuotas.Count == 0)
            , CancellationToken.None)
        ).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Deletes_individual_quota_for_identity_with_multiple_quotas()
    {
        // Arrange
        var tierId = new TierId("SomeTierId");
        var identityAddress = "some-identity-address";
        var identity = new Identity("some-identity-address", tierId);
        var createdQuota = identity.CreateIndividualQuota(MetricKey.NumberOfSentMessages, 5, QuotaPeriod.Month);
        identity.CreateIndividualQuota(MetricKey.NumberOfSentMessages, 5, QuotaPeriod.Month);
        identity.CreateIndividualQuota(MetricKey.NumberOfSentMessages, 5, QuotaPeriod.Month);
        var command = new DeleteQuotaForIdentityCommand(identityAddress, createdQuota.Id);
        var identitiesRepository = A.Fake<IIdentitiesRepository>();
        A.CallTo(() => identitiesRepository.Find(identityAddress, A<CancellationToken>._, A<bool>._)).Returns(identity);
        var handler = CreateHandler(identitiesRepository);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        A.CallTo(() => identitiesRepository.Update(A<Identity>.That.Matches(t =>
                t.Address == identityAddress &&
                t.IndividualQuotas.Count == 2)
            , CancellationToken.None)
        ).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public void Fails_to_delete_individual_quota_for_inexistent_identity()
    {
        // Arrange
        var tierId = new TierId("SomeTierId");
        var command = new DeleteQuotaForIdentityCommand("some-inexistent-identity", "some-quota-id");
        var identitiesRepository = A.Fake<IIdentitiesRepository>();
        A.CallTo(() => identitiesRepository.Find("some-inexistent-identity", A<CancellationToken>._, A<bool>._)).Returns(Task.FromResult<Identity>(null));
        var handler = CreateHandler(identitiesRepository);

        // Act
        Func<Task> acting = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        var exception = acting.Should().AwaitThrowAsync<NotFoundException>().Which;
        exception.Message.Should().StartWith("Identity");
        exception.Code.Should().Be("error.platform.recordNotFound");
    }

    [Fact]
    public void Fails_to_delete_individual_quota_when_providing_an_inexistent_quota_id()
    {
        // Arrange
        var tierId = new TierId("SomeTierId");
        var identityAddress = "some-identity-address";
        var identity = new Identity("some-identity-address", tierId);
        var command = new DeleteQuotaForIdentityCommand(identityAddress, "some-quota-id");
        var identitiesRepository = A.Fake<IIdentitiesRepository>();
        A.CallTo(() => identitiesRepository.Find(identityAddress, A<CancellationToken>._, A<bool>._)).Returns(identity);
        var handler = CreateHandler(identitiesRepository);

        // Act
        Func<Task> acting = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        var exception = acting.Should().AwaitThrowAsync<DomainException>().Which;
        exception.Message.Should().StartWith("IndividualQuota");
        exception.Code.Should().Be("error.platform.recordNotFound");
    }

    private Handler CreateHandler(IIdentitiesRepository identitiesRepository)
    {
        var logger = A.Fake<ILogger<Handler>>();

        return new Handler(identitiesRepository, logger);
    }
}

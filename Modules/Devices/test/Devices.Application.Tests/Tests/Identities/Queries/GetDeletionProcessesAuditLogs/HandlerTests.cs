using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Application.Identities.Queries.GetDeletionProcessesAuditLogs;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Application.Tests.Tests.Identities.Queries.GetIdentity;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.UnitTestTools.BaseClasses;
using Backbone.UnitTestTools.Extensions;
using FakeItEasy;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Devices.Application.Tests.Tests.Identities.Queries.GetDeletionProcessesAuditLogs;

public class HandlerTests : AbstractTestsBase
{
    [Fact]
    public async Task Gets_audit_logs_of_identity_with_address()
    {
        // Arrange
        var identity = TestDataGenerator.CreateIdentityWithThreeAuditLogEntries();

        var handler = CreateHandler(new FindByAddressStubRepository(identity));

        // Act
        var result = await handler.Handle(new GetDeletionProcessesAuditLogsQuery(identity.Address), CancellationToken.None);

        // Assert
        result.ToList().Count.Should().Be(3);
    }

    [Fact]
    public void Fails_when_no_identity_found()
    {
        // Arrange
        var identityRepository = A.Fake<IIdentitiesRepository>();
        A.CallTo(() => identityRepository.FindByAddress(A<IdentityAddress>._, A<CancellationToken>._, A<bool>._)).Returns<Identity?>(null);

        var handler = CreateHandler(identityRepository);

        // Act
        Func<Task> acting = async () => await handler.Handle(new GetDeletionProcessesAuditLogsQuery("some-inexistent-identity-address"), CancellationToken.None);

        // Assert
        var exception = acting.Should().AwaitThrowAsync<NotFoundException>().Which;
        exception.Message.Should().StartWith("Identity");
        exception.Code.Should().Be("error.platform.recordNotFound");
    }

    private Handler CreateHandler(IIdentitiesRepository identitiesRepository)
    {
        return new Handler(identitiesRepository);
    }
}

using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Application.Identities.Commands.UpdateDeletionProcesses;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications.Handles;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Tooling;
using Backbone.UnitTestTools.Extensions;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Backbone.Modules.Devices.Application.Tests.Tests.Identities.Commands.UpdateDeletionProcesses;
public class HandlerTests
{
    private static List<Identity> _identities;

    [Fact]
    public async Task Empty_response_if_no_identities_are_past_DeletionGracePeriodEndsAt()
    {
        // Arrange
        var identitiesRepository = CreateFakeIdentitiesRepository();

        var anIdentity = _identities.First();
        anIdentity.StartDeletionProcess(new Device(anIdentity).Id);
        anIdentity.DeletionGracePeriodEndsAt = SystemTime.UtcNow.AddDays(30); // Future

        var handler = CreateHandler(identitiesRepository);
        var command = new UpdateDeletionProcessesCommand();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IdentityAddresses.Should().BeEmpty();
        A.CallTo(() => identitiesRepository.FindAllWithPastDeletionGracePeriod(A<CancellationToken>._, A<bool>._)).MustHaveHappenedOnceOrMore();
    }

    [Fact]
    public async Task Response_contains_identities_which_are_past_DeletionGracePeriodEndsAt()
    {
        // Arrange
        var identitiesRepository = CreateFakeIdentitiesRepository();

        var anIdentity = _identities.First();
        anIdentity.StartDeletionProcess(new Device(anIdentity).Id);
        anIdentity.DeletionGracePeriodEndsAt = SystemTime.UtcNow.AddDays(-1); // Past

        var handler = CreateHandler(identitiesRepository);
        var command = new UpdateDeletionProcessesCommand();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IdentityAddresses.Should().HaveCount(1);
        result.IdentityAddresses.First().Should().Be(anIdentity.Address);
        A.CallTo(() => identitiesRepository.FindAllWithPastDeletionGracePeriod(A<CancellationToken>._, A<bool>._)).MustHaveHappenedOnceOrMore();
    }

    [Fact]
    public async Task Handler_continues_if_Identity_without_IdentityDeletionProcess_has_past_DeletionGracePeriodEndsAt()
    {
        // Arrange
        var identitiesRepository = CreateFakeIdentitiesRepository();

        var anIdentity = _identities.First();
        // anIdentity.StartDeletionProcess(new Device(anIdentity).Id); // not called
        anIdentity.DeletionGracePeriodEndsAt = SystemTime.UtcNow.AddDays(-1); // Past

        var anotherIdentity = _identities.Second();
        anotherIdentity.StartDeletionProcess(new Device(anIdentity).Id); // not called
        anotherIdentity.DeletionGracePeriodEndsAt = SystemTime.UtcNow.AddDays(-1); // Past

        var handler = CreateHandler(identitiesRepository);
        var command = new UpdateDeletionProcessesCommand();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IdentityAddresses.Should().HaveCount(1);
        result.IdentityAddresses.First().Should().Be(anotherIdentity.Address);
        A.CallTo(() => identitiesRepository.FindAllWithPastDeletionGracePeriod(A<CancellationToken>._, A<bool>._)).MustHaveHappenedOnceOrMore();
    }

    [Fact]
    public async Task Handler_calls_delete_for_PnsRegistrations_for_each_identity_to_be_deleted()
    {
        // Arrange
        var identitiesRepository = CreateFakeIdentitiesRepository();

        var anIdentity = _identities.First();
        anIdentity.StartDeletionProcess(new Device(anIdentity).Id);
        anIdentity.DeletionGracePeriodEndsAt = SystemTime.UtcNow.AddDays(-1); // Past

        var anotherIdentity = _identities.Second();
        anotherIdentity.StartDeletionProcess(new Device(anotherIdentity).Id);
        anotherIdentity.DeletionGracePeriodEndsAt = SystemTime.UtcNow.AddDays(-2); // Past

        var pnsRegistrationRepository = A.Fake<IPnsRegistrationRepository>();

        var handler = CreateHandler(identitiesRepository, pnsRegistrationRepository);
        var command = new UpdateDeletionProcessesCommand();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        A.CallTo(() => pnsRegistrationRepository.DeleteByIdentityAddress(A<IdentityAddress>._, A<CancellationToken>._)).MustHaveHappenedTwiceExactly();
    }

    private static Handler CreateHandler(IIdentitiesRepository identitiesRepository, IPnsRegistrationRepository pnsRegistrationRepository = null)
    {
        return new Handler(
            identitiesRepository,
            pnsRegistrationRepository ?? A.Dummy<IPnsRegistrationRepository>(),
            A.Dummy<ILogger<Handler>>()
            );
    }

    private static IIdentitiesRepository CreateFakeIdentitiesRepository()
    {
        var tierId = TestDataGenerator.CreateRandomTierId();
        _identities = new List<Identity>();

        for (var i = 0; i < 5; i++)
        {
            var identity = TestDataGenerator.CreateIdentityWithTier(tierId);
            _identities.Add(identity);
        }

        var identitiesRepository = A.Fake<IIdentitiesRepository>();
        A.CallTo(() => identitiesRepository.FindAllWithPastDeletionGracePeriod(A<CancellationToken>._, A<bool>._)).Returns(_identities.Where(i => i.DeletionGracePeriodEndsAt < SystemTime.UtcNow));

        return identitiesRepository;
    }
}

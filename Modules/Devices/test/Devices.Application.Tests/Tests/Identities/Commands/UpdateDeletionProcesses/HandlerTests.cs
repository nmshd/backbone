using System.Linq.Expressions;
using Backbone.Modules.Devices.Application.Identities.Commands.TriggerRipeDeletionProcesses;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Tooling;
using Backbone.UnitTestTools.Shouldly.Extensions;
using FakeItEasy;

namespace Backbone.Modules.Devices.Application.Tests.Tests.Identities.Commands.UpdateDeletionProcesses;

public class HandlerTests : AbstractTestsBase
{
    [Fact]
    public async Task Empty_response_if_no_identities_are_past_DeletionGracePeriodEndsAt()
    {
        // Arrange
        var mockIdentitiesRepository = CreateFakeIdentitiesRepository(0, out _);

        var handler = CreateHandler(mockIdentitiesRepository);

        // Act
        var response = await handler.Handle(new TriggerRipeDeletionProcessesCommand(), CancellationToken.None);

        // Assert
        response.Results.ShouldBeEmpty();
    }

    [Fact]
    public async Task Response_contains_expected_identities()
    {
        // Arrange
        var identitiesRepository = CreateFakeIdentitiesRepository(1, out var identities);

        var anIdentity = identities.First();
        anIdentity.StartDeletionProcess(anIdentity.Devices.First().Id);

        var handler = CreateHandler(identitiesRepository);

        // Act
        var response = await handler.Handle(new TriggerRipeDeletionProcessesCommand(), CancellationToken.None);

        // Assert
        response.Results.ShouldHaveCount(1);
        response.Results.Single().Key.ShouldBe(anIdentity.Address);
    }

    [Fact]
    public async Task Deletion_process_started_successfully()
    {
        // Arrange
        var identitiesRepository = CreateFakeIdentitiesRepository(1, out var identities);

        var anIdentity = identities.First();
        anIdentity.StartDeletionProcess(anIdentity.Devices.First().Id);

        SystemTime.Set(SystemTime.UtcNow.AddDays(IdentityDeletionConfiguration.Instance.LengthOfGracePeriodInDays));

        var handler = CreateHandler(identitiesRepository);

        // Act
        var response = await handler.Handle(new TriggerRipeDeletionProcessesCommand(), CancellationToken.None);

        // Assert
        response.Results.ShouldHaveCount(1);
        identities.First().Status.ShouldBe(IdentityStatus.Deleting);
    }

    private static Handler CreateHandler(IIdentitiesRepository identitiesRepository)
    {
        return new Handler(identitiesRepository);
    }

    private static IIdentitiesRepository CreateFakeIdentitiesRepository(ushort numberOfIdentities, out List<Identity> returnedIdentities)
    {
        returnedIdentities = [];

        for (var i = 0; i < numberOfIdentities; i++)
            returnedIdentities.Add(TestDataGenerator.CreateIdentityWithOneDevice());

        var identitiesRepository = A.Fake<IIdentitiesRepository>();
        A.CallTo(() => identitiesRepository.List(A<Expression<Func<Identity, bool>>>._, A<CancellationToken>._, A<bool>._)).Returns(returnedIdentities);

        return identitiesRepository;
    }
}

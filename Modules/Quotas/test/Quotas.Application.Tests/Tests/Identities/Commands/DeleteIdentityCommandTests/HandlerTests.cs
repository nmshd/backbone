using Backbone.Modules.Quotas.Application.Identities.Commands.DeleteIdentity;
using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using static Backbone.UnitTestTools.Data.TestDataGenerator;
using FakeItEasy;
using Xunit;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;

namespace Backbone.Modules.Quotas.Application.Tests.Tests.Identities.Commands.DeleteIdentityCommandTests;
public class HandlerTests
{
    [Fact]
    public async Task Handler_calls_deletion_method_on_repository()
    {
        // Arrange
        var identity = new Identity(CreateRandomIdentityAddress(), new TierId("tier-id"));
        var identitiesRepository = A.Fake<IIdentitiesRepository>();
        var handler = CreateHandler(identitiesRepository);
        A.CallTo(() => identitiesRepository.Find(identity.Address, A<CancellationToken>._, A<bool>._)).Returns(identity);

        // Act
        await handler.Handle(new DeleteIdentityCommand(identity.Address), CancellationToken.None);

        // Assert
        A.CallTo(() => identitiesRepository.Delete(identity, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
    }

    private static Handler CreateHandler(IIdentitiesRepository identitiesRepository = null)
    {
        return new Handler(identitiesRepository ?? A.Fake<IIdentitiesRepository>());
    }
}

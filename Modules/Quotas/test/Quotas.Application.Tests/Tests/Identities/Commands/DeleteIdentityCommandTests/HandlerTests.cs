using System.Linq.Expressions;
using Backbone.Modules.Quotas.Application.Identities.Commands.DeleteIdentity;
using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using FakeItEasy;
using Xunit;
using static Backbone.UnitTestTools.Data.TestDataGenerator;

namespace Backbone.Modules.Quotas.Application.Tests.Tests.Identities.Commands.DeleteIdentityCommandTests;
public class HandlerTests
{
    [Fact]
    public async Task Handler_calls_deletion_method_on_repository()
    {
        var identity = new Identity(CreateRandomIdentityAddress(), new TierId("tier-id"));
        var mockIdentitiesRepository = A.Fake<IIdentitiesRepository>();
        var handler = CreateHandler(mockIdentitiesRepository);

        await handler.Handle(new DeleteIdentityCommand(identity.Address), CancellationToken.None);

        A.CallTo(() => mockIdentitiesRepository.Delete(A<Expression<Func<Identity, bool>>>._, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
    }

    private static Handler CreateHandler(IIdentitiesRepository identitiesRepository)
    {
        return new Handler(identitiesRepository ?? A.Fake<IIdentitiesRepository>());
    }
}

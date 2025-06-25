using System.Linq.Expressions;
using Backbone.Modules.Quotas.Application.Identities.Commands.DeleteIdentity;
using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using FakeItEasy;

namespace Backbone.Modules.Quotas.Application.Tests.Tests.Identities.Commands.DeleteIdentityCommandTests;

public class HandlerTests : AbstractTestsBase
{
    [Fact]
    public async Task Handler_calls_deletion_method_on_repository()
    {
        var identity = new Identity(CreateRandomIdentityAddress(), TierId.Parse("tier-id"));
        var mockIdentitiesRepository = A.Fake<IIdentitiesRepository>();
        var handler = CreateHandler(mockIdentitiesRepository);

        await handler.Handle(new DeleteIdentityCommand { IdentityAddress = identity.Address }, CancellationToken.None);

        A.CallTo(() => mockIdentitiesRepository.Delete(A<Expression<Func<Identity, bool>>>._, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
    }

    private static Handler CreateHandler(IIdentitiesRepository identitiesRepository)
    {
        return new Handler(identitiesRepository);
    }
}

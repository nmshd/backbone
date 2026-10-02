using DeleteIdentity = Backbone.Modules.Quotas.Module.Features.Identities.DeleteIdentity;
using System.Linq.Expressions;
using Backbone.Modules.Quotas.Module.Features.Identities.DeleteIdentity;
using Backbone.Modules.Quotas.Abstractions;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using FakeItEasy;

namespace Backbone.Modules.Quotas.Module.Tests.Tests.Identities.Commands.DeleteIdentityCommandTests;

public class HandlerTests : AbstractTestsBase
{
    [Fact]
    public async Task Handler_calls_deletion_method_on_repository()
    {
        var identity = new Identity(CreateRandomIdentityAddress(), TierId.Parse("tier-id"));
        var mockIdentitiesRepository = A.Fake<IIdentitiesRepository>();
        var handler = CreateHandler(mockIdentitiesRepository);

        await handler.Handle(new DeleteIdentity.Command { IdentityAddress = identity.Address }, CancellationToken.None);

        A.CallTo(() => mockIdentitiesRepository.Delete(A<Expression<Func<Identity, bool>>>._, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
    }

    private static Handler CreateHandler(IIdentitiesRepository identitiesRepository)
    {
        return new Handler(identitiesRepository);
    }
}

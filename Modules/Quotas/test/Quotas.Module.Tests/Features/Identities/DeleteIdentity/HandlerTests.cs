using System.Linq.Expressions;
using Backbone.Modules.Quotas.Abstractions;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using Backbone.Modules.Quotas.Module.Features.Identities.DeleteIdentity;
using FakeItEasy;
using DeleteIdentitySlice = Backbone.Modules.Quotas.Module.Features.Identities.DeleteIdentity;

namespace Backbone.Modules.Quotas.Module.Tests.Features.Identities.DeleteIdentity;

public class HandlerTests : AbstractTestsBase
{
    [Fact]
    public async Task Handler_calls_deletion_method_on_repository()
    {
        var identity = new Identity(CreateRandomIdentityAddress(), TierId.Parse("tier-id"));
        var mockIdentitiesRepository = A.Fake<IIdentitiesRepository>();
        var handler = CreateHandler(mockIdentitiesRepository);

        await handler.Handle(new DeleteIdentitySlice.Command { IdentityAddress = identity.Address }, CancellationToken.None);

        A.CallTo(() => mockIdentitiesRepository.Delete(A<Expression<Func<Identity, bool>>>._, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
    }

    private static Handler CreateHandler(IIdentitiesRepository identitiesRepository)
    {
        return new Handler(identitiesRepository);
    }
}

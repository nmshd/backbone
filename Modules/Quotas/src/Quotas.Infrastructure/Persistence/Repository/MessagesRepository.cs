using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Quotas.Infrastructure.Persistence.Repository;
public class MessagesRepository : IMessagesRepository
{
    public Task<uint> Count(IdentityAddress sender, DateTime createdAtFrom, DateTime createdAtTo)
    {
        return Task.FromResult(0u);
    }
}

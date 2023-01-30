using Backbone.Modules.Messages.Domain.Entities;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Messages.Application.Extensions;

public static class RecipientInformationICollectionExtensions
{
    public static RecipientInformation FirstWithIdOrDefault(this IReadOnlyCollection<RecipientInformation> query, IdentityAddress address)
    {
        return query.FirstOrDefault(r => r.Address == address);
    }
}

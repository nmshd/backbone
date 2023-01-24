using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Messages.Domain.Entities;

namespace Messages.Application.Extensions;

public static class RecipientInformationICollectionExtensions
{
    public static RecipientInformation FirstWithIdOrDefault(this IReadOnlyCollection<RecipientInformation> query, IdentityAddress address)
    {
        return query.FirstOrDefault(r => r.Address == address);
    }
}

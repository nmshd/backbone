using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Messages.Domain.Entities;

namespace Backbone.Modules.Messages.Module.Features.Messages.Shared;

public static class RecipientInformationExtensions
{
    public static RecipientInformation? FirstWithIdOrDefault(this IReadOnlyCollection<RecipientInformation> recipients, IdentityAddress address)
    {
        return recipients.FirstOrDefault(recipient => recipient.Address == address);
    }
}

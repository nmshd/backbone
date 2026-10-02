using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Devices.Module.Features.Identities.ListAddressesOfIdentitiesWithDeletionProcessInStatusDeleting;

public class ListAddressesOfIdentitiesWithDeletionProcessInStatusDeletingResponse
{
    public ListAddressesOfIdentitiesWithDeletionProcessInStatusDeletingResponse(IEnumerable<IdentityAddress> identityAddresses)
    {
        Addresses = identityAddresses.Select(a => a.Value).ToList();
    }

    public List<string> Addresses { get; }
}

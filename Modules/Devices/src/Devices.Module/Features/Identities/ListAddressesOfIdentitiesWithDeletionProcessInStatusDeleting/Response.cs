using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Devices.Module.Features.Identities.ListAddressesOfIdentitiesWithDeletionProcessInStatusDeleting;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("ListAddressesOfIdentitiesWithDeletionProcessInStatusDeletingResponse")]
public class Response
{
    public Response(IEnumerable<IdentityAddress> identityAddresses)
    {
        Addresses = identityAddresses.Select(a => a.Value).ToList();
    }

    public List<string> Addresses { get; }
}

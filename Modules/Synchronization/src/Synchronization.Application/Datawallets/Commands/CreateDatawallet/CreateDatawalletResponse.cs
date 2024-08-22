using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Synchronization.Domain.Entities;

namespace Backbone.Modules.Synchronization.Application.Datawallets.Commands.CreateDatawallet;
public class CreateDatawalletResponse
{
    public required DatawalletId DatawalletId { get; set; }
    public required IdentityAddress Owner { get; set; }
    public required ushort Version { get; set; }
}

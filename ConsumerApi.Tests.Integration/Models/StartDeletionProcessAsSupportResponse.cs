using Backbone.Modules.Devices.Domain.Entities.Identities;

namespace Backbone.ConsumerApi.Tests.Integration.Models;

public class StartDeletionProcessAsSupportResponse
{
    public required string Id { get; set; }
    public required DeletionProcessStatus Status { get; set; }
    public required DateTime CreatedAt { get; set; }
}

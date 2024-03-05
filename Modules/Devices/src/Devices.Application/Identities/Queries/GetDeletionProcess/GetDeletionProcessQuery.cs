using Backbone.Modules.Devices.Application.DTOs;
using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Queries.GetDeletionProcess;

public class GetDeletionProcessQuery : IRequest<IdentityDeletionProcessDTO>
{
    public required string Id { get; set; }
}

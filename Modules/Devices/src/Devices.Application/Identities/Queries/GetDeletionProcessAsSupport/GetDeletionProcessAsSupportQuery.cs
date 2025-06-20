using Backbone.Modules.Devices.Application.DTOs;
using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Queries.GetDeletionProcessAsSupport;

public class GetDeletionProcessAsSupportQuery : IRequest<IdentityDeletionProcessDetailsDTO>
{
    public required string IdentityAddress { get; init; }
    public required string DeletionProcessId { get; init; }
}

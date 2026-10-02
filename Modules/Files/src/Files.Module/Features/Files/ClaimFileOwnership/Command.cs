using MediatR;

namespace Backbone.Modules.Files.Module.Features.Files.ClaimFileOwnership;

public class Command : IRequest<Response>
{
    public required string FileId { get; init; }
    public required string OwnershipToken { get; init; }
}

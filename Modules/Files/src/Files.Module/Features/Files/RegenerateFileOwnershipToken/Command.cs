using MediatR;

namespace Backbone.Modules.Files.Module.Features.Files.RegenerateFileOwnershipToken;

public class Command : IRequest<Response>
{
    public required string FileId { get; init; }
}

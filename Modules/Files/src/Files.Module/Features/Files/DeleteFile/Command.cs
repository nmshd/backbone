using MediatR;

namespace Backbone.Modules.Files.Module.Features.Files.DeleteFile;

public class Command : IRequest
{
    public required string Id { get; init; }
}

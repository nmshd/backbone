using MediatR;

namespace Backbone.Modules.Files.Module.Features.Files.DeleteFile;

public class DeleteFileCommand : IRequest
{
    public required string Id { get; init; }
}

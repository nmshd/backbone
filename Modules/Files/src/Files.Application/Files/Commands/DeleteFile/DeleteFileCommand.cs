using MediatR;

namespace Backbone.Modules.Files.Application.Files.Commands.DeleteFile;

public class DeleteFileCommand : IRequest
{
    public required string Id { get; init; }
}

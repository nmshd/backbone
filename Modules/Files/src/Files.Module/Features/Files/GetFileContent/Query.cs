using MediatR;

namespace Backbone.Modules.Files.Module.Features.Files.GetFileContent;

public class GetFileContentQuery : IRequest<GetFileContentResponse>
{
    public required string Id { get; init; }
}

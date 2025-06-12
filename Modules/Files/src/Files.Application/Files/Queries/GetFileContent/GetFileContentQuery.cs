using MediatR;

namespace Backbone.Modules.Files.Application.Files.Queries.GetFileContent;

public class GetFileContentQuery : IRequest<GetFileContentResponse>
{
    public required string Id { get; init; }
}

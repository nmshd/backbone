using MediatR;

namespace Backbone.Modules.Files.Module.Features.Files.GetFileContent;

public class Query : IRequest<Response>
{
    public required string Id { get; init; }
}

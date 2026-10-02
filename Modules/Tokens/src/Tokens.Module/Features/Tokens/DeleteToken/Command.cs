using MediatR;

namespace Backbone.Modules.Tokens.Module.Features.Tokens.DeleteToken;

public class Command : IRequest
{
    public required string Id { get; init; }
}

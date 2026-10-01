using MediatR;

namespace Backbone.Modules.Tokens.Module.Features.Tokens.DeleteToken;

public class DeleteTokenCommand : IRequest
{
    public required string Id { get; init; }
}

using MediatR;

namespace Backbone.Modules.Files.Module.Features.Files.ValidateFileOwnershipToken;

public class ValidateFileOwnershipTokenQuery : IRequest<ValidateFileOwnershipTokenResponse>
{
    public required string FileId { get; init; }
    public required string OwnershipToken { get; init; }
}
